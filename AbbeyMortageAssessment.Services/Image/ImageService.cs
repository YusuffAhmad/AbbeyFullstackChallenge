namespace AbbeyMortageAssessment.Services.Image
{
    using Microsoft.EntityFrameworkCore;
    using AbbeyMortageAssessment.Data;
    using AbbeyMortageAssessment.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext _data;

        public ImageService(ApplicationDbContext context) => _data = context;

        public async Task AddImageAsync(ImageServiceModel serviceModel)
        {
            if (serviceModel == null)
                throw new ArgumentException("Image cannot be null");

            var img = new Image
            {
                ImageTitle = serviceModel.ImageTitle,
                ImageData = serviceModel.ImageData,
                UploaderId = serviceModel.UploaderId
            };

            _data.Images.Add(img);
            await _data.SaveChangesAsync();
        }

        public async Task DeleteAvatarAsync(string userId)
        {
            if (IsThereAvatar(userId))
            {
                var avatar = await GetAvatarEntityAsync(userId);

                _data.ProfilePictures.Remove(avatar);
                await _data.SaveChangesAsync();
            }
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await GetImageEntityAsync(imageId);

            _data.Images.Remove(image);
            await _data.SaveChangesAsync();
        }

        public bool IsThereAvatar(string userId)
        => _data.ProfilePictures
            .Any(u => u.UploaderId == userId);

        public async Task<bool> IsImageExistAsync(int imageId)
        => await _data.Images
            .AnyAsync(i => i.Id == imageId);

        public async Task<string> GetAvatarAsync(string userId)
        {
            if (IsThereAvatar(userId))
            {
                var avatar = await GetAvatarEntityAsync(userId);

                return GetImageDataUrl(avatar.AvatarData);
            }
            return null;
        }

        private async Task<Avatar> GetAvatarEntityAsync(string userId)
        => await _data.ProfilePictures
            .FirstOrDefaultAsync(u => u.UploaderId == userId);

        public IEnumerable<KeyValuePair<int, string>> GetAllImagesByUserId(string userId)
        {
            var imageEntities = GetAllImagesEntitiesByUserId(userId);

            if (imageEntities.Count > 0)
            {
                var images = new List<KeyValuePair<int, string>>();

                foreach (var image in imageEntities)
                {
                    images.Add(
                        new KeyValuePair<int, string>(
                            image.Id,
                            GetImageDataUrl(image.ImageData)));
                }
                return images;
            }
            return null;
        }

        public async Task AddAvatarAsync(AvatarServiceModel avatarServiceModel)
        {
            _data.ProfilePictures.Add(new Avatar
            {
                AvatarData = avatarServiceModel.AvatarData,
                UploaderId = avatarServiceModel.UploaderId
            });

            await _data.SaveChangesAsync();
        }

        public async Task<string> GetImageByIdAsync(int imageId)
        {
            var imageEntity = await _data.Images
                .FirstOrDefaultAsync(i => i.Id == imageId);

            return this.GetImageDataUrl(imageEntity.ImageData);
        }

        private ICollection<Image> GetAllImagesEntitiesByUserId(string userId)
         => _data.Images
            .Where(i => i.UploaderId == userId)
            .ToList();

        private string GetImageDataUrl(byte[] imageData)
        {
            var imageBase64 = Convert.ToBase64String(imageData);

            return string.Format($"data:image/jpg;base64,{imageBase64}");
        }

        private async Task<Image> GetImageEntityAsync(int imageId)
        => await _data.Images
            .FirstOrDefaultAsync(i => i.Id == imageId);

    }
}
