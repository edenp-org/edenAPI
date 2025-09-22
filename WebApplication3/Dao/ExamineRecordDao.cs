using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Dao;

public class ExamineRecordDao
{
    public void Add(ExamineRecord record)
    {
        FreeSqlHelper.Instance
            .Insert<ExamineRecord>(record)
            .ExecuteAffrows();
    }
    public List<ExamineRecord> GetExamineRecords(long WorkCode)
    {
        return FreeSqlHelper.Instance
            .Select<ExamineRecord>()
            .Where(t => t.WorkCode == WorkCode)
            .ToList();
    }
}