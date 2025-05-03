using AdminLibrary.Core.Dtos;

namespace AdminLibrary.Core.Interfaces
{
    public interface IMaterialHistory
    {
        Task<List<MovementsDto>> HistoryMovements();
    }
}
