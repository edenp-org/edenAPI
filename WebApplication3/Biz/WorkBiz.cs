using WebApplication3.Dao;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz
{
    public class WorkBiz
    {
        WorkDao workDao = new WorkDao();

        public Work AddWork(Work work)
        {
            return workDao.AddWork(work);
        }
    }
}
