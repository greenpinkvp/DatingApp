using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;
using DatingApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Repositories
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

        public async Task<Photo> GetPhotoById(Guid id)
        {
            return await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<PhotoForApprovalDto>> GetUnapprovalPhotos(PaginationParams paginationParams)
        {
            var photos = _context.Photos.IgnoreQueryFilters()
                .Where(x => x.IsApproved == false)
                .Select(u => new PhotoForApprovalDto
                {
                    Id = u.Id,
                    UserName = u.AppUser.UserName,
                    Url = u.Url,
                    IsApproved = u.IsApproved
                }).AsQueryable();

            return await PagedList<PhotoForApprovalDto>
                .CreateAsync(photos, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }
    }
}