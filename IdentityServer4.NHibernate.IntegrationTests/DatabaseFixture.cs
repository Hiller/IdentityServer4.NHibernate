namespace IdentityServer4.NHibernate.IntegrationTests
{
    using System;
    using System.Collections.Generic;

    public class DatabaseFixture : IDisposable
    {
        public List<TestDatabase> TestDatabases;

        public void Dispose()
        {
            foreach (var db in TestDatabases)
            {
                // Database objects are dropped after dispose of the Session Factory
                db.SessionFactory.Dispose();
                // Drops the physical database
                db.Drop();
            }
        }
    }
}