using BusinessLogic.ServiceInterfaces;
using Quartz;

namespace class_absences_backend.Jobs;

public class RemoveJwtJob : IJob
{
    private readonly ITokenService _tokenService;

    public RemoveJwtJob(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _tokenService.ClearTokens();
    }
}