using System.Collections.Generic;
using System.IO;
using System.Linq;
using API.Data;
using API.Models;
using System.Text.Json;

namespace API.Helpers
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
                string jsonStirng = File.ReadAllText("./Assets/Collegelist.json");
                var root = JsonSerializer.Deserialize<RootObject>(jsonStirng);
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