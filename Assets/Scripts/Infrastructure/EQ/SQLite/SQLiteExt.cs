namespace Infrastructure.Lantern.SQLite
{
    public static class SqlMethods
    {
        // Stub to allow sqlite-net `like` queries when using linq syntax.
        // https://github.com/guillaume86/data-linq/blob/master/Mindbox.Data.Linq/SqlClient/SqlMethods.cs#L567
        // Usage:
        // Table<Items>().Where(x => SQLiteExt.SqlMethods.Like(x.Name, name))
        public static bool Like(string matchExpression, string pattern)
        {
            throw new System.NotImplementedException();
        }
    }
}
