using FreeSql;
using WebApplication3.Models.DB;

namespace WebApplication3.Foundation.Helper;


/// <summary>
/// 专用于请求日志的 SQLite FreeSql 单例
/// </summary>
public static class LoggingFreeSqlHelper
{
    private static readonly IFreeSql _instance;

    public static IFreeSql Instance => _instance;

    static LoggingFreeSqlHelper()
    {
        // 日志文件目录
        var baseDir = AppContext.BaseDirectory;
        var logDir = Path.Combine(baseDir, "logs");
        if (!Directory.Exists(logDir))
            Directory.CreateDirectory(logDir);

        // SQLite 数据库文件
        var dbPath = Path.Combine(logDir, "requestlog.db");
        var conn = $"Data Source={dbPath};";

        _instance = new FreeSqlBuilder()
            .UseConnectionString(DataType.Sqlite, conn)
            .UseAutoSyncStructure(true)
            .Build();

        // 仅同步所需实体(可省略, 保留自动同步也行)
        _instance.CodeFirst.SyncStructure(typeof(RequestLog));
    }
}