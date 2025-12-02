using ClubManagement.Repository.Models;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Club, ClubResponseDTO>().ReverseMap();
            CreateMap<Club, CreateClubRequestDTO>().ReverseMap();
            CreateMap<Club, UpdateClubRequestDTO>().ReverseMap();
            CreateMap<ClubResponseDTO, CreateClubRequestDTO>().ReverseMap();
            CreateMap<ClubResponseDTO, UpdateClubRequestDTO>().ReverseMap();
            CreateMap<User, UserResponseDTO>().ReverseMap();

            // Map phức tạp (Khác tên hoặc lấy dữ liệu từ bảng khác)
            //CreateMap<Club, ClubResponseDTO>()
            //    .ForMember(dest => dest.LeaderName, 
            //               opt => opt.MapFrom(src => src.Leader.FullName));

            // Map cho Create/Update (Request DTO)             
            // CreateMap<ClubCreateDTO, Club>();
        }
    }
}
