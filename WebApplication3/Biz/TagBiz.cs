using WebApplication3.Dao;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz
{
    public class TagBiz
    {
        TagDao tagDao = new TagDao();
        public void AddTag(Tag tag)
        {
            tagDao.AddTag(tag);
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
        public void AddWorkAndTag(List<int> tagId, int workId)
        {
            tagDao.AddWorkAndTag(tagId, workId);
        }

    }
}
