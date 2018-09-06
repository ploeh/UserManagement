using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.UserManagement.UnitTests
{
    public class FakeDB : Collection<User>, IUserReader, IUserRepository
    {
        public IResult<User, IUserLookupError> Lookup(string id)
        {
            if (!(int.TryParse(id, out int i)))
                return Result.Error<User, IUserLookupError>(
                    UserLookupError.InvalidId);

            var user = this.FirstOrDefault(u => u.Id == i);
            if (user == null)
                return Result.Error<User, IUserLookupError>(
                    UserLookupError.NotFound);

            return Result.Success<User, IUserLookupError>(user);
        }

        public bool IsDirty { get; set; }

        public void Update(User user)
        {
            IsDirty = true;
            if (!Contains(user))
                Add(user);
        }
    }
}
