using AutoMapper;
using ClubManagement.Repository.Basic.Interfaces;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;
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
        private readonly IGenericRepository<Membership> _membershipRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly ClubManagementContext _context;

        public async Task<IEnumerable<JoinRequest>> GetAllAsync()
        {
            return await _joinRequestRepo.GetAllAsync();
        }

        public async Task<JoinRequest?> GetByIdAsync(int id)
        {
            return await _joinRequestRepo.GetByIdAsync(id);
        }
        public JoinRequestService(
            IJoinRequestRepository joinRequestRepo,
            IGenericRepository<Membership> membershipRepo,
            IGenericRepository<User> userRepo,
            ClubManagementContext context)
        {
            _joinRequestRepo = joinRequestRepo;
            _membershipRepo = membershipRepo;
            _userRepo = userRepo;
            _context = context;
        }

        // ----------------------------------------------------
        // 1) Submit Join Request
        // ----------------------------------------------------
        public async Task<JoinRequest> SubmitAsync(int userId, int clubId, string? note)
        {
            // Optional: check user tồn tại
            var user = await _userRepo.GetByIdAsync(userId)
                       ?? throw new Exception("User không tồn tại.");

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
            var request = await _joinRequestRepo.GetByIdAsync(requestId)
                          ?? throw new Exception("Không tìm thấy đơn.");

            if (request.Status != "Pending")
                throw new Exception("Đơn đã được xử lý trước đó.");

            request.Status = "Approved";

            // Optional: tạo membership
            var membership = new Membership
            {
                UserId = request.UserId,
                ClubId = request.ClubId,
                JoinedAt = DateTime.Now,
                Status = "Active"
            };

            await _membershipRepo.CreateAsync(membership);
            _joinRequestRepo.Update(request);

            await _context.SaveChangesAsync();

            return request;
        }

        // ----------------------------------------------------
        // 3) Reject Join Request
        // ----------------------------------------------------
        public async Task<JoinRequest> RejectAsync(int requestId, int leaderId, string? reason)
        {
            var request = await _joinRequestRepo.GetByIdAsync(requestId)
                          ?? throw new Exception("Không tìm thấy đơn.");

            if (request.Status != "Pending")
                throw new Exception("Đơn đã được xử lý trước đó.");

            request.Status = "Rejected";
            request.Note = reason;

            _joinRequestRepo.Update(request);
            await _context.SaveChangesAsync();

            return request;
        }
    }
}
