using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using Microsoft.AspNetCore.Http;

namespace MatchMaker.Service.Abstracts
{
    public interface IUserService
    {
        Task<PaginatedResponse<MemberDto>> GetAllUsersAsync(UserParams userParams, string currentUsername);
        Task<BaseResponse<MemberDto>> GetUserByEmailAsync(string email);
        Task<BaseResponse<MemberDto>> GetUserByUsernameAsync(string username);
        Task<BaseResponse<PhotoDto>> AddPhotoAsync(IFormFile file, string userEmail);
        Task<BaseResponse<string>> SetMainPhotoAsync(int photoId, string userEmail);
        Task<BaseResponse<string>> DeletePhotoAsync(int photoId, string userEmail);
    }
}
