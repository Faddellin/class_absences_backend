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


    public async Task AddRole(Guid userId, Guid targetUserId, UserType newUserType)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        UserEntity? targetUserEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == targetUserId);

        Validator.ThrowIfNull(userEntity);
        Validator.ThrowIfNull(targetUserEntity);

        Validator.ThrowIfNotEnoughAccess(userEntity.UserTypes.Max(), 2);
        Validator.ThrowIfNotEnoughAccess(userEntity.UserTypes.Max(), targetUserEntity.UserTypes.Max());
        Validator.ThrowIfNotEnoughAccess(userEntity.UserTypes.Max(), newUserType);
        
        if (userId == targetUserId)
        {
            Validator.ThrowIfNotEnoughAccess(userEntity.UserTypes.Max(), 3);
        }

        if (targetUserEntity.UserTypes.Contains(newUserType))
        {
            throw new ArgumentException("User already has this role");
        }

        targetUserEntity.UserTypes.Add(newUserType);
        await _appDbContext.SaveChangesAsync();

        return;
    }

    public async Task DeleteRole(Guid userId, Guid targetUserId, UserType newUserType)
    {
        UserEntity? userEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == userId);
        UserEntity? targetUserEntity = await _appDbContext.Users.FirstOrDefaultAsync(o => o.Id == targetUserId);

        Validator.ThrowIfNull(userEntity);
        Validator.ThrowIfNull(targetUserEntity);

        Validator.ThrowIfNotEnoughAccess(userEntity.UserTypes.Max(), 2);
        Validator.ThrowIfNotEnoughAccess(userEntity.UserTypes.Max(), targetUserEntity.UserTypes.Max());
        Validator.ThrowIfNotEnoughAccess(userEntity.UserTypes.Max(), newUserType);

        if (userId == targetUserId)
        {
            Validator.ThrowIfNotEnoughAccess(userEntity.UserTypes.Max(), 3);
        }

        if (!targetUserEntity.UserTypes.Contains(newUserType))
        {
            throw new ArgumentException("User doesn't have this role");
        }

        if (targetUserEntity.UserTypes.Count() == 1)
        {
            throw new ArgumentException("Can't delete last user role");
        }

        targetUserEntity.UserTypes.Remove(newUserType);
        await _appDbContext.SaveChangesAsync();

        return;
    }
}