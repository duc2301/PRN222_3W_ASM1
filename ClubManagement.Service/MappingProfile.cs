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
            CreateMap<JoinRequest, JoinRequestResponseDTO>();

            // Activity mappings
            CreateMap<Activity, ActivityResponseDTO>().ReverseMap();
            CreateMap<Activity, ActivityCreateDTO>().ReverseMap();
            CreateMap<Activity, ActivityUpdateDTO>().ReverseMap();
            
            // ActivityParticipant mappings
            CreateMap<ActivityParticipant, ActivityParticipantResponseDTO>().ReverseMap();

            CreateMap<Payment, PaymentResponseDTO>().ReverseMap();

            // Map phức tạp (Khác tên hoặc lấy dữ liệu từ bảng khác)
            //CreateMap<Club, ClubResponseDTO>()
            //    .ForMember(dest => dest.LeaderName, 
            //               opt => opt.MapFrom(src => src.Leader.FullName));

            // Map cho Create/Update (Request DTO)             
            // CreateMap<ClubCreateDTO, Club>();

            CreateMap<Membership, ClubMemberListItemDTO>()
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.User.Department));

            // Fee mappings
            CreateMap<Fee, FeeResponseDTO>()
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club != null ? src.Club.ClubName : string.Empty));
            CreateMap<CreateFeeRequestDTO, Fee>();
        }
    }
}
