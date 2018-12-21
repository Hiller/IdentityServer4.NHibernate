﻿using System;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Options;
using IdentityServer4.NHibernate.Services;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.NHibernate.TokenCleanup;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Hosting;
using NHibernate;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for configuring NHibernate-based stores in IdentityServer
    /// </summary>
    public static class IdentityServerBuilderNHibernateExtensions
    {
        /// <summary>
        /// Configures NHibernate-based database support for IdentityServer.
        /// - Adds NHibernate implementation of IClientStore, IResourceStore, and ICorsPolicyService (configuration store)
        /// - Adds NHibernate implementation of IPersistedGrantStore and TokenCleanup (operational store).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="databaseConfiguration">The NHibernate configuration to access the underlying database.</param>
        /// <param name="configurationStoreOptionsAction">The configurations store options action.</param>
        /// <param name="operationalStoreOptionsAction">The operational store options action.</param>
        public static IIdentityServerBuilder AddNHibernateStores(
            this IIdentityServerBuilder builder,
            NHibernate.Cfg.Configuration databaseConfiguration,
            Action<ConfigurationStoreOptions> configurationStoreOptionsAction,
            Action<OperationalStoreOptions> operationalStoreOptionsAction)
        {
            var configStoreOptions = new ConfigurationStoreOptions();
            builder.Services.AddSingleton(configStoreOptions);
            configurationStoreOptionsAction?.Invoke(configStoreOptions);

            var operationalStoreOptions = new OperationalStoreOptions();
            builder.Services.AddSingleton(operationalStoreOptions);
            operationalStoreOptionsAction?.Invoke(operationalStoreOptions);

            builder.AddNHibernatePersistenceSupport(databaseConfiguration, configStoreOptions, operationalStoreOptions);

            // Adds configuration store components
            if (configStoreOptions.EnableConfigurationStoreCache)
            {
                builder.AddCachedConfigurationStore();
            }
            else
            {
                builder.AddConfigurationStore();
            }

            // Adds operational store components.
            builder.AddOperationalStore();

            return builder;
        }

        /// <summary>
        /// Configures NHibernate-based database support for IdentityServer.
        /// - Adds NHibernate implementation of IClientStore, IResourceStore, and ICorsPolicyService (configuration store)
        /// - Adds NHibernate implementation of IPersistedGrantStore, IDeviceFlowStore and TokenCleanup (operational store).
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="databaseConfigurationFunction">Configuration function that configures NHibernate to access the underlying database.</param>
        /// <param name="configurationStoreOptionsAction">The configurations store options action.</param>
        /// <param name="operationalStoreOptionsAction">The operational store options action.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddNHibernateDatabaseStores(
            this IIdentityServerBuilder builder,
            Func<NHibernate.Cfg.Configuration> databaseConfigurationFunction,
            Action<ConfigurationStoreOptions> configurationStoreOptionsAction,
            Action<OperationalStoreOptions> operationalStoreOptionsAction)
        {
            return builder.AddNHibernateStores(
                databaseConfigurationFunction(),
                configurationStoreOptionsAction,
                operationalStoreOptionsAction);
        }

        /// <summary>
        /// Adds an implementation of the IOperationalStoreNotification to IdentityServer.
        /// </summary>
        /// <typeparam name="T">Concrete implementation if the <see cref="IOperationalStoreNotification"/> interface.</typeparam>
        /// <param name="builder">The builder.</param>
        public static IIdentityServerBuilder AddOperationalStoreNotification<T>(
           this IIdentityServerBuilder builder)
           where T : class, IOperationalStoreNotification
        {
            builder.Services.AddTransient<IOperationalStoreNotification, T>();
            return builder;
        }

        /// <summary>
        /// Adds NHibernate core components to the DI system.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="databaseConfiguration">NHibernate database configuration.</param>
        /// <param name="configurationStoreOptions">Configuration store options (needed to configure NHibernate mappings).</param>
        /// <param name="operationalStoreOptions">Operational store options (needed to configure NHibernate mappings).</param>
        private static IIdentityServerBuilder AddNHibernatePersistenceSupport(
            this IIdentityServerBuilder builder,
            NHibernate.Cfg.Configuration databaseConfiguration,
            ConfigurationStoreOptions configurationStoreOptions,
            OperationalStoreOptions operationalStoreOptions)
        {
            // Adds NHibernate mappings
            databaseConfiguration.AddConfigurationStoreMappings(configurationStoreOptions);
            databaseConfiguration.AddOperationalStoreMappings(operationalStoreOptions);

            // Registers NHibernate components
            builder.Services.AddSingleton(databaseConfiguration.BuildSessionFactory());
            builder.Services.AddScoped(provider =>
            {
                var factory = provider.GetService<ISessionFactory>();
                return factory.OpenSession();
            });
            builder.Services.AddScoped(provider =>
            {
                var factory = provider.GetService<ISessionFactory>();
                return factory.OpenStatelessSession();
            });

            return builder;
        }

        /// <summary>
        /// Adds the stores for managing IdentityServer configuration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static IIdentityServerBuilder AddConfigurationStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            return builder;
        }

        /// <summary>
        /// Adds the cache based stores for managing IdentityServer configuration.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static IIdentityServerBuilder AddCachedConfigurationStore(this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();
            builder.AddCorsPolicyCache<CorsPolicyService>();

            return builder;
        }

        /// <summary>
        /// Adds stores and services for managing IdentityServer persisted grants.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static IIdentityServerBuilder AddOperationalStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<TokenCleanup>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();
            builder.Services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            return builder;
        }
    }
}