namespace Ploeh.Samples.UserManagement
{
    public class OkNegotiatedContentResult<T> : IHttpActionResult
    {
        public OkNegotiatedContentResult(T content)
        {
            this.Content = content;
        }

        public T Content { get; }
    }
}