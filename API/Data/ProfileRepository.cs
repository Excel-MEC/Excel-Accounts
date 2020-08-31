using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos.Profile;
using API.Models;
using API.Data.Interfaces;
using API.Dtos.Admin;
using API.Extensions.CustomExceptions;
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

        public async Task<PagedList<User>> GetAllUser(QueryParametersForGetAllUsers parameters)
        {
            IQueryable<User> query = _context.Users;
            if (parameters.Id != null) query = query.Where(user => user.Id == parameters.Id);
            if(parameters.Name != null) query = query.Where(user => user.Name.ToLower().Contains(parameters.Name.ToLower()));
            if(parameters.Email != null)  query = query.Where(user => user.Email.ToLower().Contains(parameters.Email.ToLower()));
            if(parameters.Gender != null)  query = query.Where(user => user.Gender.ToLower() == parameters.Gender.ToLower());
            if (parameters.InstitutionId != null) query = query.Where(user => user.Id == parameters.Id);
            if(parameters.CategoryId != null)  query = query.Where(user => user.CategoryId == parameters.CategoryId);
            if(parameters.Role != null)  query = query.Where(user => user.Role.ToLower().Contains(parameters.Role.ToLower()));
            if(parameters.MobileNumber != null)  query = query.Where(user => user.MobileNumber.Contains(parameters.MobileNumber));
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

            var users = await PagedList<User>.ToPagedList(query, parameters.PageNumber, parameters.PageSize);
            return users;
        }

        public async Task<User> GetUser(int userid)
        {
            return await _context.Users
                .Include(user => user.Ambassador)
                .Include(user => user.Referrer)
                .FirstOrDefaultAsync(user => user.Id == userid);
        }

        public async Task<List<User>> GetStaffs()
        {
            IQueryable<User> query = _context.Users;
            query = query.Where(
                user => user.Role.Contains(Roles.Admin) 
                        || user.Role.Contains(Roles.Core)
                        || user.Role.Contains(Roles.Editor)
                        || user.Role.Contains(Roles.Staff)
                );
            return await query.ToListAsync();
        }

        public async Task<User> RemoveUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null) throw new DataInvalidException("The user does not exist. Please re-check the ID");
            _context.Remove(user);
            if(await _context.SaveChangesAsync() > 0) return user;
            throw new Exception("Problem in saving changes");
        }

        public async Task<List<User>> GetUserList(List<int> userIds)
        {
            return await _context.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();
        }
        public async Task<User> UpdateProfile(int id, UserForProfileUpdateDto data)
        {            
            var user = await _context.Users.FindAsync(id);
            user.Name = data.Name ?? user.Name;
            user.Gender = data.Gender ?? user.Gender;
            user.MobileNumber = data.MobileNumber ?? user.MobileNumber;
            var categoryId = data.CategoryId ?? user.CategoryId.ToString();
            var institutionId = data.InstitutionId ?? user.InstitutionId;
            user.CategoryId = int.Parse(categoryId);
            if (categoryId == "2")
            {
                user.InstitutionId = null;
                if(await _context.SaveChangesAsync() > 0) return user;
                throw new DataInvalidException("No changes to update. Please re-check the details");
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
            if(await _context.SaveChangesAsync() > 0) return user;
            throw new DataInvalidException("No changes to update. Please re-check the details");
        }


        public async Task<User> UpdateProfileImage(int id, string imageUrl)
        {
            var user = await _context.Users.FindAsync(id);
            if (user.Picture.Equals(imageUrl))
            {
                return user;
            }

            user.Picture = imageUrl;
            if(await _context.SaveChangesAsync() > 0) return user;
            throw new DataInvalidException("No changes to update.");
        }

        public async Task<User> ChangeRole(DataForChangingRoleDto dataForChangingRoleDto)
        {
            var user = await _context.Users.FindAsync(dataForChangingRoleDto.Id);
            user.Role = dataForChangingRoleDto.Role;
            if(await _context.SaveChangesAsync() > 0) return user;
            throw new Exception("Problem in saving changes");
        }

        public async Task<string> GetRole(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user.Role;
        }

        public async Task<User> UpdatePaymentStatus(DataForChangingPaymentStatusDto data)
        {
            var user = await _context.Users.FindAsync(data.Id);
            user.IsPaid = data.IsPaid;
            await _context.SaveChangesAsync();
            return user;
        }
    }
}