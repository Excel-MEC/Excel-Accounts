using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Ambassador;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using API.Extensions.CustomExceptions;
using API.Models.Custom;

namespace API.Data
{
    public class AmbassadorRepository : IAmbassadorRepository
    {
        private readonly IProfileRepository _repo;
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IInstitutionRepository _institution;
        public AmbassadorRepository(IProfileRepository repo, IMapper mapper, DataContext context, IInstitutionRepository institution)
        {
            _context = context;
            _mapper = mapper;
            _repo = repo;
            _institution = institution;
        }

        public async Task<User> ApplyReferralCode(int id, int referralCode)
        {
            var user = await _context.Users.Include(user => user.Referrer).FirstOrDefaultAsync(user => user.Id == id);
            if (user.Referrer != null) throw new OneTimeUseException("A referral code has been already applied ");
            var ambassador = await _context.Ambassadors.Include(a => a.ReferredUsers).FirstOrDefaultAsync(a => a.Id == referralCode);
            if(ambassador == null) throw new CodeNotFoundException("This referral code doesn't exist ");
            user.Referrer = ambassador;
            ambassador.ReferredUsers.Add(user);
            ambassador.FreeMembership +=1;
            if(await _context.SaveChangesAsync() > 0) return user;
            throw new Exception("Problem saving changes");
        }

        public async  Task<AmbassadorProfileDto> GetAmbassador(int id)
        {
            var user = await _context.Users.Include(user => user.Ambassador).FirstOrDefaultAsync(user => user.Id == id);
            var ambassadorForView = _mapper.Map<AmbassadorProfileDto>(user);
            var institutionId = user.InstitutionId ?? default(int);
            ambassadorForView.InstitutionName = await _institution.FindName(user.Category, institutionId);
            ambassadorForView.AmbassadorId = user.Ambassador.Id;            
            ambassadorForView.FreeMembership = user.Ambassador.FreeMembership;
            ambassadorForView.PaidMembership = user.Ambassador.PaidMembership;
            return ambassadorForView;
        }

        public async Task<List<AmbassadorListViewDto>> ListOfAmbassadors()
        {
            var ambassadors = await _context.Ambassadors.Include(a => a.User)
                                                                    .ToListAsync();
            var newlist = new List<AmbassadorListViewDto>();
            foreach (var ambassador in ambassadors)
            {
                var user = _mapper.Map<AmbassadorListViewDto>(ambassador.User);
                user.AmbassadorId = ambassador.Id;
                user.FreeMembership = ambassador.FreeMembership;
                user.PaidMembership = ambassador.PaidMembership;
                newlist.Add(user);
            }
            return newlist;
        }

        public async Task<List<UserViewDto>> ListOfReferredUsers(int id)
        {
            User user = await _context.Users.Include(user => user.Ambassador)
                                            .ThenInclude(a => a.ReferredUsers)
                                            .FirstOrDefaultAsync(user => user.Id == id);
            var referredUsers = user.Ambassador.ReferredUsers;
            List<UserViewDto> newlist = new List<UserViewDto>();
            foreach (var referredUser in referredUsers)
            {
                var referredUserView = _mapper.Map<UserViewDto>(referredUser);
                newlist.Add(referredUserView);
            }
            return newlist;   
        }

        public async Task<Ambassador> SignUpForAmbassador(int id)
        {
            var user = await _context.Users.Include(user => user.Ambassador)
                                            .FirstOrDefaultAsync(user => user.Id == id);
            if(user.Ambassador != null)  throw new OneTimeUseException(" This email address is already registered ");                       
            var ambassador = new Ambassador();
            user.Ambassador = ambassador;
            await _context.Ambassadors.AddAsync(ambassador);
            if(await _context.SaveChangesAsync() > 0) return ambassador;
            throw new Exception("Problem in saving changes");
        }
    }
}