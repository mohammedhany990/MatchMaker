using MatchMaker.Core.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            return code switch
            {
                400 => BadRequest(new BaseResponse<ErrorsController>(400)),
                401 => Unauthorized(new BaseResponse<ErrorsController>(401)),
                404 => NotFound(new BaseResponse<ErrorsController>(404)),
                _ => StatusCode(code)
            };
        }
    }
}
