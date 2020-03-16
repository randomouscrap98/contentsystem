using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Xunit;

namespace Randomous.ContentSystem.test
{
    public class UserProviderTest : BaseEntityProviderTest<UserProvider, User, BasicUser, UserSearch>
    {
        protected override string simpleField(BasicUser thing)
        {
            return thing.username;
        }

        public override User MakeBasic()
        {
            return new User()
            {
                createDate = DateTime.Now,
                username = DateTime.Now.Ticks.ToString(),
                email = $"thing{DateTime.Now.Ticks}@something.com",
                passwordHash = $"apasswordhash_{DateTime.Now.Ticks}"
            };
        }

        public override List<User> MakeMany(int count = 100)
        {
            var users = new List<User>();

            for(int i = 0; i < count; i++)
            {
                var user = MakeBasic();
                user.username += (char)('a' + (i % 26));
                user.email = (char)('a' + (i % 26)) + user.email;
                users.Add(user);
            }

            return users;
        }

        public UserProviderTest()
        {
            provider = CreateService<UserProvider>();
        }

        [Fact]
        public void SimpleUsernameSearch()
        {
            var users = MakeMany();
            provider.WriteAsync(users).Wait();

            var search = new UserSearch() { UsernameLike = "%a" };
            var result = provider.GetBasicAsync(search).Result;
            Assert.True(result.Count() > 0 && result.Count() < users.Count);

            search.UsernameLike = "%b";
            result = provider.GetBasicAsync(search).Result;
            Assert.True(result.Count() > 0 && result.Count() < users.Count);

            search.UsernameLike = "nothing";
            result = provider.GetBasicAsync(search).Result;
            Assert.Empty(result);
        }

        [Fact]
        public void SimpleEmailSearch()
        {
            var users = MakeMany();
            provider.WriteAsync(users).Wait();

            var search = new UserSearch() { EmailLike = "a%" };
            var result = provider.GetBasicAsync(search).Result;
            Assert.True(result.Count() > 0 && result.Count() < users.Count);

            search.EmailLike = "b%";
            result = provider.GetBasicAsync(search).Result;
            Assert.True(result.Count() > 0 && result.Count() < users.Count);

            search.EmailLike = "nothing";
            result = provider.GetBasicAsync(search).Result;
            Assert.Empty(result);
        }
    }
}
