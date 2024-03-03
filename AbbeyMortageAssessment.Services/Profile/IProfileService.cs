namespace AbbeyMortageAssessment.Services.Profile
{
    using AbbeyMortageAssessment.Services.Common;
    using System.Threading.Tasks;

    public interface IProfileService : IService
    {
        Task<ProfileServiceModel> GetProfileAsync(string userId);
    }
}
