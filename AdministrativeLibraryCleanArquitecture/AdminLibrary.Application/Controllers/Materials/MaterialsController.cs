using AdminLibrary.Controllers.Materials.request;
using AdminLibrary.Core.Dtos;
using AdminLibrary.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminLibrary.Controllers.audioVisual
{

    [ApiController]
    [Route("api/Materials")]
    public class MaterialsController(
            IMaterialService materialService
            ) : Controller
    {
        private readonly IMaterialService _materialService = materialService;

        #region Lista de materiales
        [HttpGet("GetMaterials")]
        public async Task<List<MaterialDto>> GetMaterials()
        {
            return await _materialService.ListMaterials();
        }
        #endregion

        #region Registrar un nuevo libro
        [HttpPost("Register")]
        public async Task<ResponseDto> CreateMaterial(
            MaterialControllerRequestDto materialControllerRequest
            )
        {
            return await _materialService.RegisterNewMaterial( materialControllerRequest);
        }
        #endregion

        #region Actualizar libro existente
        [HttpPut("Update")]
        public async Task<ResponseDto> UpdateMaterial(
            MaterialControllerUpdate materialControllerRequest
            )
        {
            return await _materialService.UpdateCurrentNewMaterial( materialControllerRequest);
        }
        #endregion
    }
}
