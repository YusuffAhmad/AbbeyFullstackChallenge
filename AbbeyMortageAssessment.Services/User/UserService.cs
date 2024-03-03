namespace AbbeyMortageAssessment.Services.User
{
    using Microsoft.AspNetCore.Identity;
    using AbbeyMortageAssessment.Data.Models;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager) => _userManager = userManager;

        public async Task<UserServiceModel> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return new UserServiceModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Country = user.Country,
                DateOfBirth = user.DOB
            };
        }

        public async Task<string> GetUserIdByNameAsync(string name)
        {
            var user = await _userManager
                .FindByNameAsync(name);

            return user.Id;
        }

        public async Task<UserServiceModel> GetUserByNameAsync(string name)
        {
            var user = await _userManager
                .FindByNameAsync(name);

            return new UserServiceModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Country = user.Country,
                DateOfBirth = user.DOB
            };
        }
    }
}
