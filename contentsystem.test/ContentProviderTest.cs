using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Xunit;

namespace Randomous.ContentSystem.test
{
    public class ContentProviderTest : BaseEntityProviderTest<ContentProvider, Content, BasicContent, ContentSearch>
    {
        protected override string simpleField(BasicContent thing)
        {
            return thing.name;
        }

        public ContentProviderTest() : base()
        {
            provider = CreateService<ContentProvider>();
        }

        public override Content MakeBasic()
        {
            return new Content()
            {
                createDate = DateTime.Now,
                name = DateTime.Now.Ticks.ToString(),
                content = $"this thing is {DateTime.Now.Ticks} something.com",
                owner = 5, //BIG WARN! If foreign keys are enabled, this will NOT WORK!
                parent = 5
            };
        }

        public override List<Content> MakeMany(int count = 100)
        {
            var contents = new List<Content>();

            for(int i = 0; i < count; i++)
            {
                var content = MakeBasic();
                content.name += (char)('a' + (i % 26));
                content.content = (char)('a' + (i % 26)) + content.content;
                content.owner = (i % 10) + 1;
                content.parent = (i / 10) + 1;
                contents.Add(content);
            }

            return contents;
        }

        [Fact]
        public void SimpleNameSearch()
        {
            var users = MakeMany();
            provider.WriteAsync(users).Wait();

            var search = new ContentSearch() { NameLike = "%a" };
            var result = provider.GetBasicAsync(search).Result;
            Assert.True(result.Count() > 0 && result.Count() < users.Count);

            search.NameLike = "%b";
            result = provider.GetBasicAsync(search).Result;
            Assert.True(result.Count() > 0 && result.Count() < users.Count);

            search.NameLike = "nothing";
            result = provider.GetBasicAsync(search).Result;
            Assert.Empty(result);
        }
    }
}