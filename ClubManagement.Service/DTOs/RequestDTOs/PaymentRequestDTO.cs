using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagement.Service.DTOs.RequestDTOs
{
    public class PaymentRequestDTO
    {
        public int FeeId { get; set; }
        public decimal Amount { get; set; }
    }

}
