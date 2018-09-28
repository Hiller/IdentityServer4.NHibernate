﻿namespace IdentityServer4.NHibernate.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer4.Stores;
    using IdentityServer4.Models;
    using IdentityServer4.NHibernate.Entities;
    using IdentityServer4.NHibernate.Extensions;
    using Microsoft.Extensions.Logging;
    using global::NHibernate;
    using global::NHibernate.Criterion;
    using global::NHibernate.Transform;

    /// <summary>
    /// Implementation of the NHibernate-based IResourceStore.
    /// </summary>
    public class ResourceStore : IResourceStore
    {
        private readonly ISession _session;
        private readonly ILogger<ResourceStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session used to retrieve the data.</param>
        /// <param name="logger">The logger.</param>
        public ResourceStore(ISession session, ILogger<ResourceStore> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger;
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        public async Task<Models.ApiResource> FindApiResourceAsync(string name)
        {
            var apiResource = await _session.QueryOver<Entities.ApiResource>()
                .Where(r => r.Name == name)
                .Fetch(r => r.Secrets).Eager
                .Fetch(r => r.Scopes).Eager
                .Fetch(r => r.UserClaims).Eager
                .SingleOrDefaultAsync();

            if (apiResource != null)
            {
                _logger.LogDebug("Found {api} API resource in database", name);
            }
            else
            {
                _logger.LogDebug("Did not find {api} API resource in database", name);
            }

            return await Task.FromResult(apiResource.ToModel());
        }

        /// <summary>
        /// Gets API resources by scope names.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        public async Task<IEnumerable<Models.ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            Entities.ApiResource apiResourceAlias = null;
            var resourcesQuery = _session.QueryOver(() => apiResourceAlias)
                .WithSubquery.WhereExists(
                    QueryOver.Of<ApiScope>()
                        .WhereRestrictionOn(s => s.Name).IsInG(scopeNames)
                        .And(s => s.ApiResource.ID == apiResourceAlias.ID)
                        .Select(s => s.ApiResource)
                )
                .Fetch(ar => ar.Scopes).Eager
                .Fetch(ar => ar.Secrets).Eager
                .Fetch(ar => ar.UserClaims).Eager;

            var results = await resourcesQuery.ListAsync();

            var models = results.Select(x => x.ToModel()).ToArray();

            _logger.LogDebug("Found {scopes} API scopes in database", models.SelectMany(x => x.Scopes).Select(x => x.Name));

            return await Task.FromResult(models);
        }

        /// <summary>
        /// Gets identity resources by scope names.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        public async Task<IEnumerable<Models.IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var resourcesQuery = _session.QueryOver<Entities.IdentityResource>()
                .WhereRestrictionOn(r => r.Name).IsInG(scopeNames)
                .TransformUsing(Transformers.DistinctRootEntity);

            var results = await resourcesQuery.ListAsync();

            _logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            return await Task.FromResult(results.Select(x => x.ToModel()).ToArray());
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        public async Task<Resources> GetAllResourcesAsync()
        {
            var identityResources = _session.QueryOver<Entities.IdentityResource>()
                .Fetch(ir => ir.UserClaims).Eager
                .Future();

            var apiResources = _session.QueryOver<Entities.ApiResource>()
                .Fetch(ar => ar.Secrets).Eager
                .Fetch(ar => ar.Scopes).Eager
                .Fetch(ar => ar.UserClaims).Eager
                .Future();

            var result = new Resources(
                identityResources.Select(ir => ir.ToModel()).ToArray(),
                apiResources.Select(ar => ar.ToModel()).ToArray()
            );

            _logger.LogDebug("Found {scopes} as all scopes in database", 
                result.IdentityResources.Select(x => x.Name)
                    .Union(result.ApiResources.SelectMany(x => x.Scopes)
                .Select(x => x.Name)));

            return await Task.FromResult(result);
        }
    }
}