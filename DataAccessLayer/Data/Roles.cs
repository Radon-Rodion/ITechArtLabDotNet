using System;

namespace DataAccessLayer.Data
{
    public enum Roles
    {
        User,
        Admin
    }

    public class Role
    {
        public static string Name(Roles role)
        {
            return Enum.GetName(typeof(Roles), role);
        }
    }
}