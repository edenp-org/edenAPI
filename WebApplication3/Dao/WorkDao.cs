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

        public Work GetWorkByGetWorkCode(long code)
        {
            return FreeSqlHelper.Instance.Select<Work>().Where(w => w.Code == code).First();
        }

        public Work GetMaxCodeWork()
        {
            return FreeSqlHelper.Instance.Select<Work>()
                .OrderByDescending(w => w.Code)
                .First();
        }

        public List<Work> GetArticlesByUserFavoriteTags(long userCode, int page, int pageSize)
        {
            return FreeSqlHelper.Instance.Select<Work>()
                .Where(w => FreeSqlHelper.Instance.Select<UserFavoriteTag>()
                                .Where(uf => uf.UserCode == userCode)
                                .Any(uf => uf.TagCode == w.Code) &&
                            !FreeSqlHelper.Instance.Select<UserDislikedTag>()
                                .Where(ud => ud.UserCode == userCode)
                                .Any(ud => ud.TagCode == w.Code))
                .OrderByDescending(d => d.CreatedAt)
                .Page(page, pageSize)
                .ToList();
        }

        public List<Work> GetWorksByTagCode(long tagCode, int page = 0, int pageSize = 0)
        {
            return FreeSqlHelper.Instance.Select<Work>()
                .Where(w => FreeSqlHelper.Instance.Select<WorkAndTag>()
                    .Where(wt => wt.TagId == tagCode)
                    .Any(wt => wt.WorkId == w.Code))
                .OrderByDescending(w => w.CreatedAt) // 排序
                .Page(page, pageSize) // 分页
                .ToList();
        }
    }
}
