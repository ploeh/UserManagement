using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using System;

namespace Ploeh.Samples.UserManagement.UnitTests
{
    public class UserManagementTestConventionsAttribute : AutoDataAttribute
    {
        public UserManagementTestConventionsAttribute() :
            base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}