using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos.Admin;
using API.Dtos.Profile;
using API.Models;
using API.Models.Custom;

namespace API.Data.Interfaces
{
    public interface IProfileRepository
    {
        Task<PagedList<User>> GetAllUser(QueryParametersForGetAllUsers parameters);
        Task<List<User>> GetStaffs();
        Task<User> GetUser(int id);
        Task<User> RemoveUser(int id);
        Task<List<User>> GetUserList(List<int> userIds);
        Task<User> UpdateProfile(int id, UserForProfileUpdateDto data);
        Task<User> UpdateProfileImage(int id, string imageUrl);
        Task<User> ChangeRole(DataForChangingRoleDto dataForChangingRoleDto);
        Task<string> GetRole(int id);
    }
}