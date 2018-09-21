﻿namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Extensions;
    using Options;
    using Stores;
    using IdentityServer4.Models;
    using Xunit;
    using Moq;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;

    public class ClientStoreFixture : IClassFixture<DatabaseFixture>
    {
        private static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        private static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        public static readonly TheoryData<TestDatabase> TestDatabases = new TheoryData<TestDatabase>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("(local)", "IdentityServer_NH_Test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

        public ClientStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_If_It_Exists(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testClient.ToEntity());
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Not_Retrieve_Non_Existing_Client(TestDatabase testDb)
        {
            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync("not_existing_client").Result;
            }

            requestedClient.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Grant_Types(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_grant_types",
                ClientName = "Test Client with Grant Types",
                AllowedGrantTypes =
                {
                    "grant_1",
                    "grant_2",
                    "grant_3",
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.AllowedGrantTypes.Count.Should().Be(3);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Client_Secrets(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_client_secrets",
                ClientName = "Test Client with Client Secrets",
                ClientSecrets = new List<Secret>()
                {
                    new Secret("secret1", "secret 1"),
                    new Secret("secret2", "secret 2"),
                    new Secret("secret3", "secret 3"),
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.ClientSecrets.Count.Should().Be(3);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Redirect_Uris(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_redirect_uris",
                ClientName = "Test Client with Redirect Uris",
                RedirectUris =
                {
                    @"http://redirect/uri/1",
                    @"http://redirect/uri/2",
                    @"http://redirect/uri/3"
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.RedirectUris.Count.Should().Be(3);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_PostLogout_Redirect_Uris(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_postlogout_redirect_uris",
                ClientName = "Test Client with PostLogout Redirect Uris",
                PostLogoutRedirectUris =
                {
                    @"http://postlogout/redirect/uri/1",
                    @"http://postlogout/redirect/uri/2",
                    @"http://postlogout/redirect/uri/3"
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.PostLogoutRedirectUris.Count.Should().Be(3);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Allowed_Scopes(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_allowed_scopes",
                ClientName = "Test Client with Allowed Scopes",
                AllowedScopes =
                {
                    "scope1",
                    "scope2",
                    "scope3"
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.AllowedScopes.Count.Should().Be(3);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Provider_Restrictions(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_provider_restrictions",
                ClientName = "Test Client with Provider Restrictions",
                IdentityProviderRestrictions =
                {
                    "restriction_1",
                    "restriction_2",
                    "restriction_3"
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.IdentityProviderRestrictions.Count.Should().Be(3);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Claims(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_claims",
                ClientName = "Test Client with Claims",
                Claims =
                {
                    new Claim("type1", "value1"),
                    new Claim("type2", "value2"),
                    new Claim("type3", "value3")
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.Claims.Count.Should().Be(3);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Allowed_Cors_Origins(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_cors_ordigins",
                ClientName = "Test Client with CORS Origins",
                AllowedCorsOrigins =
                {
                    "*.tld1",
                    "*.tld2",
                    "*.tld3"
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.AllowedCorsOrigins.Count.Should().Be(3);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Properties(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_properties",
                ClientName = "Test Client with Properties",
                Properties =
                {
                    {"prop1", "val1" },
                    {"prop2", "val2" },
                    {"prop3", "val3" }
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.Properties.Count.Should().Be(3);
        }
    }
}
