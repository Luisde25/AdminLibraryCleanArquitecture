using AdminLibrary.Constants;
using AdminLibrary.Controllers.RolesModel.request;
using AdminLibrary.Core.Dtos;
using AdminLibrary.Model.Models;
using AdminLibrary.Models;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminLibrary.Core.Interfaces
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _dbContext;
        public RoleService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<RolesDto>> ListRoles()
        {
            var listRoles = await _dbContext.Roles.ToListAsync();

            if (listRoles.Count == 0)
                return [];


            return listRoles.Select(m => new RolesDto(
                                        m.Id,
                                        m.Name,
                                        m.Description,
                                        m.Status
                                    )).ToList();
        }

        public async Task<ResponseDto> CreateNewRol(RoleControllerRequestDto roleControllerRequest)
        {
            ResponseDto response = new();
            try
            {
                if (string.IsNullOrEmpty(roleControllerRequest.Name) ||
                    string.IsNullOrEmpty(roleControllerRequest.Description))
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.requestInvalid);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }

                var isExist = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleControllerRequest.Name);

                if (isExist != null)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.existsRol);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;

                }

                var roles = new Roles(roleControllerRequest.Name,
                                      roleControllerRequest.Description,
                                      roleControllerRequest.status);

                await _dbContext.Roles.AddAsync(roles);
                var result = await _dbContext.SaveChangesAsync();

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

     
        public async Task<ResponseDto> UpdateCurrentRol(RoleControllerRequestUpdate roleControllerRequest)
        {
            ResponseDto response = new();
         try
            {
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(ConstantsApi.utcNow, ConstantsApi.timeZoneInfo);

                var isExist = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == roleControllerRequest.Id);

                if (isExist != null)
                {
                    isExist.Name = roleControllerRequest.Name;
                    isExist.Description = roleControllerRequest.Description;
                    isExist.Status = roleControllerRequest.status;
                    isExist.UpdateDate = localTime;
                    isExist.UpdateUser = "Admin";

                    _dbContext.Roles.Update(isExist);
                    var result = await _dbContext.SaveChangesAsync();
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
                else
                {
                    var failed = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.updateFailed);
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
    }
}
