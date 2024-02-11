using API.DataTransferObjects;
using API.Entities;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PhotoRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> ApprovePhoto(int photoId)
        {
            var photo = await _context.Photos.FindAsync(photoId);
            if(photo==null)return false;
            photo.IsApproved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePhoto(int photoId)
        {
            var photo = await _context.Photos.FindAsync(photoId);
            if(photo==null)return false;
            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Photo> GetPhoto(int photoId)
        {
            return await _context.Photos.FirstOrDefaultAsync(x => x.Id == photoId);
        }

        public async Task<List<PhotoDTO>> GetUnapprovedPhotos()
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .Where(x => x.IsApproved == false)
                .ProjectTo<PhotoDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}