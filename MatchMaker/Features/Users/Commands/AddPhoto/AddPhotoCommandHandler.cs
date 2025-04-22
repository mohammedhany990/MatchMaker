using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Users.Commands.AddPhoto
{
    public class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand, BaseResponse<PhotoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddPhotoCommandHandler(IUnitOfWork unitOfWork, IPhotoService photoService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<PhotoDto>> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        {
            var email = _httpContextAccessor.HttpContext.User.GetEmail();

            var user = await _unitOfWork.Repository<AppUser, string>()
                .GetAsync(x => x.Email == email);

            if (user is null)
            {
                return new BaseResponse<PhotoDto>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "User not found"
                );
            }

            var result = await _photoService.AddPhotoAsync(request.File);

            if (result.Error is not null)
            {
                return new BaseResponse<PhotoDto>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: result.Error.Message
                );
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user.Photos.Add(photo);

            var done = await _unitOfWork.SaveAsync();
            if (done > 0)
            {
                var mappedPhoto = _mapper.Map<PhotoDto>(photo);
                return new BaseResponse<PhotoDto>(
                    statusCode: StatusCodes.Status201Created,
                    success: true,
                    data: mappedPhoto,
                    message: "Photo added successfully"
                );
            }

            return new BaseResponse<PhotoDto>(
                statusCode: StatusCodes.Status400BadRequest,
                success: false,
                message: "Problem adding photo"
            );
        }
    }

}
