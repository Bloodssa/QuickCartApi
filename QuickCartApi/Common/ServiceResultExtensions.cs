using Microsoft.AspNetCore.Mvc;

namespace QuickCartApi.Common
{
    // resolve the validation or status code error in controller result.ToProblem();
    public static class ServiceResultExtensions
    {
        public static ActionResult ToProblem(this ServiceResult result) => result.Kind switch
        {
            ErrorKind.NotFound => new NotFoundObjectResult(new { message = result.Error }),
            ErrorKind.Conflict => new ConflictObjectResult(new { message = result.Error }),
            ErrorKind.Forbidden => new ObjectResult(new { message = result.Error }) { StatusCode = StatusCodes.Status403Forbidden },
            _ => new BadRequestObjectResult(new { message = result.Error })
        };
    }
}
