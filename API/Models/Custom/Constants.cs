using System.Collections.Generic;

namespace API.Models.Custom
{
    public static class Constants
    {
        public static string[] Roles => new string[] {
            "User",
            "Admin",
            "Volunteers",
            "Core",
            "Staff"
        };

        public static string[] Category = {
            "college",
            "school",            
            "others"
        };
    }
}