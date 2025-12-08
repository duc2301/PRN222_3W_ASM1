using AutoMapper;
using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services
{
    public class JoinRequestService : IJoinRequestService
    {
        private readonly IJoinRequestRepository _joinRequestRepo;
        private readonly IMembershipRepository _membershipRepo;
        private readonly IUserRepository _userRepo;
        private readonly IClubRepository _clubRepo;
        private readonly ClubManagementContext _context;

        public JoinRequestService(
            IJoinRequestRepository joinRequestRepo,
            IMembershipRepository membershipRepo,
            IUserRepository userRepo,
            IClubRepository clubRepo,
            ClubManagementContext context)
        {
            _joinRequestRepo = joinRequestRepo;
            _membershipRepo = membershipRepo;
            _userRepo = userRepo;
            _clubRepo = clubRepo;
            _context = context;
        }

        public async Task<IEnumerable<JoinRequest>> GetAllAsync()
        {
            return await _joinRequestRepo.GetAllAsync();
        }

        public async Task<JoinRequest?> GetByIdAsync(int id)
        {
            return await _joinRequestRepo.GetByIdAsync(id);
        }

        public async Task<JoinRequest> SubmitAsync(int userId, int clubId, string? note)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new Exception("User không tồn tại.");

            var club = await _clubRepo.GetByIdAsync(clubId)
                       ?? throw new Exception("Câu lạc bộ không tồn tại.");

            var existingMembership = await _context.Memberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.ClubId == clubId);

            if (existingMembership != null && existingMembership.Status == "Active")
            {
                throw new Exception("Bạn đã là thành viên của câu lạc bộ này.");
            }

            var pendingRequest = await _context.JoinRequests
                .FirstOrDefaultAsync(jr => jr.UserId == userId
                                        && jr.ClubId == clubId
                                        && jr.Status == "Pending");

            if (pendingRequest != null)
            {
                throw new Exception("Bạn đã có yêu cầu tham gia đang chờ duyệt.");
            }

            var request = new JoinRequest
            {
                UserId = userId,
                ClubId = clubId,
                RequestDate = DateTime.Now,
                Status = "Pending",
                Note = note
            };

            await _joinRequestRepo.CreateAsync(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<JoinRequest> ApproveAsync(int requestId, int leaderId)
        {
            Console.WriteLine($"🔥 ApproveAsync CALLED - RequestId: {requestId}, LeaderId: {leaderId}");

            var request = await _joinRequestRepo.GetByIdAsync(requestId)
                          ?? throw new Exception("Không tìm thấy đơn yêu cầu.");

            Console.WriteLine($"✅ Found request - UserId: {request.UserId}, ClubId: {request.ClubId}, Status: {request.Status}");

            if (request.Status != "Pending")
            {
                Console.WriteLine($"❌ Request already processed!");
                throw new Exception("Đơn đã được xử lý trước đó.");
            }

            // Update request status
            request.Status = "Approved";
            _joinRequestRepo.Update(request);
            Console.WriteLine($"📝 Request status updated to: Approved");

            // Check if membership exists
            var membershipExists = await _context.Memberships
                .AnyAsync(m => m.UserId == request.UserId && m.ClubId == request.ClubId);

            if (membershipExists)
            {
                Console.WriteLine($"🔄 Membership EXISTS - Updating to Active");

                // Update existing membership directly in database
                var rowsAffected = await _context.Memberships
                    .Where(m => m.UserId == request.UserId && m.ClubId == request.ClubId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(m => m.Status, "Active")
                        .SetProperty(m => m.JoinedAt, DateTime.Now));

                Console.WriteLine($"✅ Membership updated! Rows affected: {rowsAffected}");
            }
            else
            {
                Console.WriteLine($"➕ Creating NEW membership - UserId: {request.UserId}, ClubId: {request.ClubId}");

                var membership = new Membership
                {
                    UserId = request.UserId,
                    ClubId = request.ClubId,
                    JoinedAt = DateTime.Now,
                    Status = "Active",
                    Role = "Member"
                };

                await _membershipRepo.CreateAsync(membership);
                Console.WriteLine($"✅ NEW Membership created!");
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"💾 SaveChanges COMPLETED!");

            return request;
        }

        public async Task<JoinRequest> RejectAsync(int requestId, int leaderId, string? reason)
        {
            var request = await _joinRequestRepo.GetByIdAsync(requestId)
                          ?? throw new Exception("Không tìm thấy đơn yêu cầu.");

            if (request.Status != "Pending")
                throw new Exception("Đơn đã được xử lý trước đó.");

            request.Status = "Rejected";
            if (!string.IsNullOrEmpty(reason))
            {
                request.Note = string.IsNullOrEmpty(request.Note)
                    ? $"Lý do từ chối: {reason}"
                    : $"{request.Note}\nLý do từ chối: {reason}";
            }

            _joinRequestRepo.Update(request);
            await _context.SaveChangesAsync();

            return request;
        }
    }
}