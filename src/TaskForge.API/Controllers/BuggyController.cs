using Microsoft.AspNetCore.Mvc;

namespace TaskForge.API.Controllers
{

    /// <summary>
    /// Controller for testing client error behavior
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BuggyController : BaseApiController
    {
        #region Methods

        [HttpGet("not-found")]
        public ActionResult GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("bad-request")]
        public ActionResult GetBadRequest()
        {
            return BadRequest("This is a bad request");
        }

        [HttpGet("server-error")]
        public ActionResult GetServerError()
        {
            throw new Exception("This is a server error");
        }

        [HttpGet("unauthorised")]
        public ActionResult GetUnauthorised()
        {
            return Unauthorized();
        }

        #endregion
    }
}
