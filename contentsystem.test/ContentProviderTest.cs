using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Xunit;

namespace Randomous.ContentSystem.test
{
    public class ContentProviderTest : BaseEntityProviderTest<Content, BasicContent, ContentProvider, ContentSearch> //UnitTestBase
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
                contents.Add(content);
            }

            return contents;
        }
    }
}