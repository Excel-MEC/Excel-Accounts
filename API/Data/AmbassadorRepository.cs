using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Ambassador;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using API.Extensions.CustomExceptions;

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
            this._context = context;
            this._mapper = mapper;
            this._repo = repo;
            this._institution = institution;
        }

        public async Task<bool> ApplyReferralCode(int id, int referralCode)
        {
            var user = await _context.Users.Include(user => user.Referrer).FirstOrDefaultAsync(user => user.Id == id);
            if (user.Referrer != null) throw new OneTimeUseException("A referral code has been already applied ");
            var ambassador = await _context.Ambassadors.Include(a => a.ReferredUsers).FirstOrDefaultAsync(a => a.Id == referralCode);
            if(ambassador == null) throw new CodeNotFoundException("This referral code doesn't exist ");
            user.Referrer = ambassador;
            ambassador.ReferredUsers.Add(user);
            ambassador.FreeMembership +=1;
            var success = await _context.SaveChangesAsync() > 0;
            return success;
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
            List<Ambassador> ambassadors = await _context.Ambassadors.Include(a => a.User)
                                                                    .ToListAsync();
            List<AmbassadorListViewDto> newlist = new List<AmbassadorListViewDto>();
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

        public async Task<bool> SignUpForAmbassador(int id)
        {
            User user = await _context.Users.Include(user => user.Ambassador)
                                            .FirstOrDefaultAsync(user => user.Id == id);
            if(user.Ambassador != null)  throw new OneTimeUseException(" This email address is already registered ");                       
            Ambassador ambassador = new Ambassador();
            user.Ambassador = ambassador;
            await _context.Ambassadors.AddAsync(ambassador);
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }
    }
}