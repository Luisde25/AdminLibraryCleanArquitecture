using AdminLibrary.Constants;
using AdminLibrary.Controllers.Movement.requests;
using AdminLibrary.Core.Dtos;
using AdminLibrary.Model.Models;
using AdminLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminLibrary.Core.Interfaces
{
    public class MaterialsMovementsService : IMaterialsMovements
    {
        private readonly AppDbContext _dbContext;
        public MaterialsMovementsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
       

        public async Task<List<MovementsDto>> ListMovements()
        {
            var listMovements = await(from m in _dbContext.Movements
                                      join u in _dbContext.Users on m.UserId equals u.Id
                                      join mt in _dbContext.Materials on m.MaterialsId equals mt.Id
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

        public async Task<ResponseDto> Loans(MovementsControllerRequestDto movementsControllerRequest)
        {
            ResponseDto response = new();
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(ConstantsApi.utcNow, ConstantsApi.timeZoneInfo);
            try
            {
                if (string.IsNullOrEmpty(movementsControllerRequest.MovementType))
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.requestInvalid);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }

                var material = await _dbContext.Materials.FirstOrDefaultAsync(m => m.Id == movementsControllerRequest.MaterialId);

                if (material == null)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.MaterialNoFound);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == movementsControllerRequest.UserId);

                if (user == null)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.UserNoFound);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }

                var listMovements = await(from m in _dbContext.Movements
                                          join u in _dbContext.Users on m.UserId equals u.Id
                                          join mt in _dbContext.Materials on m.MaterialsId equals mt.Id
                                          where m.MovementType == ConstantsApi.Loans && u.Id == user.Id
                                          select m).ToListAsync();

                if (listMovements.Select(x => x.MaterialsId).Count() > material.CurrentQuantity)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.LimitMaterial);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }


                if (movementsControllerRequest.MovementType.ToLower() == ConstantsApi.Loans.ToLower() && user.UserType.ToLower() == ConstantsApi.Student.ToLower() && listMovements.Select(x => x.UserId).Count() > ConstantsApi.CantStudent)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.MaxEstudents);
                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;

                }
                else if (movementsControllerRequest.MovementType.ToLower() == ConstantsApi.Loans.ToLower() && user.UserType.ToLower() == ConstantsApi.Teacher.ToLower() && listMovements.Select(x => x.UserId).Count() > ConstantsApi.CantTeacher)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.MaxProf);
                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }
                else if (movementsControllerRequest.MovementType.ToLower() == ConstantsApi.Loans.ToLower() && user.UserType.ToLower() == ConstantsApi.Admin.ToLower() && listMovements.Select(x => x.UserId).Count() > ConstantsApi.CantAdmin)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.MaxAdmin);
                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }

                var movimiento = new MaterialsMovements
                {
                    MaterialsId = material.Id,
                    UserId = user.Id,
                    MovementType = movementsControllerRequest.MovementType,
                    MovementDate = localTime,
                    Observations = movementsControllerRequest.Observations
                };

                await _dbContext.Movements.AddAsync(movimiento);
                var result = await _dbContext.SaveChangesAsync();

                var historyMaterial = new MaterialHistory(material.Id,
                                                            user.Id,
                                                            "Prestamo de libro de " + material.Title,
                                                            ConstantsApi.Loans,
                                                            localTime
                                                        );

                await _dbContext.History.AddAsync(historyMaterial);
                await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.success);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;
                }
                else
                {
                    var failed = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.failed);
                    response.Code = failed?.Code ?? string.Empty;
                    response.Message = failed?.Message;
                }

            }
            catch (Exception ex)
            {
                var failed = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.failed);
                response.Code = failed?.Code ?? string.Empty;
                response.Message = failed?.Message + ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> Return(MovementsControllerRequestDto movementsControllerRequest)
        {
            ResponseDto response = new();
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(ConstantsApi.utcNow, ConstantsApi.timeZoneInfo);
            try
            {


                if (movementsControllerRequest.UserId == 0 && movementsControllerRequest.MaterialId == 0)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.requestInvalid);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == movementsControllerRequest.UserId);
                var material = await _dbContext.Materials.FirstOrDefaultAsync(m => m.Id == movementsControllerRequest.MaterialId);

                if (user != null && material != null)
                {
                    var deleteMovement = await _dbContext.Movements.FirstOrDefaultAsync(x => x.UserId == user.Id && x.MaterialsId == material.Id);

                    if (deleteMovement != null)
                    {
                        _dbContext.Movements.Remove(deleteMovement);
                        await _dbContext.SaveChangesAsync();

                        var historyMaterial = new MaterialHistory(material.Id,
                                                         user.Id,
                                                         "Se devuelve el libro de " + material.Title,
                                                         ConstantsApi.Return,
                                                         localTime
                                                     );

                        await _dbContext.History.AddAsync(historyMaterial);
                        await _dbContext.SaveChangesAsync();


                        var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.MateriaReturn);

                        response.Code = success?.Code ?? string.Empty;
                        response.Message = success?.Message;
                    }
                    else
                    {
                        var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.MaterialNoFound);

                        response.Code = success?.Code ?? string.Empty;
                        response.Message = success?.Message;
                    }

                }
                else
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.MaterialNoFound);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;
                }

            }
            catch (Exception ex)
            {
                var failed = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.failed);
                response.Code = failed?.Code ?? string.Empty;
                response.Message = failed?.Message + ex.Message;
            }
            return response;
        }
    }
}
