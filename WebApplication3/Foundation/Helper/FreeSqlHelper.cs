using System.Diagnostics;
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
                //.UseMonitorCommand(cmd => Console.WriteLine($"Sql：{cmd.CommandText}"))
                .Build();
            // 定义 Stopwatch 用于记录时间
            Stopwatch stopwatch = new Stopwatch();

            // 监听 SQL 执行前事件
            _instance.Aop.CurdBefore += (s, e) =>
            {
                stopwatch.Restart(); // 开始计时
                //Console.WriteLine($"[SQL Before] {e.Sql}");
            };

            // 监听 SQL 执行后事件
            _instance.Aop.CurdAfter += (s, e) =>
            {
                stopwatch.Stop(); // 停止计时
                NLogHelper.Debug($"sql 执行时间： {stopwatch.ElapsedMilliseconds} ms | 输出的SQL： {e.Sql}");
            };
        }

        public static IFreeSql Instance => _instance;
    }
}
