using WebApplication3.Dao;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz
{
    public class TagBiz
    {
        TagDao tagDao = new TagDao();

        public Tag AddTag(Tag tag)
        {
            return tagDao.AddTag(tag);
        }

        public Tag GetTagByCode(long code)
        {
            return tagDao.GetTagByCode(code);
        }

        public Tag GetTagByName(string name)
        {
            return tagDao.GetTagByName(name);
        }

        public List<Tag> GetTagByFuzzyName(string name)
        {
            return tagDao.GetTagByFuzzyName(name);
        }

        public void AddWorkAndTag(List<long> tagId, long workId)
        {
            tagDao.AddWorkAndTag(tagId, workId);
        }

        public List<Tag> GetAllTag(int page = 0, int pageSize = 0)
        {
            return tagDao.GetAllTag(page, pageSize);
        }
        public long GetNewTagCode()
        {
            long newCode = 0;
            var maxCodeTag = tagDao.GetMaxCodeTag();
            if (maxCodeTag != null && maxCodeTag.Code != 0)
            {
                newCode = (maxCodeTag.Code + 1); // 生成新的 Code，从1开始递增
            }
            else
            {
                newCode = 1; // 如果没有标签，初始化为1
            }

            return newCode;
        }

    }
}
