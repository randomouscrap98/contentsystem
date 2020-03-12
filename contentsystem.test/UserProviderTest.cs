using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Xunit;

namespace Randomous.ContentSystem.test
{
    public class UserProviderTest : UnitTestBase
    {
        protected UserProvider provider;
        protected IMapper mapper;
        
        public User MakeBasicUser()
        {
            return new User()
            {
                createDate = DateTime.Now,
                username = DateTime.Now.Ticks.ToString(),
                email = $"thing{DateTime.Now.Ticks}@something.com",
                passwordHash = $"apasswordhash_{DateTime.Now.Ticks}"
            };
        }

        public List<User> MakeManyUsers(int count = 100)
        {
            var users = new List<User>();

            for(int i = 0; i < count; i++)
            {
                var user = MakeBasicUser();
                user.username += (char)('a' + (i % 26));
                user.email = (char)('a' + (i % 26)) + user.email;
                users.Add(user);
            }

            return users;
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
            provider.WriteAsync(new [] {MakeBasicUser()}).Wait();
        }

        [Fact]
        public void ReadWrite()
        {
            var user = MakeBasicUser();
            provider.WriteAsync(new [] {user}).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            Assert.Single(result);
            Assert.Equal(result.First().username, user.username);
        }

        [Fact]
        public void ReadWriteID()
        {
            var user = MakeBasicUser();
            provider.WriteAsync(new [] {user}, false).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            Assert.Equal(0, user.id); //ID should still be 0 with a false.

            user = MakeBasicUser();
            provider.WriteAsync(new [] {user}, true).Wait(); //Now actually put the id
            Assert.True(user.id > 0); //user ID should be nonzero
        }

        [Fact]
        public void MultiWriteOrdered()
        {
            var users = MakeManyUsers();
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
            var users = MakeManyUsers();
            provider.WriteAsync(users).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            var basicUsers = users.Select(x => mapper.Map<BasicUser>(x));

            AssertResultsEqual(basicUsers, result);
        }

        [Fact]
        public void SimpleExpand()
        {
            var user = MakeBasicUser();
            provider.WriteAsync(new [] {user}).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            Assert.Single(result);
            var super = provider.ExpandAsync(result).Result;
            Assert.Equal(user, super.First()); //EVERYTHING, including email, should be the same.
        }

        [Fact]
        public void MultiExpand()
        {
            var users = MakeManyUsers();
            provider.WriteAsync(users).Wait();
            var result = provider.GetBasicAsync(new UserSearch()).Result;
            var superResult = provider.ExpandAsync(result).Result;

            AssertResultsEqual(users, superResult);
        }

        [Fact]
        public void SimpleIdSearch()
        {
            var users = MakeManyUsers();
            provider.WriteAsync(users).Wait();

            var search = new UserSearch();
            search.Ids.AddRange(new long[] {1,2,3,4,5,6,7,8,9,10});

            var result = provider.GetBasicAsync(search).Result;

            Assert.Equal(10, result.Count());

            for(int i = 0; i < 10; i++)
                Assert.Equal(i + 1, result.ElementAt(i).id);
        }
            
        [Fact]
        public void SimpleUsernameSearch()
        {
            var users = MakeManyUsers();
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
            var users = MakeManyUsers();
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
