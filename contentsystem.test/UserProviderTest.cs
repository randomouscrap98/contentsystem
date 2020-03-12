using System;
using System.Linq;
using AutoMapper;
using Xunit;

namespace Randomous.ContentSystem.test
{
    public class UserProviderTest : UnitTestBase
    {
        protected UserProvider provider;
        protected IMapper mapper;
        
        public User GetBasicUser()
        {
            return new User()
            {
                username = DateTime.Now.Ticks.ToString(),
                email = $"thing{DateTime.Now.Ticks}@something.com",
                passwordHash = $"apasswordhash_{DateTime.Now.Ticks}"
            };
        }

        public UserProviderTest()
        {
            provider = CreateService<UserProvider>();
            mapper = CreateService<IMapper>();
        }

        [Fact]
        public void SimpleReadEmpty()
        {
            //Can it even get started???
            var thing = provider.GetBasicAsync(new UserSearch()).Result;
            Assert.Empty(thing); //There should be nothing.
        }

        [Fact]
        public void SimpleWrite()
        {
            provider.WriteAsync(new [] {GetBasicUser()}).Wait();
        }

        [Fact]
        public void ReadWrite()
        {
            var user = GetBasicUser();
            provider.WriteAsync(new [] {user}).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            Assert.Single(result);
            Assert.Equal(result.First().username, user.username);
        }

        [Fact]
        public void ReadWriteID()
        {
            var user = GetBasicUser();
            provider.WriteAsync(new [] {user}, false).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            Assert.Equal(0, user.id); //ID should still be 0 with a false.

            user = GetBasicUser();
            provider.WriteAsync(new [] {user}, true).Wait(); //Now actually put the id
            Assert.True(user.id > 0); //user ID should be nonzero
        }

        [Fact]
        public void MultiWriteOrdered()
        {
            const int count = 100;
            var users = Enumerable.Repeat(0, count).Select(x => GetBasicUser()).ToList(); //Lazy evaluation is funny
            provider.WriteAsync(users).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            Assert.Equal(result.Count(), users.Count());

            //This asserts the order is the same and that the elements were all written individually,
            //but not pure equality yet.
            for(int i = 0; i < result.Count(); i++)
            {
                var r = result.ElementAt(i);
                var u = result.ElementAt(i);
                Assert.Equal(i + 1, r.id);
                Assert.Equal(i + 1, u.id);
                Assert.Equal(r.username, u.username);
            }
        }

        [Fact]
        public void MultiWriteCastEqual()
        {
            const int count = 100;
            var users = Enumerable.Repeat(0, count).Select(x => GetBasicUser()).ToList();
            provider.WriteAsync(users).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            var basicUsers = users.Select(x => mapper.Map<BasicUser>(x));

            AssertResultsEqual(basicUsers, result);
        }

        [Fact]
        public void SimpleExpand()
        {
            var user = GetBasicUser();
            provider.WriteAsync(new [] {user}).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            Assert.Single(result);
            var super = provider.ExpandAsync(result).Result;
            Assert.Equal(user, super.First()); //EVERYTHING, including email, should be the same.
            //The above should've checked this but I WANT TO MAKE SURE
            Assert.Equal(user.email, super.First().email);
            Assert.Equal(user.passwordHash, super.First().passwordHash);
        }
    }
}
