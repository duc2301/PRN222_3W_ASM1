using ClubManagement.Repository.Models;
using ClubManagement.Service.ServiceProviders.Interface;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementMVC.Controllers
{
  
        [Authorize]
        public class FeeController : Controller
        {
            private readonly IFeeService _feeService;

            public FeeController(IFeeService feeService)
            {
                _feeService = feeService;
            }

            public async Task<IActionResult> Index()
            {
                return View(await _feeService.GetAllAsync());
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Create(Fee fee)
            {
                if (!ModelState.IsValid) return View(fee);

                await _feeService.CreateAsync(fee);

                TempData["msg"] = "Tạo khoản phí thành công.";
                return RedirectToAction("Index");
            }

            public async Task<IActionResult> Edit(int id)
            {
                var fee = await _feeService.GetByIdAsync(id);
                if (fee == null) return NotFound();

                return View(fee);
            }

            [HttpPost]
            public async Task<IActionResult> Edit(Fee fee)
            {
                if (!ModelState.IsValid) return View(fee);

                await _feeService.UpdateAsync(fee);

                TempData["msg"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }

            public async Task<IActionResult> Delete(int id)
            {
                var fee = await _feeService.GetByIdAsync(id);
                if (fee == null) return NotFound();

                return View(fee);
            }

            [HttpPost, ActionName("Delete")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _feeService.DeleteAsync(id);
                TempData["msg"] = "Đã xoá khoản phí.";
                return RedirectToAction("Index");
            }
        }
    }

