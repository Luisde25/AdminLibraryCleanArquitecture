using AdminLibrary.Controllers.UsersModel.request;
using AdminLibrary.Core.Dtos;
using AdminLibrary.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminLibrary.Controllers.UsersModel
{
    [ApiController]
    [Route("api/Users")]
    public class UsersController(
         IUserService userService
        ) : Controller
    {
        
        private readonly IUserService _userService = userService;

        #region Lista de usuarios que tienen un rol asignado
        [HttpGet("GetUsers")]
        public async Task<List<UsersDto>> GetUsers()
        {
            return await _userService.CallingListUsers();
        }
        #endregion

        #region Creación de un nuevo usuario
        [HttpPost("Create")]
        public async Task<ResponseDto> CreateUser(
           UserControllerRequestDto userControllerRequest
           )
        {
            return await _userService.CreateNewUser(userControllerRequest);
        }

        #endregion

        #region Actualizar usuario existe
        [HttpPut("Update")]
        public async Task<ResponseDto> UpdateUser(
          UserControllerUpdate userControllerUpdate
          )
        {
            return await _userService.UpdateCurrentUser( userControllerUpdate);
        }
        #endregion 

        #region Eliminar usuario existente
        [HttpDelete("Remove")]
        public async Task<ResponseDto> DeleteUser( int id
       )
        {
            return await _userService.RemoveCurrentUser( id);
        }
        #endregion
    }
}
