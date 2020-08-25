using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Models;
using API.Data.Interfaces;
using API.Dtos.Admin;
using API.Extensions;
using API.Models.Custom;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly DataContext _context;
        private readonly IInstitutionRepository _institution;
        private readonly IMapper _mapper;

        public ProfileRepository(DataContext context, IInstitutionRepository institution, IMapper mapper)
        {
            _mapper = mapper;
            _institution = institution;
            _context = context;
        }

        public PagedList<User> GetAllUser(QueryParameters parameters)
        {
            var query = from s in _context.Users select s;
            if (parameters.Id != null) query = query.Where(user => user.Id == parameters.Id);
            if(parameters.Name != null) query = query.Where(user => user.Name.ToLower()==parameters.Name.ToLower());
            if(parameters.Email != null)  query = query.Where(user => user.Email.ToLower() == parameters.Email.ToLower());
            if(parameters.Gender != null)  query = query.Where(user => user.Gender.ToLower() == parameters.Gender.ToLower());
            if (parameters.InstitutionId != null) query = query.Where(user => user.Id == parameters.Id);
            if(parameters.CategoryId != null)  query = query.Where(user => user.CategoryId == parameters.CategoryId);
            if(parameters.Role != null)  query = query.Where(user => user.Role.ToLower().Contains(parameters.Role.ToLower()));
            if(parameters.MobileNumber != null)  query = query.Where(user => user.MobileNumber == parameters.MobileNumber);
            if(parameters.IsPaid != null)  query = query.Where(user => user.IsPaid == parameters.IsPaid);
            if(parameters.ReferrerAmbassadorId != null)  query = query.Where(user => user.ReferrerAmbassadorId == parameters.ReferrerAmbassadorId);
            switch (parameters.SortOrder)
            {
                case "desc":
                    query = query.OrderByDescending(on => on.Name);
                    break;
                default:
                    query = query.OrderBy(on => on.Name);
                    break;
            }
            var users =  PagedList<User>.ToPagedList( query, parameters.PageNumber, parameters.PageSize);
            return users;
        }

        public async Task<User> GetUser(int userid)
        {
            return await _context.Users
            .Include(user => user.Ambassador)
            .Include(user => user.Referrer)
            .FirstOrDefaultAsync(user => user.Id == userid);
        }

        public async Task<List<User>> GetUserList(List<int> userIds)
        {
            return await _context.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();
        }

        public async Task<bool> UpdateProfile(int id, UserForProfileUpdateDto data)
        {            
            User user = await _context.Users.FindAsync(id);
            user.Name = data.Name ?? user.Name;
            user.Gender = data.Gender ?? user.Gender;
            user.MobileNumber = data.MobileNumber ?? user.MobileNumber;            
            var categoryId = data.CategoryId ?? user.CategoryId.ToString();
            var institutionId = data.InstitutionId ?? user.InstitutionId;
            user.CategoryId = int.Parse(categoryId);            
            if(categoryId == "2")
            {
                user.InstitutionId = null;
                return await _context.SaveChangesAsync() > 0;
            }
            if (institutionId == 0) //Adds new college or school
            {
                switch (user.Category)
                {
                    case "college":
                    {
                        var college = await _institution.AddCollege(data.InstitutionName);
                        user.InstitutionId = college.Id;
                        break;
                    }
                    case "school":
                    {
                        var school = await _institution.AddSchool(data.InstitutionName);
                        user.InstitutionId = school.Id;
                        break;
                    }
                }
            }
            else 
            {
                user.InstitutionId = institutionId;               
            }
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> UpdateProfileImage(int id, string imageUrl)
        {
            var user = await _context.Users.FindAsync(id);
            if (user.Picture.Equals(imageUrl))
            {
                return true;
            }
            user.Picture = imageUrl;
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

        public async Task<bool> ChangeRole(DataForChangingRoleDto dataForChangingRoleDto)
        {
            var user = await _context.Users.FindAsync(dataForChangingRoleDto.Id);
            user.Role = dataForChangingRoleDto.Role;
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

        public async Task<string> GetRole(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user.Role;
        }
    }
}