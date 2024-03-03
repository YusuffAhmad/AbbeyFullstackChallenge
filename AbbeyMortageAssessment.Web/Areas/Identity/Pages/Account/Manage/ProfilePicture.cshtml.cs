namespace AbbeyMortageAssessment.Web.Areas.Identity.Pages.Account.Manage
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using AbbeyMortageAssessment.Data.Models;
    using AbbeyMortageAssessment.Services.Image;
    using AbbeyMortageAssessment.Services.Stream;
    using System.Linq;
    using System.Threading.Tasks;

    public class ProfilePictureModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IStreamService _streamService;
        private readonly IImageService _imageService;

        public ProfilePictureModel(
            UserManager<User> userManager,
            IStreamService streamService,
            IImageService imageService)
        {
            _userManager = userManager;
            _imageService = imageService;
            _streamService = streamService;
        }

        public string StatusMessage { get; set; }

        public async Task<ActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            var avatar = await _imageService
                .GetAvatarAsync(userId);

            if (avatar != null)
            {
                ViewData["Avatar"] = avatar;
            }

            return Page();
        }

        public async Task<ActionResult> OnPostAsync(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager
                .GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //Deletes the avatar if there is so
            await _imageService.DeleteAvatarAsync(user.Id);

            if (file == null)
            {
                return NotFound();
            }

            var memoryStream = await _streamService
                .CopyFileToMemoryStreamAsync(file);

            await _imageService.AddAvatarAsync(new AvatarServiceModel
            {
                AvatarData = memoryStream.ToArray(),
                UploaderId = user.Id,
            });

            StatusMessage = "Profile picture has been set successfully.";
            return RedirectToPage();
        }
    }
}
