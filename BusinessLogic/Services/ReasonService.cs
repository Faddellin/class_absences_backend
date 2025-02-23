using System.Drawing;
using System.Drawing.Imaging;
using BusinessLogic.ServiceInterfaces;
using Common.DbModels;
using Common.DtoModels.Reason;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class ReasonService : IReasonService
{
    private readonly AppDbContext _appDbContext;

    public ReasonService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<ReasonListModel?> GetAllUsersReasons(Guid userId)
    {
        var user = await _appDbContext.Users
            .FindAsync(userId);
        if (user == null)
        {
            return null;
        }

        var reasons = await _appDbContext.Reasons
            .Include(r => r.User)
            .Where(r => r.User == user)
            .ToListAsync();
        var reasonList = reasons.Select(reason => new ReasonModel
            {
                DateFrom = reason.DateFrom,
                DateTo = reason.DateTo,
                Description = reason.Description,
                Id = reason.Id,
                Images = reason.Images!,
                ReasonType = reason.ReasonType
            })
            .ToList();

        return new ReasonListModel() { ReasonList = reasonList };
    }

    public async Task<Guid?> CreateReason(Guid userId, IFormFileCollection files, 
        ReasonCreateModel createModel)
    {
        var user = await _appDbContext.Users
            .FindAsync(userId);
        if (user == null)
        {
            return null;
        }

        var urlsList = new List<string>();
        if (files.Count > 0)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine("static/images/reasons", fileName);
                    urlsList.Add(filePath);

                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using var image = new Bitmap(stream);
                    image.Save(filePath, ImageFormat.Jpeg);
                }
            }           
        }


        var reason = new ReasonEntity
        {
            DateFrom = createModel.DateFrom,
            DateTo = createModel.DateTo,
            Description = createModel.Description,
            Id = Guid.NewGuid(),
            Images = urlsList!,
            ReasonType = ReasonType.Unchecked,
            User = user
        };

        await _appDbContext.Reasons.AddAsync(reason);
        await _appDbContext.SaveChangesAsync();

        return reason.Id;
    }
}