using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Dao
{
    public class TagDao
    {
        public Tag AddTag(Tag tag) 
        {
            tag.Id = FreeSqlHelper.Instance.Insert(tag).ExecuteIdentity();
            return tag;
        }
        public Tag GetTagByCode(string code)
        {
            return FreeSqlHelper.Instance.Select<Tag>().Where(t => t.Code == code).ToOne();
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

        public void AddWorkAndTag(List<string> tagId, string workId)
        {
            List<WorkAndTag> workAndTags = new List<WorkAndTag>();
            foreach (var item in tagId)
            {
                workAndTags.Add(new WorkAndTag
                {
                    TagId = item,
                    WorkId = workId
                });
            }
            FreeSqlHelper.Instance.Insert(workAndTags).ExecuteAffrows();
        }

        public Tag GetMaxCodeTag()
        {
            return FreeSqlHelper.Instance.Select<Tag>()
                .OrderByDescending(t => Convert.ToInt32(t.Code))
                .First();
        }

        public bool TagCodeExists(string code)
        {
            return FreeSqlHelper.Instance.Select<Tag>().Where(t => t.Code == code).Any();
        }
    }
}
