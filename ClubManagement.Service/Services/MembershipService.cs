using ClubManagement.Repository.UnitOfWork.Interface;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MembershipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateMemberAsync(UpdateMemberRequestDTO request)
        {
            // Lấy membership theo (UserId, ClubId)
            var membership = await _unitOfWork.MembershipRepository
                                              .GetByUserAndClubAsync(request.UserId, request.ClubId);

            if (membership == null)
            {
                // Tuỳ bạn: có thể throw exception, hoặc return false.
                return;
            }

            membership.Role = request.Role;
            membership.Status = request.Status;

            _unitOfWork.MembershipRepository.Update(membership);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
