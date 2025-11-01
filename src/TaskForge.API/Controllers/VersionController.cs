using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using TaskForge.Application.Core;

namespace TaskForge.API.Controllers
{
    /// <summary>
    /// Controller for retrieving application version information
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : BaseApiController
    {
        #region Fields

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<VersionController> _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the VersionController class
        /// </summary>
        /// <param name="environment">Web hosting environment</param>
        /// <param name="logger">Logger instance</param>
        public VersionController(IWebHostEnvironment environment, ILogger<VersionController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the application version and environment information
        /// </summary>
        /// <returns>Version information including environment name and version number</returns>
        [HttpGet]
        public IActionResult GetVersion()
        {
            try
            {
                var execVersion = GetVersionFromAssembly();
                _logger.LogInformation("Retrieved application version: {Version}", execVersion);
                
                var versionResult = new VersionResult
                {
                    Environment = _environment.EnvironmentName,
                    Version = execVersion?.ToString()
                };

                return HandleResult(Result<VersionResult>.Success(versionResult));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve application version");
                return HandleResult(Result<VersionResult>.Failure("Unable to retrieve version information"));
            }
        }

        /// <summary>
        /// Gets the version from the executing assembly
        /// </summary>
        /// <returns>Assembly version</returns>
        protected virtual Version GetVersionFromAssembly()
        {
            var execVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return execVersion ?? throw new InvalidOperationException("Assembly version is null");
        }

        #endregion
    }
}
