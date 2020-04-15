using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using API.Data;
using API.Models;

namespace Tests.Helpers
{
    public class RootObject
    {
        public List<College> Colleges { get; set; }
        public List<School> Schools { get; set; }
    }
    public static class SeedData
    {
        public static void Seed(DataContext context)
        {

            string jsonStirngInstitution = File.ReadAllText("./Assets/Institutions.json");
            var root = JsonSerializer.Deserialize<RootObject>(jsonStirngInstitution);
            foreach (var item in root.Colleges)
            {
                var college = new College();
                college.Name = item.Name;
                context.Colleges.Add(college);
            }
            foreach (var item in root.Schools)
            {
                var school = new School();
                school.Name = item.Name;
                context.Schools.Add(school);
            }

            string jsonStirngUser = File.ReadAllText("./Assets/Users.json");
            var users = JsonSerializer.Deserialize<List<User>>(jsonStirngUser);
            foreach (var user in users)
            {
                context.Users.Add(user);
            }
            context.SaveChanges();
        }
    }
}