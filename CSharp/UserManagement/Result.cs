using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement
{
    public static class Result
    {
        public static IResult<S, E> Success<S, E>(S success)
        {
            return new SuccessResult<S, E>(success);
        }

        public static IResult<S, E> Error<S, E>(E error)
        {
            return new ErrorResult<S, E>(error);
        }
    }
}
