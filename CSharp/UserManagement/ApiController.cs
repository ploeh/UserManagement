using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public abstract class ApiController
    {
        protected IHttpActionResult BadRequest(string message)
        {
            return new BadRequestErrorMessageResult(message);
        }

        protected IHttpActionResult Ok<T>(T content)
        {
            return new OkNegotiatedContentResult<T>(content);
        }
    }
}
