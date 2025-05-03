using AdminLibrary.Constants;
using AdminLibrary.Controllers.UsersModel.request;
using AdminLibrary.Core.Dtos;
using AdminLibrary.Models;
using AdminLibrary.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminLibrary.Core.Interfaces
{
    public class UserService : IUserService
    {
        private readonly  AppDbContext _dbContext;
        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public async Task<List<UsersDto>> CallingListUsers()
        {
            var listUsers = await _dbContext.Users.Include(u => u.UsersRolesVirtual)
                                                   .ThenInclude(r => r.RolesVirtual).ToListAsync();

            if (listUsers.Count == 0)
                return [];

            var users = listUsers.Select(m => new UsersDto(
                                        m.Id,
                                        m.FirtsName,
                                        m.MiddleName,
                                        m.FirtsLastName,
                                        m.SecondLastName,
                                        m.TypeIdentification,
                                        m.NumberIdentification,
                                        m.Status,
                                        m.UserName,
                                        m.UserType,
                                        m.UsersRolesVirtual?.FirstOrDefault()?.RolesVirtual.Name ?? string.Empty
                                    ));


            return users.Where(x => !string.IsNullOrEmpty(x.RolName)).ToList();
        }

        public async Task<ResponseDto> CreateNewUser(UserControllerRequestDto userControllerRequest)
        {
            ResponseDto response = new();
            try
            {
                if (string.IsNullOrEmpty(userControllerRequest.FirtsName) ||
                string.IsNullOrEmpty(userControllerRequest.FirtsLastName) ||
                string.IsNullOrEmpty(userControllerRequest.TypeIdentification) ||
                    string.IsNullOrEmpty(userControllerRequest.NumberIdentification)
                    )
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.requestInvalid);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;
                    return response;
                }

                var isExist = await _dbContext.Users.FirstOrDefaultAsync(x => x.NumberIdentification == userControllerRequest.NumberIdentification);

                if (isExist != null)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.existsUser);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;
                    return response;
                }
                var users = new Users(userControllerRequest.FirtsName!,
                userControllerRequest.MiddleName,
                userControllerRequest.FirtsLastName,
                                      userControllerRequest.SecondLastName ?? string.Empty,
                                      userControllerRequest.TypeIdentification,
                                      userControllerRequest.NumberIdentification,
                                      userControllerRequest.Status,
                                      userControllerRequest.UserName,
                                      userControllerRequest.UserType!
                                      );

                await _dbContext.Users.AddAsync(users);
                var result = await _dbContext.SaveChangesAsync();

                if (userControllerRequest.Rol != 0)
                {
                    var roleEntities = await _dbContext.Roles.ToListAsync();

                    var roleAssignments = roleEntities.Select(role => new UsersRoles
                    {
                        IdUser = users.Id,
                        IdRol = userControllerRequest.Rol
                    }).ToList();

                    await _dbContext.UsersRoles.AddRangeAsync(roleAssignments);
                    await _dbContext.SaveChangesAsync();
                }

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

        public async Task<ResponseDto> RemoveCurrentUser(int id)
        {
            ResponseDto response = new();
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(r => r.Id == id);

                if (user != null)
                {
                    var loans = _dbContext.Movements.Where(r => r.UserId == id).ToList();
                    var userRol = await _dbContext.UsersRoles.Where(x => x.IdUser == id).ToListAsync();
                    if (loans.Count > 0)
                    {
                        var failed = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.UserNoDelete);
                        response.Code = failed?.Code ?? string.Empty;
                        response.Message = failed?.Message;

                        return response;
                    }


                    _dbContext.UsersRoles.RemoveRange(userRol);
                    await _dbContext.SaveChangesAsync();

                    _dbContext.Users.Remove(user);
                    await _dbContext.SaveChangesAsync();

                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.DeleteSuccess);
                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;
                }
                else
                {
                    var failed = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.MaterialNoFound);
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

        public async Task<ResponseDto> UpdateCurrentUser(UserControllerUpdate userControllerUpdate)
        {
            ResponseDto response = new();
            try
            {
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(ConstantsApi.utcNow, ConstantsApi.timeZoneInfo);

                var isExist = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userControllerUpdate.Id);
                var roleEntities = await _dbContext.UsersRoles.FirstOrDefaultAsync(x => x.IdRol == userControllerUpdate.Rol);

                if (isExist != null && roleEntities != null)
                {

                    isExist.Id = userControllerUpdate.Id!;
                    isExist.FirtsName = userControllerUpdate.FirtsName!;
                    isExist.MiddleName = userControllerUpdate.MiddleName!;
                    isExist.FirtsLastName = userControllerUpdate.FirtsLastName!;
                    isExist.SecondLastName = userControllerUpdate.SecondLastName!;
                    isExist.TypeIdentification = userControllerUpdate.TypeIdentification!;
                    isExist.NumberIdentification = userControllerUpdate.NumberIdentification!;
                    isExist.Status = userControllerUpdate.Status;
                    isExist.UserName = userControllerUpdate.UserName!;
                    roleEntities.IdRol = userControllerUpdate.Rol;
                    isExist.UpdateDate = localTime;
                    isExist.UpdateUser = "Admin";

                    _dbContext.Users.Update(isExist);
                    var result = await _dbContext.SaveChangesAsync();

                    _dbContext.UsersRoles.Update(roleEntities);
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
