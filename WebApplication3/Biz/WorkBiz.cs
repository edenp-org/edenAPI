using WebApplication3.Dao;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz
{
    public class WorkBiz
    {
        private readonly IWebHostEnvironment env;
        public WorkBiz()
        {
        }
        public WorkBiz(IWebHostEnvironment env)
        {
            this.env = env;
        }

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
                return maxCodeWork.Code + 1; // 生成新的 Code，格式为8位数字
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

        public List<Work> GetWorksByTagCode(long tagCode , int page = 0, int pageSize = 0)
        {
           return workDao.GetWorksByTagCode(tagCode, page, pageSize);
        }

        public void ApproveArticleReview(long workCode, int IsExamine ,DateTime ExamineDate,bool IsPublished,bool isScheduledRelease,DateTime ScheduledReleaseTime)
        {
            workDao.ApproveArticleReview(workCode, IsExamine, ExamineDate, IsPublished, isScheduledRelease, ScheduledReleaseTime);
        }

        public Work UpdateWork(long workcode,string title,string dscription)
        {
           return workDao.UpdateWork(workcode, title, dscription);
        }

        public (List<Work> Data, long Total) GetWorksByUserCode(long uCode, int pageIndex, int pageSize)
        {
            return workDao.GetWorksByUserCode(uCode, pageIndex, pageSize);
        }
    }
}
