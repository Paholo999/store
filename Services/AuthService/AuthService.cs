using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace store.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }
        
        

        public async Task<List<RegisterModel>> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            return users.Select(user => new RegisterModel
            {
                Username = user.UserName,
                Email = user.Email,
                Password = user.PasswordHash,
                FirstName = user.FirstName,
                LastName = user.LastName
            }).ToList();
        }

        public async Task<List<UserRolesViewModel>> ListUsersWithRoles()
        {
            var users = userManager.Users.ToList(); // Obtiene todos los usuarios

            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user); // Obtiene los roles de cada usuario
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    Username = user.UserName,
                    Roles = string.Join(", ", roles)
                });
            }

            return userRolesViewModel;
           
        } 

        public async Task<(int,string)> Register(RegisterModel model,string role)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return (0, "User already exists");

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var createUserResult = await userManager.CreateAsync(user, model.Password);
            if (!createUserResult.Succeeded){
                var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                return (0, $"User creation failed! Errors: {errors}");
            }
               
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

            if (await roleManager.RoleExistsAsync(role))
                await userManager.AddToRoleAsync(user, role);

            return (1,"User created successfully!");
        }

        public async Task<(int,string)> Login(LoginModel model)
        {
            try{
                var user = await userManager.FindByNameAsync(model.Username);
                if (user == null)
                    return (0, "Invalid username");
                if (!await userManager.CheckPasswordAsync(user, model.Password))
                    return (0, "Invalid password " + model.Password);
            
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                string token = GenerateToken(authClaims);
                return (1, token);
            }
            catch (Exception ex)
            {
        

                // Return a generic error message to the client
                return (0, "An error occurred during login");
            }

            
            
        }


        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                //Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}