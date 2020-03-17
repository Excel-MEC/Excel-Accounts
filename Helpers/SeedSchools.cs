using System.Collections.Generic;
using System.IO;
using System.Linq;
using Excel_Accounts_Backend.Data;
using Excel_Accounts_Backend.Models;

namespace Excel_Accounts_Backend.Helpers
{
    public class RootObj
    {
        public List<School> School { get; set; }
    }
    public class SeedSchools
    {
        // Extract data from a json file ("Schoollist.json)
        // And populate the table - Schools
        public static void SeedData(DataContext context)
        {
            if (!context.Schools.Any())
            {
                string jsonStirng = File.ReadAllText("Schoollist.json");
                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObj>(jsonStirng);
                foreach (var item in root.School)
                {
                    var school = new School();
                    school.Id = item.Id;
                    school.Name = item.Name;
                    school.District = item.District;                    
                    context.Schools.Add(school);
                    context.SaveChanges();
                }
            }

        }    
    }
}