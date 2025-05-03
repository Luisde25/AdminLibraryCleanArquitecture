using AdminLibrary.Constants;
using AdminLibrary.Controllers.Materials.request;
using AdminLibrary.Core.Dtos;
using AdminLibrary.Model.Models;
using AdminLibrary.Models;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminLibrary.Core.Interfaces
{
    public class MaterialService : IMaterialService
    {
        private readonly AppDbContext _dbContext;
        public MaterialService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MaterialDto>> ListMaterials()
        {
            var listMaterials = await _dbContext.Materials.ToListAsync();

            if (listMaterials.Count == 0)
            {
                return [];
            }

            return listMaterials.Select(m => new MaterialDto(
                                        m.Id,
                                        m.Identifier,
                                        m.Title,
                                        m.RegisterDate.ToString("yyyyMMdd HH:mm:ss"),
                                        m.RegisterQuantity,
                                        m.CurrentQuantity
                                    )).ToList();
        }

        public async Task<ResponseDto> RegisterNewMaterial(MaterialControllerRequestDto materialControllerRequest)
        {
            ResponseDto response = new();
            try
            {
                if (string.IsNullOrEmpty(materialControllerRequest.Identifier) ||
                    string.IsNullOrEmpty(materialControllerRequest.Title) ||
                    materialControllerRequest.RegisterQuantity <= 0
                    )
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.requestInvalid);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }

                var isExist = await _dbContext.Materials.FirstOrDefaultAsync(x => x.Identifier == materialControllerRequest.Identifier);

                if (isExist != null)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.ExistMaterial);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;

                }

                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(ConstantsApi.utcNow, ConstantsApi.timeZoneInfo);
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == materialControllerRequest.userId);

                if (user == null)
                {
                    var success = await _dbContext.Response.FirstOrDefaultAsync(r => r.Code == Codes.UserNoFound);

                    response.Code = success?.Code ?? string.Empty;
                    response.Message = success?.Message;

                    return response;
                }


                var material = new MaterialsModel(
                    materialControllerRequest.Identifier,
                    materialControllerRequest.Title,
                    localTime,
                    materialControllerRequest.RegisterQuantity,
                    materialControllerRequest.RegisterQuantity
                    );

                await _dbContext.Materials.AddAsync(material);
                var result = await _dbContext.SaveChangesAsync();

                var historyMaterial = new MaterialHistory(material.Id,
                                                        materialControllerRequest.userId,
                                                        ConstantsApi.Message,
                                                         ConstantsApi.Register,
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

        public async Task<ResponseDto> UpdateCurrentNewMaterial(MaterialControllerUpdate materialControllerRequest)
        {
            ResponseDto response = new();
            try
            {
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(ConstantsApi.utcNow, ConstantsApi.timeZoneInfo);

                var isExist = await _dbContext.Materials.FirstOrDefaultAsync(x => x.Id == materialControllerRequest.Id);

                if (isExist != null)
                {
                    isExist.Title = materialControllerRequest.Title;
                    isExist.RegisterQuantity = materialControllerRequest.RegisterQuantity;
                    isExist.CurrentQuantity = materialControllerRequest.CurrentQuantity;
                    isExist.UpdateDate = localTime;
                    isExist.UpdateUser = "Admin";

                    _dbContext.Materials.Update(isExist);
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
