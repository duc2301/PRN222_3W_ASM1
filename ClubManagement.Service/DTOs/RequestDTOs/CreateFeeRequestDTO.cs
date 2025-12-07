using System;
using System.ComponentModel.DataAnnotations;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class CreateFeeRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn câu lạc bộ")]
        public int ClubId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự")]
        public string Title { get; set; } = null!;
        
        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn hạn nộp")]
        public DateOnly DueDate { get; set; }
        
        public string? Description { get; set; }
    }
}

