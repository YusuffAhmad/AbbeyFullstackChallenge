namespace AbbeyMortageAssessment.Services.User
{
    using AbbeyMortageAssessment.Services.Common;
    using System.Threading.Tasks;

    public interface IUserService : IService
    {
        Task<UserServiceModel> GetUserByIdAsync(string userId);

        Task<string> GetUserIdByNameAsync(string name);

        Task<UserServiceModel> GetUserByNameAsync(string name);
    }
}
