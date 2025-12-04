using ClubManagement.Service.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementMVC.Controllers.Interface
{
    public interface IJoinRequestController
    {
        Task<IActionResult> Index();
        Task<IActionResult> Submit();
        Task<IActionResult> Submit(SubmitJoinRequestDTO dto);
        Task<IActionResult> Approve(int id);
        Task<IActionResult> ApproveConfirmed(ApproveJoinRequestDTO dto);
        Task<IActionResult> Reject(int id);
        Task<IActionResult> RejectConfirmed(RejectJoinRequestDTO dto);
    }
}
