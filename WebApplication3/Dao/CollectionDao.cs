using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Dao;

public class CollectionDao
{
    public void AddNewCollection(Collection collection)
    {
        FreeSqlHelper.Instance.Insert<Collection>(collection).ExecuteAffrows();
    }

    public Collection GetMaxCodeCollection()
    {
        return FreeSqlHelper.Instance.Select<Collection>()
            .OrderByDescending(c => c.Code)
            .First();
    }

    public Collection GetCollectionByCode(long code)
    {
        return FreeSqlHelper.Instance.Select<Collection>()
            .Where(c => c.Code == code)
            .First();
    }
}