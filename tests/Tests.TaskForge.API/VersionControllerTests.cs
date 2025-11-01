using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Tests.TaskForge.API.Stub;

namespace Tests.TaskForge.API
{
    public class VersionControllerTests
    {
        #region Fields

        private readonly ILogger<StubVersionController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion


        #region

        public VersionControllerTests()
        {
            _logger = A.Fake<ILogger<StubVersionController>>();
            _webHostEnvironment = A.Fake<IWebHostEnvironment>();
        }

        #endregion


        #region Methods

        [Fact]
        public async void VersionController_GetVersion_Test()
        {
            var stub = new StubVersionController(_webHostEnvironment, _logger)
            {
                VersionString = "1.0.0.0"
            };

            var result = await stub.IndexAsync();

            // Assert
            Assert.NotNull(result);
        }

        #endregion
    }
}
