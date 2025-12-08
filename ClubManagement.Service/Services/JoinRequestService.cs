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

        // ----------------------------------------------------
        // 1) Submit Join Request
        // ----------------------------------------------------
        public async Task<JoinRequest> SubmitAsync(int userId, int clubId, string? note)
        {
            // Kiểm tra user tồn tại
            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new Exception("User không tồn tại.");

            // Kiểm tra club tồn tại
            var club = await _clubRepo.GetByIdAsync(clubId)
                       ?? throw new Exception("Câu lạc bộ không tồn tại.");

            // Kiểm tra xem user đã là member chưa
            var existingMembership = await _context.Memberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.ClubId == clubId);

            if (existingMembership != null && existingMembership.Status == "Active")
            {
                throw new Exception("Bạn đã là thành viên của câu lạc bộ này.");
            }

            // Kiểm tra xem có pending request nào chưa
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

        // ----------------------------------------------------
        // 2) Approve Join Request
        // ----------------------------------------------------
        public async Task<JoinRequest> ApproveAsync(int requestId, int leaderId)
        {
            Console.WriteLine($"🔥 ApproveAsync called - RequestId: {requestId}, LeaderId: {leaderId}");

            var request = await _joinRequestRepo.GetByIdAsync(requestId)
                          ?? throw new Exception("Không tìm thấy đơn yêu cầu.");

            Console.WriteLine($"✅ Found request - UserId: {request.UserId}, ClubId: {request.ClubId}, Status: {request.Status}");

            if (request.Status != "Pending")
                throw new Exception("Đơn đã được xử lý trước đó.");

            // Cập nhật status của request
            request.Status = "Approved";
            Console.WriteLine($"📝 Updated request status to: {request.Status}");

            // Kiểm tra xem đã có membership chưa
            var existingMembership = await _context.Memberships
                .FirstOrDefaultAsync(m => m.UserId == request.UserId && m.ClubId == request.ClubId);

            if (existingMembership != null)
            {
                Console.WriteLine($"🔄 Updating existing membership - MembershipId: {existingMembership.MembershipId}");
                // Nếu đã có membership (có thể Inactive), cập nhật thành Active
                existingMembership.Status = "Active";
                existingMembership.JoinedAt = DateTime.Now;
                _context.Memberships.Update(existingMembership);
            }
            else
            {
                Console.WriteLine($"➕ Creating NEW membership - UserId: {request.UserId}, ClubId: {request.ClubId}");
                // Tạo membership mới
                var membership = new Membership
                {
                    UserId = request.UserId,
                    ClubId = request.ClubId,
                    JoinedAt = DateTime.Now,
                    Status = "Active",
                    Role = "Member" // Role mặc định
                };

                await _membershipRepo.CreateAsync(membership);
                Console.WriteLine($"✅ Membership created successfully!");
            }

            _joinRequestRepo.Update(request);
            await _context.SaveChangesAsync();
            Console.WriteLine($"💾 SaveChanges completed!");

            return request;
        }

        // ----------------------------------------------------
        // 3) Reject Join Request
        // ----------------------------------------------------
        public async Task<JoinRequest> RejectAsync(int requestId, int leaderId, string? reason)
        {
            var request = await _joinRequestRepo.GetByIdAsync(requestId)
                          ?? throw new Exception("Không tìm thấy đơn yêu cầu.");

            if (request.Status != "Pending")
                throw new Exception("Đơn đã được xử lý trước đó.");

            request.Status = "Rejected";
            // Lưu reason vào Note field (vì không có RejectReason field)
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