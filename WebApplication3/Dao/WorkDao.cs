using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Dao
{
    public class WorkDao
    {
        public Work AddWork(Work work)
        {
            var insertedId = FreeSqlHelper.Instance.Insert(work).ExecuteIdentity();
            work.Id = (int)insertedId;
            return work;
        }

        public Work GetWorkByGetWorkId(string code)
        {
           return  FreeSqlHelper.Instance.Select<Work>().Where(w=>w.Code == id).First();
        }
        public Work GetMaxCodeWork()
        {
            return FreeSqlHelper.Instance.Select<Work>()
                .OrderByDescending(w => Convert.ToInt32(w.Code))
                .First();
        }

    }
}
