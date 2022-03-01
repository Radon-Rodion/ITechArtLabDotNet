using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
    public enum SortFields
    {
        Price,
        Rating
    }
    public class SortField
    {
        public static string Name(SortFields field)
        {
            return Enum.GetName(typeof(SortFields), field);
        }
    }
}
