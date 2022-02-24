using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Platform
    {
        public int PlatformId { get; set; }
        public string PlatformName { get; set; }
        public ICollection<Product> Products { get; set; }
        public Platform()
        {
            Products = new List<Product>();
        }
        public static string Name(Platforms role)
        {
            return Enum.GetName(typeof(Platforms), role);
        }
    }

    public enum Platforms
    {
        Playstation,
        Windows,
        XBox,
        IMac,
        Linux,
        Nintendo
    }
}
