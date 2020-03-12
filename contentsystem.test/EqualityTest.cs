using System;
using Xunit;

namespace Randomous.ContentSystem.test
{
    public class EqualityTest : UnitTestBase
    {
        [Fact]
        public void TestBaseEquality()
        {
            var now = DateTime.Now;
            Assert.Equal(new BaseSystemObject(), new BaseSystemObject());
            Assert.Equal(new BaseSystemObject() { id = 1 }, new BaseSystemObject() { id = 1 });
            Assert.Equal(new BaseSystemObject() { createDate = now }, new BaseSystemObject() { createDate = now });
        }

        [Fact]
        public void TestBaseInequality()
        {
            Assert.NotEqual(new BaseSystemObject() { id = 8 }, new BaseSystemObject() { id = 9 });
            Assert.NotEqual(
                new BaseSystemObject() { createDate = DateTime.Now },
                new BaseSystemObject() { createDate = DateTime.Now.AddMinutes(1) });
        }

        [Fact]
        public void TestUserEquality()
        {
            //Screw these stupid individual unit tests. It's so much easier to just have it in the same place.
            Assert.Equal(new User(), new User());
            Assert.Equal(new User() { username = "yes" }, new User() { username = "yes" });
            Assert.Equal(new User() { email = "yes" }, new User() { email = "yes" });
            Assert.Equal(new User() { passwordHash = "yes" }, new User() { passwordHash = "yes" });
        }

        [Fact]
        public void TestUserInequality()
        {
            Assert.NotEqual(new User() { username = "yes" }, new User() { username = "no" });
            Assert.NotEqual(new User() { email = "yes" }, new User() { email = "no" });
            Assert.NotEqual(new User() { passwordHash = "yes" }, new User() { passwordHash = "no" });
        }
    }
}