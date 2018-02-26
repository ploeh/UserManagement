namespace Ploeh.Samples.UserManagement
{
    internal class ErrorUserLookupResult<S> : IUserLookupResult<S>
    {
        private IUserLookupError error;

        public ErrorUserLookupResult(IUserLookupError error)
        {
            this.error = error;
        }

        public TResult Accept<TResult>(
            IUserLookupResultVisitor<S, TResult> visitor)
        {
            return visitor.VisitError(error);
        }
    }
}