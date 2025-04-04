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

        public Work GetWorkByGetWorkCode(long code)
        {
            return workDao.GetWorkByGetWorkCode(code);
        }

        public Work GetMaxCodeWork()
        {
            return workDao.GetMaxCodeWork();
        }

        public long GetNewWorkCode()
        {
            var maxCodeWork = GetMaxCodeWork();
            long maxCode = 0;
            if (maxCodeWork != null && maxCodeWork.Code != 0)
            {
                return (maxCode + 1); // 生成新的 Code，格式为8位数字
            }
            else
            {
                return 1; // 如果没有作品，初始化为00000001
            }
        }

        public List<Work> GetArticlesByUserFavoriteTags(long userCode, int page, int pageSize)
        {
            return workDao.GetArticlesByUserFavoriteTags(userCode, page, pageSize);
        }
    }
}
