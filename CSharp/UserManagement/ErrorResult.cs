namespace Ploeh.Samples.UserManagement
{
    internal class ErrorResult<S, E> : IResult<S, E>
    {
        private readonly E error;

        public ErrorResult(E error)
        {
            this.error = error;
        }

        public TResult Accept<TResult>(IResultVisitor<S, E, TResult> visitor)
        {
            return visitor.VisitError(error);
        }
    }
}