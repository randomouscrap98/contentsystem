using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Randomous.EntitySystem;
using Serilog;
using Xunit;

namespace Randomous.ContentSystem.test
{
    public class UnitTestBase
    {
        public IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();
            var identifiers = new IdentifierKeys();
            var keys = identifiers.GetKeys();
            var mapperConfig = new AutoMapper.MapperConfiguration((cfg) => 
            {
                cfg.AddProfile<SearchProfile>();
                cfg.AddProfile<ContentProfile>(); //(new ContentProfile(keys));
            });
            services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration().WriteTo.File($"{GetType()}.txt").CreateLogger()));
            services.AddTransient<IEntityProvider, EntityProviderMemory>();
            services.AddSingleton(keys);
            services.AddSingleton(mapperConfig.CreateMapper());
            return services;
        }

        public T CreateService<T>()
        {
            var services = CreateServices();
            var provider = services.BuildServiceProvider();
            return (T)ActivatorUtilities.GetServiceOrCreateInstance(provider, typeof(T));
        }

        [Fact]
        public void TestCreateService()
        {
            var provider = CreateService<UserProvider>();
            Assert.NotNull(provider);
        }

        [Fact]
        public void AndedSearch()
        {
            var provider = CreateService<UserProvider>();
            var list = new List<int> () { 1, 2, 3 };
            Assert.True(provider.AndedSearch(list, new[] {1,2,3}).SequenceEqual(new[] {1,2,3}));
            Assert.True(provider.AndedSearch(list, new[] {1,3}).SequenceEqual(new[] {1,3}));
            Assert.True(provider.AndedSearch(list, new[] {2,5}).SequenceEqual(new[] {2}));
        }

        protected void AssertResultsEqual<T>(IEnumerable<T> expected, IEnumerable<T> result)
        {
            Assert.Equal(expected.Count(), result.Count());
            Assert.Equal(expected.ToHashSet(), result.ToHashSet());
        }
    }
}