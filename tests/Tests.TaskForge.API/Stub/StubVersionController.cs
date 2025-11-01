using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using TaskForge.API.Controllers;

namespace Tests.TaskForge.API.Stub;

/// <summary>
/// Stub version of VersionController for testing purposes
/// Allows overriding version retrieval for unit tests
/// </summary>
public class StubVersionController : VersionController
{
    public string VersionString { get; set; }

    public StubVersionController(IWebHostEnvironment environment, ILogger<VersionController> logger)
        : base(environment, logger) { }

    protected override Version GetVersionFromAssembly()
    {
        if (string.IsNullOrEmpty(VersionString))
        {
            throw new InvalidOperationException("Version string is not set");
        }
        return Version.Parse(VersionString);
    }
}

