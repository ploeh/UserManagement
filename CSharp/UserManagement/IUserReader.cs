namespace Ploeh.Samples.UserManagement
{
    public interface IUserReader
    {
        IResult<User, IUserLookupError> Lookup(string id);
    }
}