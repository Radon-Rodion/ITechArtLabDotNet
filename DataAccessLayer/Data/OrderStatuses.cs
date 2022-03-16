using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
    public enum OrderStatuses
    {
        Active,
        Bought,
        Deleted
    }
    public class OrderStatus
    {
        public static string Name(OrderStatuses status)
        {
            return Enum.GetName(typeof(OrderStatuses), status);
        }
    }
}
