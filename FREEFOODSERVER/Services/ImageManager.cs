using FREEFOODSERVER.Data;
using FREEFOODSERVER.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FREEFOODSERVER.Services
{
    public class ImageManager
    {
        private readonly IMemoryCache _cache;
        private readonly ImageDbContext _db;
        private readonly int _baseWidth = 300;
        private readonly int _baseHeigth = 300;

        public class AddImage
        {
            public Image Image { get; set; }

            public string Name { get; set; }

        }

        public class EditImage
        {
            public string Image { get; set; }

            public Guid Id { get; set; }

        }

        public ImageManager(ImageDbContext db, IMemoryCache memoryCache)
        {
            _db = db;
            _cache = memoryCache;
        }

        public async Task<Guid?> FindImageGuid(string imageName)
        {
            var image = await _db.Images.AsNoTracking().Where(x => x.Name == imageName).FirstOrDefaultAsync();
            return image?.Id ?? null;
        }

        public async Task<bool> IsContainImageAsync(Guid imageId)
        {
            return await _db.Images.AsNoTracking().ContainsAsync<ImageDb>(new ImageDb() { Id = imageId });
        }

        public async Task<List<Guid>> IsContainImageRangeAsync(List<Guid> imageIds)
        {
            var query = _db.Images.AsNoTracking();
            foreach (var item in imageIds)
            {
                query = query.Where(x => x.Id == item);
            }
            return (await query.ToListAsync()).Select(x => x.Id).ToList();
        }

        public async Task<string?> GetStringImageAsync(string imageName)
        {
            var imageinDb = await _db.Images.Include(x => x.Data).AsNoTracking()
                .Where(x => x.Name == imageName)
                .FirstOrDefaultAsync();

            if (imageinDb == null || imageinDb.Data == null) return null;
            return imageinDb.Data.Img;
        }

        public async Task<string?> GetStringImageAsync(Guid imageId)
        {
            var imageinDb = await _db.Images.Include(x => x.Data).AsNoTracking().Where(x => x.Id == imageId).FirstOrDefaultAsync();

            if (imageinDb == null || imageinDb.Data == null) return null;
            return imageinDb.Data.Img;
        }

        public async Task<Guid?> SaveImageAsync(AddImage newImage)
        {
            var image = ValidateImage(newImage.Image);
            if (image.Width >= _baseWidth || image.Height >= _baseHeigth)
                image.Mutate(x => x.Resize(
                    new ResizeOptions()
                    {
                        Mode = ResizeMode.Crop,
                        Size = new Size(_baseWidth, _baseHeigth)
                    }));

            byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                await image.SaveAsJpegAsync(memoryStream);
                data = memoryStream.ToArray();
            }

            var name = newImage.Name + Guid.NewGuid();

            await _db.Images.AddAsync(new ImageDb()
            {
                Data = new() { Img = Convert.ToBase64String(data) },
                Name = name,
            });

            await _db.SaveChangesAsync().ConfigureAwait(false);
            return (await _db.Images.LastAsync().ConfigureAwait(false)).Id;
        }

        /// <summary>
        /// Сохранение списка изображений в бд
        /// </summary>
        /// <param name="addImages"></param>
        /// <returns></returns>
        public async Task<List<Guid>> SaveImageRangeAsync(IEnumerable<AddImage> addImages)
        {
            List<string> Names = new List<string>(addImages.Count());
            {
                var query1 = _db.Images;

                foreach (var item in addImages)
                {
                    Names.Add(item.Name + "_" + Guid.NewGuid());
                    query1.Add(new ImageDb()
                    {
                        Data = new() { Img = await SetImageToBase64StringAsync(item.Image) },
                        Name = Names.Last(),
                    });
                }

                await _db.SaveChangesAsync().ConfigureAwait(false);

                var result = new List<Guid>();
            }

            var query2 = _db.Images.AsQueryable();

            foreach (var item in Names)
            {
                query2 = query2.Where(x => x.Name == item);
            }

            return await query2.Select(x => x.Id).ToListAsync();
        }

        public class ImageEditException : Exception
        {
            static readonly string _exceptionMessage = "Image Edit Error";
            public ImageEditException(string? message) :
                base(_exceptionMessage + message)
            { }
        }

        public async Task<Guid> EditImageAsync(EditImage value)
        {
            var image = await _db.Images.Include(x => x.Data).Where(x => x.Id == value.Id).FirstOrDefaultAsync();

            if (image == null || image.Data == null) throw new ImageEditException("Image Not Exist");
            image.Data.Img = await ValidateImageAsync(value.Image);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return image.Id;
        }

        public async Task<List<Guid>> EditImageRangeAsync(List<EditImage> value)
        {
            List<Guid> id = new List<Guid>(value.Count);
            var query = _db.Images.Include(x => x.Data).AsQueryable();
            foreach (var item in value)
            {
                query = query.Where(x => x.Id == item.Id);
            }

            var image = await query.ToListAsync();

            image.ForEach(async x =>
                {
                    var image = value.Find(y => y.Id == x.Id);
                    id.Add(image.Id);
                    x.Data.Img = await ValidateImageAsync(image.Image);
                }
            );

            return id;

        }


        public async Task<bool> RemoveImageAsync(Guid imageName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SetImageToBase64StringAsync(Image image)
        {
            var validated = ValidateImage(image);

            using (var memoryStream = new MemoryStream())
            {
                await validated.SaveAsJpegAsync(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public async Task<Image> GetImageFromStringAsync(string image)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(Convert.FromBase64String(image));
                return await Image.LoadAsync(memoryStream);
            }
        }

        public Image GetImageFromString(string image)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(Convert.FromBase64String(image));
                return Image.Load(memoryStream);
            }
        }

        public Image ValidateImage(Image image)
        {
            if (image.Width >= _baseWidth || image.Height >= _baseHeigth)
                image.Mutate(x => x.Resize(
                    new ResizeOptions()
                    {
                        Mode = ResizeMode.Crop,
                        Size = new Size(_baseWidth, _baseHeigth)
                    }));
            return image;
        }

        public async Task<string> ValidateImageAsync(string imageStr)
        {
            using (var memoryStream = new MemoryStream())
            {
                await memoryStream.WriteAsync(Convert.FromBase64String(imageStr));
                var validatedImage = ValidateImage(await Image.LoadAsync(memoryStream));
                memoryStream.SetLength(0);
                await validatedImage.SaveAsJpegAsync(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

    }
}
