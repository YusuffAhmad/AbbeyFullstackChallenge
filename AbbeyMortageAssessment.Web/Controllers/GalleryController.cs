namespace AbbeyMortageAssessment.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using AbbeyMortageAssessment.Data.Models;
    using AbbeyMortageAssessment.Services.Image;
    using AbbeyMortageAssessment.Services.Stream;
    using AbbeyMortageAssessment.Services.User;
    using AbbeyMortageAssessment.Web.Models;
    using System.Threading.Tasks;

    public class GalleryController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IStreamService _streamService;
        private readonly IUserService _userService;

        public GalleryController(
            IImageService imageService,
            IStreamService streamService,
            IUserService userService)
        {
            _imageService = imageService;
            _streamService = streamService;
            _userService = userService;
        }

        public async Task<IActionResult> IndexAsync(string userId)
        {
            var user = await _userService
                .GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var images = _imageService
                .GetAllImagesByUserId(userId);

            return View(images);
        }

        [HttpGet]
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(IFormCollection uploadedFiles)
        {
            var userId = await _userService
                .GetUserIdByNameAsync(User.Identity.Name);

            var images = uploadedFiles.Files;

            if (images == null)
            {
                return RedirectToAction(
                    nameof(IndexAsync),
                    new { userId });
            }

            foreach (var image in images)
            {
                var memoryStream = await _streamService
                    .CopyFileToMemoryStreamAsync(image);

                await _imageService.AddImageAsync(new ImageServiceModel()
                {
                    ImageTitle = image.FileName,
                    ImageData = memoryStream.ToArray(),
                    UploaderId = userId
                });
            }

            return RedirectToAction(
                "Index",
                new { userId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImageAsync(int imageId)
        {
            if (!await _imageService.IsImageExistAsync(imageId))
            {
                return NotFound($"Image cannot be found!");
            }
            return View(new ImageViewModel
            {
                ImageId = imageId,
                Base64Image = await _imageService
                    .GetImageByIdAsync(imageId)
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int imageId)
        {
            var userId = await _userService
                .GetUserIdByNameAsync(User.Identity.Name);

            await _imageService.DeleteImageAsync(imageId);

            return RedirectToAction(
                "Index",
                new { userId });
        }
    }
}