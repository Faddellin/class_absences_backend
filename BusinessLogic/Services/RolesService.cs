using BusinessLogic.ServiceInterfaces;
using Common.DbModels;
using Common.DtoModels;
using Common.DtoModels.Request;
using Common.DtoModels.User;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using static BusinessLogic.Services.RolesService;
using BusinessLogic.Static;

namespace BusinessLogic.Services;

public class RolesService : IRolesService
{
    private readonly AppDbContext _appDbContext;

    public RolesService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    public async Task ChangeRole(Guid userId, Guid targetUserId, UserType newUserType)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        UserEntity? targetUserEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == targetUserId);

        Validator.ThrowIfNull(userEntity);
        Validator.ThrowIfNull(targetUserEntity);

        Validator.ThrowIfNotEnoughAccess(userEntity.UserType, targetUserEntity.UserType);
        Validator.ThrowIfNotEnoughAccess(userEntity.UserType, newUserType);


        targetUserEntity.UserType = newUserType;
        await _appDbContext.SaveChangesAsync();

        return;
    }
}