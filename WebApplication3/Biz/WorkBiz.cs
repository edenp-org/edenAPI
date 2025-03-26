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
        public Work GetWorkByGetWorkId(string code) 
        {
            return workDao.GetWorkByGetWorkId(code);
        }
        public Work GetMaxCodeWork()
        {
            return workDao.GetMaxCodeWork();
        }

        public string GetNewWorkCode()
        {
            var maxCodeWork = GetMaxCodeWork();
            if (maxCodeWork != null && int.TryParse(maxCodeWork.Code, out int maxCode))
            {
                return (maxCode + 1).ToString("D8"); // 生成新的 Code，格式为8位数字
            }
            else
            {
                return "00000001"; // 如果没有作品，初始化为00000001
            }
        }
    }
}
