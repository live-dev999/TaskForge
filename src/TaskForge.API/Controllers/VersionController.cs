using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TaskForge.Application.Core;

namespace TaskForge.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class VersionController : BaseApiController
    {
        #region Fields

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<VersionController> _logger;

        #endregion


        #region Ctors

        public VersionController(IWebHostEnvironment environment, ILogger<VersionController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        #endregion


        #region Methods

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            return HandleResult(
                await Task.Run(() =>
                {
                    try
                    {
                        var execVersion = GetVersion();
                        _logger.LogInformation($"get version - {execVersion}");
                        var versionResult = new VersionResult
                        {
                            Environment = _environment.EnvironmentName,
                            Version = execVersion?.ToString()
                        };

                        return Result<VersionResult>.Success(versionResult);
                    }
                    catch
                    {
                        return Result<VersionResult>.Failure("Can't get version");
                    }
                })
            );
        }

        protected virtual Version GetVersion()
        {
            var execVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return execVersion ?? throw new InvalidOperationException();
        }

        #endregion
    }
}
