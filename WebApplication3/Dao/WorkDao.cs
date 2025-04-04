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
                .OrderByDescending(w => Convert.ToInt32(w.Code))
                .First();
        }

        public List<Work> GetArticlesByUserFavoriteTags(long userCode, int page, int pageSize )
        {
            return FreeSqlHelper.Instance.Select<Work>()
                .Where(w => FreeSqlHelper.Instance.Select<UserFavoriteTag>()
                                .Where(uf => uf.UserCode == userCode)
                                .Any(uf => uf.TagCode == w.Code) &&
                            !FreeSqlHelper.Instance.Select<UserDislikedTag>()
                                .Where(ud => ud.UserCode == userCode)
                                .Any(ud => ud.TagCode == w.Code))
                .Page(page, pageSize)
                .ToList();
        }
    }
}
