using KronoGeo_Api.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.TestUnitaire.Data
{
    /// <summary>
    /// class pour avoir un suivi ici du db context entre les methodes
    /// </summary>
    public class DatabaseFixture : IDisposable
    {
        #region public properties
        public KronoGeoContextMemory Context { get; private set; }
        #endregion

        #region constructor
        public DatabaseFixture()
        {
            Context = new KronoGeoContextMemory();
        }

        #endregion

        #region method Interface 
        public void Dispose()
        {
            Context.Dispose();
        }
        #endregion
    }

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
