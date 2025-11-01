using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using TaskForge.API.Controllers;

namespace Tests.TaskForge.API.Stub
{
    public class StubVersionController : VersionController
    {
        public string VersionString { get; init; }

        public StubVersionController(IWebHostEnvironment environment, ILogger<VersionController> logger)
            : base(environment, logger) { }

        protected override Version GetVersion() => Version.Parse(VersionString);
    }
}
