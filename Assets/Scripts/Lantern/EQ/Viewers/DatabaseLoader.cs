using System.IO;
using Infrastructure.Lantern.SQLite;

namespace Lantern.EQ.Viewers
{
    public class DatabaseLoader
    {
        private SQLiteConnection database;
        public DatabaseLoader(string databasePath)
        {
            if (!LoadDatabase(Path.Combine(databasePath, "lantern_server.db")))
            {
                return;
            }
        }

        private bool LoadDatabase(string databasePath)
        {
            try
            {
                database = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadOnly);
            }
            catch (SQLiteException e)
            {
                string message = string.Empty;

#if UNITY_EDITOR
                message = $"Unable to load database.";
#else
                    message = "Error loading databases.";
#endif
                return false;
            }

            return true;
        }

        public SQLiteConnection GetDatabase()
        {
            return database;
        }

        public void CloseConnections()
        {
            database.Close();
        }
    }
}
