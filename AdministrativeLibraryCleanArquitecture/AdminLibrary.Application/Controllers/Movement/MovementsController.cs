using AdminLibrary.Controllers.Movement.requests;
using AdminLibrary.Core.Dtos;
using AdminLibrary.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminLibrary.Controllers.Movement
{
    [ApiController]
    [Route("api/Materials")]
    public class MovementsController(
         IMaterialHistory materialHistoryService,
         IMaterialsMovements materialMovementsService
        ) : Controller
    {
       
        private readonly IMaterialHistory _materialHistoryService = materialHistoryService;
        private readonly IMaterialsMovements _materialMovementsService = materialMovementsService;
    
     

        #region Lista de libros
        [HttpGet("GetLoans")]
        public async Task<List<MovementsDto>> GetMovements()
        {
            return await _materialMovementsService.ListMovements();
        }
        #endregion

        #region Historial de materiales
        [HttpGet("history")]
        public async Task<List<MovementsDto>> History()
        {
            return await _materialHistoryService.HistoryMovements();
        }
        #endregion

        #region Prestar libros
        [HttpPost("Loans")]
        public async Task<ResponseDto> Movements(
            MovementsControllerRequestDto movementsControllerRequest
            )
        {
            return await _materialMovementsService.Loans( movementsControllerRequest);
        }
        #endregion

        #region Devolver libros
        [HttpDelete("return")]
        public async Task<ResponseDto> DeleteLoans(
            MovementsControllerRequestDto movementsControllerRequest
            )
        {
            return await _materialMovementsService.Return(movementsControllerRequest);
        }
        #endregion

        
    }
}
