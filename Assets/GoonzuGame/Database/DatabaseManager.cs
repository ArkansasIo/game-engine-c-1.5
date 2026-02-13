using System;
// using System.Data;
// using System.Data.SQLite;
// using System.IO;

namespace GoonzuGame.Database
{
    // Stubbed DatabaseManager for non-SQLite builds
    public class DatabaseManager : IDisposable
    {
        public DatabaseManager(string dbPath) { /* SQLite disabled */ }

        public object Query(string sql) { throw new NotImplementedException("SQLite is disabled in this build."); }

        public int Execute(string sql) { throw new NotImplementedException("SQLite is disabled in this build."); }

        public void Dispose() { }
    }
}
