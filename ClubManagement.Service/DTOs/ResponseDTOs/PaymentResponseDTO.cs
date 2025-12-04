using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.ResponseDTOs
{
    public class PaymentResponseDTO
    {
        public int PaymentId { get; set; }
        public string FeeTitle { get; set; }
        public string ClubName { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; }
    }
}
