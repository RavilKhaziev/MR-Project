using FREEFOODSERVER.Models.Users;
using FREEFOODSERVER.Models.ViewModel.Company;
using FREEFOODSERVER.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FREEFOODSERVER.Controllers
{

    [Route("api/Image")]
    [ApiController]
    public class ImageController : Controller
    {

        private readonly ImageManager _imageManager;

        public ImageController(ImageManager imageManager)
        {
            _imageManager = imageManager;
        }


        /// <summary>
        /// Возвращает фотографию
        /// </summary>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> POSTImage([FromBody]Guid imageId)
        {
            var result = await _imageManager.GetStringImageAsync(imageId);
            return result == null ? NotFound("Image Not Exist") : Ok((string)result);
        }
    }
}
