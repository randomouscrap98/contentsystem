using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Xunit;

namespace Randomous.ContentSystem.test
{
    public abstract class BaseEntityProviderTest<T,B,P,S>: UnitTestBase 
        where T : BaseSystemObject, B where B : BaseSystemObject where S : BaseSearch, new() where P : IBasicProvider<T, B, S>
    {
        protected P provider;
        protected IMapper mapper;

        protected abstract string simpleField(B thing);
        

        public abstract T MakeBasic();
        public abstract List<T> MakeMany(int count = 100);

        public BaseEntityProviderTest()
        {
            //provider = CreateService<ContentProvider>();
            mapper = CreateService<IMapper>();
        }

        [Fact]
        public void SimpleReadEmpty()
        {
            //Can it even get started???
            var thing = provider.GetBasicAsync(new S()).Result;
            Assert.Empty(thing); //There should be nothing.
        }

        [Fact]
        public void SimpleWrite()
        {
            provider.WriteAsync(new [] {MakeBasic()}).Wait();
        }

        [Fact]
        public void ReadWrite()
        {
            var content = MakeBasic();
            provider.WriteAsync(new [] {content}).Wait();
            var result = provider.GetBasicAsync(new S()).Result;
            Assert.Single(result);
            Assert.Equal(simpleField(result.First()), simpleField(content));
        }

        [Fact]
        public void ReadWriteID()
        {
            var user = MakeBasic();
            provider.WriteAsync(new [] {user}, false).Wait();
            var result = provider.GetBasicAsync(new S()).Result;
            Assert.Equal(0, user.id); //ID should still be 0 with a false.

            user = MakeBasic();
            provider.WriteAsync(new [] {user}, true).Wait(); //Now actually put the id
            Assert.True(user.id > 0); //user ID should be nonzero
        }

        [Fact]
        public void MultiWriteOrdered()
        {
            var users = MakeMany();
            provider.WriteAsync(users).Wait();
            var result = provider.GetBasicAsync(new S()).Result;
            Assert.Equal(result.Count(), users.Count());

            //This asserts the order is the same and that the elements were all written individually,
            //but not pure equality yet.
            for(int i = 0; i < result.Count(); i++)
            {
                var r = result.ElementAt(i);
                var u = result.ElementAt(i);
                Assert.Equal(i + 1, r.id);
                Assert.Equal(i + 1, u.id);
                Assert.Equal(simpleField(r), simpleField(u));
            }
        }

        [Fact]
        public void MultiWriteCastEqual()
        {
            var users = MakeMany();
            provider.WriteAsync(users).Wait();
            var result = provider.GetBasicAsync(new S()).Result;
            var basicUsers = users.Select(x => mapper.Map<B>(x));

            AssertResultsEqual(basicUsers, result);
        }

        [Fact]
        public void SimpleExpand()
        {
            var user = MakeBasic();
            provider.WriteAsync(new [] {user}).Wait();
            var result = provider.GetBasicAsync(new S()).Result;
            Assert.Single(result);
            var super = provider.ExpandAsync(result).Result;
            Assert.Equal(user, super.First()); //EVERYTHING, including email, should be the same.
        }

        [Fact]
        public void MultiExpand()
        {
            var users = MakeMany();
            provider.WriteAsync(users).Wait();
            var result = provider.GetBasicAsync(new S()).Result;
            var superResult = provider.ExpandAsync(result).Result;

            AssertResultsEqual(users, superResult);
        }

        [Fact]
        public void SimpleIdSearch()
        {
            var users = MakeMany();
            provider.WriteAsync(users).Wait();

            var search = new S();
            search.Ids.AddRange(new long[] {1,2,3,4,5,6,7,8,9,10});

            var result = provider.GetBasicAsync(search).Result;

            Assert.Equal(10, result.Count());

            for(int i = 0; i < 10; i++)
                Assert.Equal(i + 1, result.ElementAt(i).id);
        }
            
    }
}