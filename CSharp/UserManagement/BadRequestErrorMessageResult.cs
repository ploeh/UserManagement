namespace Ploeh.Samples.UserManagement
{
    public class BadRequestErrorMessageResult : IHttpActionResult
    {
        public BadRequestErrorMessageResult(string message)
        {
            this.Message = message;
        }

        public string Message { get; }
    }
}