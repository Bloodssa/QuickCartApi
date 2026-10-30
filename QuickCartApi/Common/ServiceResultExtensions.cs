using Microsoft.AspNetCore.Mvc;

namespace QuickCartApi.Common
{
    public static class ServiceResultExtensions
    {
        public static ActionResult ToProblem(this ServiceResult result)
        {
            object response = result.Errors is not null
                ? new
                {
                    message = result.Error,
                    errors = result.Errors
                }
                : new
                {
                    message = result.Error
                };

            return result.Kind switch
            {
                ErrorKind.NotFound => new NotFoundObjectResult(response),
                ErrorKind.Conflict => new ConflictObjectResult(response),
                ErrorKind.Forbidden =>
                    new ObjectResult(response)
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    },

                _ => new BadRequestObjectResult(response)
            };
        }
    }
}
