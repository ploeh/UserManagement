namespace Ploeh.Samples.UserManagement
{
    internal class ErrorUserLookupResult<S, E> : IUserLookupResult<S, E>
    {
        private readonly E error;

        public ErrorUserLookupResult(E error)
        {
            this.error = error;
        }

        public TResult Accept<TResult>(
            IUserLookupResultVisitor<S, E, TResult> visitor)
        {
            return visitor.VisitError(error);
        }
    }
}