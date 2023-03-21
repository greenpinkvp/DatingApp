using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Helpers;

namespace DatingApp.API.Repositories.Interfaces
{
    public interface IPhotoRepository
    {
        Task<PagedList<PhotoForApprovalDto>> GetUnapprovalPhotos(PaginationParams paginationParams);

        Task<Photo> GetPhotoById(Guid id);

        void RemovePhoto(Photo photo);
    }
}