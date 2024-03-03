namespace AbbeyMortageAssessment.Services.Profile
{
    using AbbeyMortageAssessment.Services.Image;
    using AbbeyMortageAssessment.Services.Post;
    using AbbeyMortageAssessment.Services.User;
    using System.Threading.Tasks;

    public class ProfileService : IProfileService
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public ProfileService(
            IPostService postService,
            IUserService userService,
            IImageService imageService)
        {
            _postService = postService;
            _userService = userService;
            _imageService = imageService;
        }

        public async Task<ProfileServiceModel> GetProfileAsync(string userId)
            => new ProfileServiceModel()
            {
                User = await _userService
                    .GetUserByIdAsync(userId),
                Posts = await _postService
                    .GetPostsByUserIdAsync(userId),
                AvatarUrl = await _imageService
                    .GetAvatarAsync(userId)
            };
    }
}
