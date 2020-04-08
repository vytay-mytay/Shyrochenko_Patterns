using Microsoft.AspNetCore.Http;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Validate and save image file
        /// </summary>
        /// <param name="image">image file</param>
        /// <returns>Model with image paths</returns>
        Task<ImageResponseModel> UploadOne(IFormFile image, ImageType type);

        /// <summary>
        /// Validate and save multiple images
        /// </summary>
        /// <param name="images">Images</param>
        /// <param name="type">Images type</param>
        /// <returns>Model with images and statuses</returns>
        Task<List<MultipleImagesResponseModel>> UploadMultipleSavingValid(List<IFormFile> images, ImageType type);

        /// <summary>
        /// Save multiple images only if all images are valid 
        /// </summary>
        /// <param name="images">Images</param>
        /// <param name="type">Images type</param>
        /// <returns>Models with image paths</returns>
        Task<List<ImageResponseModel>> UploadMultiple(List<IFormFile> images, ImageType type);

        /// <summary>
        /// Remove image
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Task RemoveImage(int imageId);
    }
}
