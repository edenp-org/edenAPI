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

        public Tag GetTagByCode(string code)
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
        public void AddWorkAndTag(List<string> tagId, string workId)
        {
            tagDao.AddWorkAndTag(tagId, workId);
        }


        public string GetNewTagCode()
        {
            string newCode;
            do
            {
                var maxCodeTag = tagDao.GetMaxCodeTag();
                if (maxCodeTag != null && int.TryParse(maxCodeTag.Code, out int maxCode))
                {
                    newCode = (maxCode + 1).ToString(); // 生成新的 Code，从1开始递增
                }
                else
                {
                    newCode = "1"; // 如果没有标签，初始化为1
                }
            } while (tagDao.TagCodeExists(newCode)); // 检查 Code 是否唯一

            return newCode;
        }

    }
}
