using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using Microsoft.AspNetCore.Http;

namespace MatchMaker.Service.Abstracts
{
    public interface IUserService
    {
        Task<PaginatedResponse<MemberDto>> GetAllUsersAsync(UserParams userParams, string currentUsername);
        
        Task<AppUser> GetUserByEmailAsync(string email);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<AppUser> GetUserByIdAsync(int id);
        
        Task<string> UpdateUserAsync(AppUser user);
        //Task<BaseResponse<PhotoDto>> AddPhotoAsync(IFormFile file, string userEmail);
        //Task<BaseResponse<string>> SetMainPhotoAsync(int photoId, string userEmail);
        //Task<BaseResponse<string>> DeletePhotoAsync(int photoId, string userEmail);
    }
}
