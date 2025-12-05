using ClubManagement.Repository.Models;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClubManagement.Service.DTOs.RequestDTOs.Activity;

namespace ClubManagement.Service
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // Club mappings
            CreateMap<Club, ClubResponseDTO>().ReverseMap();
            CreateMap<Club, CreateClubRequestDTO>().ReverseMap();
            CreateMap<Club, UpdateClubRequestDTO>().ReverseMap();
            CreateMap<ClubResponseDTO, CreateClubRequestDTO>().ReverseMap();
            CreateMap<ClubResponseDTO, UpdateClubRequestDTO>().ReverseMap();
            
            // User mappings
            CreateMap<User, UserResponseDTO>().ReverseMap();

            // Activity mappings
            CreateMap<Activity, ActivityResponseDTO>().ReverseMap();
            CreateMap<Activity, ActivityCreateDTO>().ReverseMap();
            CreateMap<Activity, ActivityUpdateDTO>().ReverseMap();
            
            // ActivityParticipant mappings
            CreateMap<ActivityParticipant, ActivityParticipantResponseDTO>().ReverseMap();
            
            // Map phức tạp (Khác tên hoặc lấy dữ liệu từ bảng khác)
            //CreateMap<Club, ClubResponseDTO>()
            //    .ForMember(dest => dest.LeaderName, 
            //               opt => opt.MapFrom(src => src.Leader.FullName));

            // Map cho Create/Update (Request DTO)             
            // CreateMap<ClubCreateDTO, Club>();
        }
    }
}
