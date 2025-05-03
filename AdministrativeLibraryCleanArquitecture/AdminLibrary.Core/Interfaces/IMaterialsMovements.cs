using AdminLibrary.Controllers.Movement.requests;
using AdminLibrary.Core.Dtos;

namespace AdminLibrary.Core.Interfaces
{
    public interface IMaterialsMovements
    {
        Task<List<MovementsDto>> ListMovements();
        Task<ResponseDto> Loans(MovementsControllerRequestDto movementsControllerRequest);
        Task<ResponseDto> Return(MovementsControllerRequestDto movementsControllerRequest);
    }
}
