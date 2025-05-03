using AdminLibrary.Controllers.RolesModel.request;
using AdminLibrary.Core.Dtos;
using AdminLibrary.Core.Interfaces;
using AdminLibrary.Model.Models;
using AdminLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminLibrary.Controllers.RolesModel
{
    [ApiController]
    [Route("api/Roles")]
    public class RolesController(
        IRoleService rolesService
        ) : Controller
    {
      
        private readonly IRoleService _roles = rolesService;

        #region Obtener lista de roles
        [HttpGet("GetRoles")]
        public async Task<List<RolesDto>> GetRoles()
        {
            return await _roles.ListRoles();
        }
        #endregion

        #region Creación de un nuevo rol
        [HttpPost("Create")]
        public async Task<ResponseDto> CreateRole(
            RoleControllerRequestDto roleControllerRequest
            )
        {
            return await _roles.CreateNewRol( roleControllerRequest);
        }

        #endregion

        #region Actual un rol existente
        [HttpPut("Update")]
        public async Task<ResponseDto> UpdateMaterial(
          RoleControllerRequestUpdate roleControllerRequest
          )
        {
            return await _roles.UpdateCurrentRol(roleControllerRequest);
        }
        #endregion

    }
}
