namespace AbbeyMortageAssessment.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using AbbeyMortageAssessment.Services.Profile;
    using AbbeyMortageAssessment.Services.User;
    using AbbeyMortageAssessment.Services.Friendship;
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;

        public ProfileController(
            IProfileService profileService,
            IUserService userService)
        {
            _profileService = profileService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(
            string userId,
            ServiceModelFriendshipStatus friendshipStatus)
        {
            var currentUserId = await _userService
                .GetUserIdByNameAsync(User.Identity.Name);

            ProfileServiceModel profile;

            if (userId != null)
            {
                profile = await _profileService.GetProfileAsync(userId);
            }
            else //Gets the current user`s profile
            {
                profile = await _profileService.GetProfileAsync(currentUserId);
            }

            profile.FriendshipStatus = friendshipStatus;

            profile.CurrentUserId = currentUserId;

            return View(profile);
        }
    }
}