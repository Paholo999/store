using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace store.Services.AuthService
{
    public interface IAuthService
    {
        Task<(int, string)> Register(RegisterModel model, string role);
        Task<(int, string)> Login(LoginModel model);
        Task<List<RegisterModel>> GetAllUsers();
        Task<List<UserRolesViewModel>> ListUsersWithRoles();
    }
}