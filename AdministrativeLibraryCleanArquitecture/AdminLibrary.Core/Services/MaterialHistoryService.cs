using AdminLibrary.Core.Dtos;
using AdminLibrary.Core.Interfaces;
using AdminLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminLibrary.Core.Services
{
    public class MaterialHistoryService : IMaterialHistory
    {
        private readonly AppDbContext _context;
        public MaterialHistoryService(AppDbContext _context)
        {
            _context = _context ?? throw new ArgumentNullException("Error de conexión");
        }


        public async Task<List<MovementsDto>> HistoryMovements()
        {
            var listMovements = await (from m in _context.History
                                       join u in _context.Users on m.UserId equals u.Id
                                       join mt in _context.Materials on m.MaterialsId equals mt.Id
                                       select new MovementsDto(
                                           mt.Title,
                                           u.UserName!,
                                           m.Observations,
                                           m.MovementType,
                                           m.MovementDate
                                       )).ToListAsync();

            if (listMovements.Count == 0)
            {
                return [];
            }

            return listMovements;
        }
    }
}
