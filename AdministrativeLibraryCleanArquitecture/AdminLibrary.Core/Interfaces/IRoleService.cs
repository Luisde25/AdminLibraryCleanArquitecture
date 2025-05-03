using AdminLibrary.Controllers.RolesModel.request;
using AdminLibrary.Core.Dtos;

namespace AdminLibrary.Core.Interfaces
{
    public interface IRoleService
    {
        Task<List<RolesDto>> ListRoles();
        Task<ResponseDto> CreateNewRol(RoleControllerRequestDto roleControllerRequest);
        Task<ResponseDto> UpdateCurrentRol(RoleControllerRequestUpdate roleControllerRequest);
    }
}
