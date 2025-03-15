using FreeSql;

namespace WebApplication3.Foundation.Helper
{
    public class FreeSqlHelper
    {
        private static readonly IFreeSql _instance;
        static string IP = ConfigHelper.GetString("DBIP");
        static string DBNAME = ConfigHelper.GetString("DBNAME");
        static string DBUSER = ConfigHelper.GetString("DBUSER");
        static string DBPASSWORD = ConfigHelper.GetString("DBPASSWORD");
        static FreeSqlHelper()
        {
            _instance = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, $"Server={IP};Database={DBNAME};User={DBUSER};Password={DBPASSWORD};")
                .UseAutoSyncStructure(true) // 自动同步实体结构到数据库
                .Build();
        }

        public static IFreeSql Instance => _instance;
    }
}
