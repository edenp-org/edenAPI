using WebApplication3.Dao;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz;

public class ExamineRecordBiz
{
    private ExamineRecordDao dao = new ExamineRecordDao();
    public void Add(ExamineRecord record)
    {
        dao.Add(record);
    }

    public List<ExamineRecord> GetExamineRecords(long WorkCode)
    {
        return dao.GetExamineRecords(WorkCode);
    }
}