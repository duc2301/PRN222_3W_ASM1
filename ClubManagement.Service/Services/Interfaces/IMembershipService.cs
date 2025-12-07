using ClubManagement.Service.DTOs.RequestDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IMembershipService
    {
        Task UpdateMemberAsync(UpdateMemberRequestDTO request);
    }
    
}
