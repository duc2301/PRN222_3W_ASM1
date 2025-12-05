using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class CreatePaymentRequestDTO
    {
        public int FeeId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }

    public class ValidatePaymentDTO
    {
        public int UserId { get; set; }
        public int FeeId { get; set; }
        public decimal Amount { get; set; }
    }
}
