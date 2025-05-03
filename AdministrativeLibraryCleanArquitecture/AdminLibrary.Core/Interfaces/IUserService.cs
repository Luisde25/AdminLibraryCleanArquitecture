using AdminLibrary.Controllers.UsersModel.request;
using AdminLibrary.Core.Dtos;

namespace AdminLibrary.Core.Interfaces
{
    public interface IUserService
    {
        Task<List<UsersDto>> CallingListUsers();
        Task<ResponseDto> CreateNewUser(UserControllerRequestDto userControllerRequest);
        Task<ResponseDto> UpdateCurrentUser(UserControllerUpdate userControllerUpdate);
        Task<ResponseDto> RemoveCurrentUser(int id);
    }
}
