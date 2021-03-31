using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
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
            _mapper = mapper;
            _context = context;
        }

        public async Task<Photo> GetPhotoById(int photoId)
        {
            return await _context.Photos.IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == photoId);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
           return await _context.Photos
           .Where(p => p.IsApproved == false)
           .IgnoreQueryFilters()
           .ProjectTo<PhotoForApprovalDto>(_mapper.ConfigurationProvider)
           .ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
           _context.Photos.Remove(photo);   
        }

        public async Task<int> GetMainPhotoByUserId(int userId)
        {
            var photos = await _context.Photos
            .Where(photo => photo.IsMain == true && photo.AppUserId == userId)
            .ToListAsync();

            return photos.Count();
        }
    }
}