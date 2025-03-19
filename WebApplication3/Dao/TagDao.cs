using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Dao
{
    public class TagDao
    {
        public void AddTag(Tag tag) 
        {
             FreeSqlHelper.Instance.Insert(tag).ExecuteAffrows();
        }
        public Tag GetTagById(int id)
        {
            return FreeSqlHelper.Instance.Select<Tag>().Where(t => t.Id == id).ToOne();
        }

        public Tag GetTagByName(string name)
        {
            return  FreeSqlHelper.Instance.Select<Tag>().Where(t=>t.Name.Equals(name)).ToOne();
        }

        public List<Tag> GetAllTags() 
        {
            return FreeSqlHelper.Instance.Select<Tag>().ToList();
        }
        public List<Tag> GetTagByFuzzyName(string name)
        {
            return FreeSqlHelper.Instance.Select<Tag>().Where(t => t.Name.Contains(name)).ToList();
        }


    }
}
