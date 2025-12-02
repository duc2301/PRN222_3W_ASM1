using ClubManagement.Service.DTOs.ResponseDTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetAllAsync();
    }
}
