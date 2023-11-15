namespace FREEFOODSERVER.Services
{
    public interface IImageService
    {
        Task<Guid?> LoadImageAsync(byte[] bytes);

        Task<byte[]?> GetImageAsync(Guid imageId);

        Task<bool> RemoveImageAsync(Guid imageId);
    }
}
