using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Entities
{
    [Index("UserId")]
    public class Order
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public DateTime AddingDate { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Status { get; set; }
    }
}
