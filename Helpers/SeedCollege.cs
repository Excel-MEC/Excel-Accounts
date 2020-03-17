using System.Collections.Generic;
using System.IO;
using System.Linq;
using Excel_Accounts_Backend.Data;
using Excel_Accounts_Backend.Models;

namespace Excel_Accounts_Backend.Helpers
{
    public class RootObject
    {
        public List<College> College { get; set; }
    }
    public class SeedCollege
    {
        // Extract data from a json file ("Collegelist.json)
        // And populate the table - Colleges
        public static void SeedData(DataContext context)
        {
            if (!context.Colleges.Any())
            {
                string jsonStirng = File.ReadAllText("Collegelist.json");
                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(jsonStirng);
                foreach (var item in root.College)
                {
                    var college = new College();
                    college.Id = item.Id;
                    college.Name = item.Name;                    
                    context.Colleges.Add(college);
                    context.SaveChanges();
                }
            }

        }
    }

}