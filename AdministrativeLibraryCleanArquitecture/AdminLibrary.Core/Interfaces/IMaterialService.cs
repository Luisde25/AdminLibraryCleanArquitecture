using AdminLibrary.Controllers.Materials.request;
using AdminLibrary.Core.Dtos;

namespace AdminLibrary.Core.Interfaces
{
    public interface IMaterialService
    {
        Task<List<MaterialDto>> ListMaterials();
        Task<ResponseDto> RegisterNewMaterial(MaterialControllerRequestDto materialControllerRequest);
        Task<ResponseDto> UpdateCurrentNewMaterial(MaterialControllerUpdate materialControllerRequest);
    }
}
