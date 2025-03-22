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

        public Tag GetTagById(int id)
        {
            return tagDao.GetTagById(id);
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

    }
}
