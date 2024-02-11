using API.DataTransferObjects;
using API.Entities;

namespace API.Interface
{
    public interface IPhotoRepository
    {
        public Task<List<PhotoDTO>> GetUnapprovedPhotos();
        public Task<Photo> GetPhoto(int photoId);
        public Task<bool> ApprovePhoto(int photoId);
        public Task<bool> DeletePhoto(int photoId);
    }
}