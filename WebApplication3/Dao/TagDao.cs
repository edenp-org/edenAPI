﻿using WebApplication3.Foundation.Helper;
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

        public Tag GetTagByCode(long code)
        {
            return FreeSqlHelper.Instance.Select<Tag>().Where(t => t.Code == code).ToOne();
        }

        public Tag GetTagByName(string name)
        {
            return FreeSqlHelper.Instance.Select<Tag>().Where(t => t.Name.Equals(name)).ToOne();
        }

        public List<Tag> GetAllTags()
        {
            return FreeSqlHelper.Instance.Select<Tag>().ToList();
        }

        public List<Tag> GetTagByFuzzyName(string name)
        {
            return FreeSqlHelper.Instance.Select<Tag>().Where(t => t.Name.Contains(name)).ToList();
        }

        public void AddWorkAndTag(List<long> tagId, long workId)
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
                .OrderByDescending(t => t.Code)
                .First();
        }

        public List<Tag> GetAllTag(int page = 0, int pageSize = 0)
        {
            return FreeSqlHelper.Instance.Select<Tag>()
                .OrderByDescending(t => t.CreatedAt)
                .Page(page, pageSize)
                .ToList();
        }

        public bool TagCodeExists(long code)
        {
            return FreeSqlHelper.Instance.Select<Tag>()
                .Where(t => t.Code == code)
                .Any();
        }
        public void RemoveAllAssociatedTags(long workCode)
        {
            FreeSqlHelper.Instance.Delete<WorkAndTag>()
                .Where(w=>w.WorkId == workCode)
                .ExecuteAffrows();
        }
    }
}
