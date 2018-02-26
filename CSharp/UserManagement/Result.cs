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

        public static IResult<S, E2> SelectError<S, E1, E2>(
            this IResult<S, E1> source,
            Func<E1, E2> selector)
        {
            return source.Accept(new SelectErrorResultVisitor<S, E1, E2>(selector));
        }

        private class SelectErrorResultVisitor<S, E1, E2> : IResultVisitor<S, E1, IResult<S, E2>>
        {
            private readonly Func<E1, E2> selector;

            public SelectErrorResultVisitor(Func<E1, E2> selector)
            {
                this.selector = selector;
            }

            public IResult<S, E2> VisitSuccess(S success)
            {
                return Success<S, E2>(success);
            }

            public IResult<S, E2> VisitError(E1 error)
            {
                return Error<S, E2>(selector(error));
            }
        }

        public static IResult<S2, E> Select<S1, S2, E>(
            this IResult<S1, E> source,
            Func<S1, S2> selector)
        {
            return source.Accept(new SelectResultVisitor<S1, S2, E>(selector));
        }

        private class SelectResultVisitor<S1, S2, E> : IResultVisitor<S1, E, IResult<S2, E>>
        {
            private readonly Func<S1, S2> selector;

            public SelectResultVisitor(Func<S1, S2> selector)
            {
                this.selector = selector;
            }

            public IResult<S2, E> VisitSuccess(S1 success)
            {
                return Success<S2, E>(selector(success));
            }

            public IResult<S2, E> VisitError(E error)
            {
                return Error<S2, E>(error);
            }
        }

        public static IResult<S, E> Join<S, E>(
            this IResult<IResult<S, E>, E> source)
        {
            return source.Accept(new JoinResultVisitor<S, E>());
        }

        private class JoinResultVisitor<S, E> :
            IResultVisitor<IResult<S, E>, E, IResult<S, E>>
        {
            public IResult<S, E> VisitError(E error)
            {
                return Error<S, E>(error);
            }

            public IResult<S, E> VisitSuccess(IResult<S, E> success)
            {
                return success;
            }
        }

        public static IResult<S2, E> SelectMany<S1, S2, E>(
            this IResult<S1, E> source,
            Func<S1, IResult<S2, E>> selector)
        {
            return source.Select(selector).Join();
        }

        public static IResult<S2, E> SelectMany<S1, S2, U, E>(
            this IResult<S1, E> source,
            Func<S1, IResult<U, E>> k,
            Func<S1, U, S2> s)
        {
            return source
                .SelectMany(x => k(x)
                    .SelectMany(y => Success<S2, E>(s(x, y))));
        }

        public static T Bifold<T>(this IResult<T, T> source)
        {
            return source.Accept(new BifoldResultVisitor<T>());
        }

        private class BifoldResultVisitor<T> : IResultVisitor<T, T, T>
        {
            public T VisitError(T error)
            {
                return error;
            }

            public T VisitSuccess(T success)
            {
                return success;
            }
        }
    }
}
