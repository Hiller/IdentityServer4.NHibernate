﻿namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Options;
    using IdentityModel;
    using IdentityServer4.Models;
    using Xunit;
    using IdentityServer4.NHibernate.Stores;
    using Microsoft.Extensions.Logging;
    using Moq;
    using FluentAssertions;

    public class ResourceStoreFixture : IClassFixture<DatabaseFixture>
    {
        private static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        private static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        public static readonly TheoryData<TestDatabase> TestDatabases = new TheoryData<TestDatabase>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("(local)", "ResourceStore_NH_Test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

        public ResourceStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Existing_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            string testScope1 = "ar1_scope1";
            string testScope2 = "ar1_scope2";

            var testApiResource1 = CreateTestApiResource("test_api_resource1", new[] { testScope1, testScope2 });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(testApiResource1.ToEntity());
                    tx.Commit();
                }
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeAsync(new string[] { testScope1, testScope2 }).Result;
            }

            resources.Count().Should().Be(1);
            resources.FirstOrDefault(x => x.Name == testApiResource1.Name).Should().NotBeNull();
            resources.First().Scopes.Count().Should().Be(2);
            resources.First().Scopes.First().UserClaims.Count().Should().Be(1);
            resources.First().UserClaims.Count().Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Requested_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            string testScope1 = "ar2_scope1";
            string testScope2 = "ar2_scope2";

            var testApiResource1 = CreateTestApiResource("test_api_resource2", new[] { testScope1, testScope2 });
            var testApiResource2 = CreateTestApiResource("test_api_resource3", new[] { "ar3_scope3" });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(testApiResource1.ToEntity());
                    session.Save(testApiResource2.ToEntity());
                    tx.Commit();
                }
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeAsync(new string[] { testScope1, testScope2 }).Result;
            }

            resources.Count().Should().Be(1);
            resources.FirstOrDefault(x => x.Name == testApiResource1.Name).Should().NotBeNull();
            resources.First().Scopes.Count().Should().Be(2);
            resources.First().Scopes.First().UserClaims.Count().Should().Be(1);
            resources.First().UserClaims.Count().Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Not_Retrieve_Api_Resources_With_Unexisting_Scope_Name(TestDatabase testDb)
        {
            string testScope1 = "test_api_resource_scope1";
            string testScope2 = "test_api_resource_scope2";

            var testApiResource = CreateTestApiResource("test_api_resource", new[] { testScope1, testScope2 });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(testApiResource.ToEntity());
                    tx.Commit();
                }
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeAsync(new string[] { "non_existing_scope" }).Result;
            }

            resources.Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Existing_Api_Resource_By_Name(TestDatabase testDb)
        {
            string testScope1 = "ar4_scope1";
            var testApiResource = CreateTestApiResource("test_api_resource4", new[] { testScope1 });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(testApiResource.ToEntity());
                    tx.Commit();
                }
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = store.FindApiResourceAsync(testApiResource.Name).Result;
            }

            resource.Should().NotBeNull();
            resource.Scopes.Count().Should().Be(1);
            resource.Scopes.First().UserClaims.Count().Should().Be(1);
            resource.UserClaims.Count().Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Not_Retrieve_Non_Existing_Api_Resource(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = store.FindApiResourceAsync("non_existing_api_resource").Result;
            }

            resource.Should().BeNull();
        }

        private static IdentityResource CreateTestIdentityResource()
        {
            return new IdentityResource()
            {
                Name = "test_identity_resource",
                DisplayName = "Test Identity Resource",
                Description = "Identity Resource used for testing",
                ShowInDiscoveryDocument = true,
                UserClaims =
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name,
                }
            };
        }

        private static ApiResource CreateTestApiResource(string name, string[] scopes)
        {
            var testApiResource = new ApiResource()
            {
                Name = name,
                ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                Scopes = new List<Scope>(),
                UserClaims =
                {
                    "user_claim_1",
                    "user_claim_2"
                }
            };

            foreach (var scopeToAdd in scopes)
            {
                testApiResource.Scopes.Add(
                    new Scope()
                    {
                        Name = scopeToAdd,
                        UserClaims = { "test_user_claim" }
                    });
            }

            return testApiResource;
        }
    }
}
