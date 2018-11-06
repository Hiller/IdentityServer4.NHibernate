﻿using System.Diagnostics;
using IdentityServer4.NHibernate.Database;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Options;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;

namespace IdentityServer4.NHibernate.IntegrationTests
{
    /// <summary>
    /// Methods for creating test databases and related session factories.
    /// </summary>
    internal static class TestDatabaseBuilder
    {
        public static SQLServerTestDatabase SQLServer2012TestDatabase(string serverName, string databaseName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            var connString = $"Data Source={serverName}; Initial Catalog={databaseName}; Integrated Security=SSPi; Application Name=IdentityServer4.NHibernate.IntegrationTests";

            SQLServerTestDatabase testDb = null;

            //ISessionFactory sessionFactory = null;
            try
            {
                var dbConfig = Databases.SqlServer2012()
                    .UsingConnectionString(connString)
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions)
                    .SetProperty(global::NHibernate.Cfg.Environment.Hbm2ddlAuto, "create-drop");

                testDb = new SQLServerTestDatabase(serverName, databaseName, dbConfig);
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb.Drop();
            }

            return testDb;
        }

        internal static SQLiteTestDatabase SQLiteTestDatabase(string fileName, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            var connString = $"Data Source={fileName};Version=3;Pooling=True;";

            SQLiteTestDatabase testDb = null;

            try
            {
                var dbConfig = Databases.SQLite()
                    .UsingConnectionString(connString)
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions)
                    .SetProperty(global::NHibernate.Cfg.Environment.Hbm2ddlAuto, "create-drop");

                testDb = new SQLiteTestDatabase(fileName, dbConfig);
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                testDb.Drop();
            }

            return testDb;
        }

        internal static SQLiteInMemoryTestDatabase SQLiteInMemoryTestDatabase(ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions)
        {
            SQLiteInMemoryTestDatabase testDb = null;

            try
            {
                var dbConfig = Databases.SQLiteInMemory()
                    .AddConfigurationStoreMappings(configurationStoreOptions)
                    .AddOperationalStoreMappings(operationalStoreOptions);

                testDb = new SQLiteInMemoryTestDatabase(dbConfig);
                testDb.Create();
                new SchemaExport(dbConfig).Execute(false, true, false, testDb.ActiveConnection, null);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return testDb;
        }
    }
}
