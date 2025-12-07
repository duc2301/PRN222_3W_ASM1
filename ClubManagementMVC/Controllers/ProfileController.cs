using AutoMapper;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementMVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly IMapper _mapper;

        public ProfileController(IServiceProviders serviceProviders, IMapper mapper)
        {
            _serviceProviders = serviceProviders;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<UserResponseDTO>> Index()
        {
            var userName = HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Auth");
            }
            var user = await _serviceProviders.UserService.GetByUsernameAsync(userName);                
            return View(_mapper.Map<UserResponseDTO>(user));
        }
    }
}
