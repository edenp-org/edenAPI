using WebApplication3.Dao;
using WebApplication3.Models.DB;

namespace WebApplication3.Biz;

public class CollectionBiz
{
    CollectionDao collectionDao = new CollectionDao();
    WorkDao workDao = new WorkDao();
    public void AddNewCollection(Collection collection)
    {
        collectionDao.AddNewCollection(collection);
    }

    public long GetMaxCode()
    {
        var maxCode = collectionDao.GetMaxCodeCollection();
        long newCode = 0;
        if (maxCode != null && maxCode.Code != 0)
        {
            newCode = (maxCode.Code + 1); // 生成新的 Code，从1开始递增
        }
        else
        {
            newCode = 1; // 如果没有标签，初始化为1
        }

        return newCode;
    }

    public Collection GetCollectionByCode(long code)
    {
        return collectionDao.GetCollectionByCode(code);
    }

    public List<Work> GetWorkByCollectionCode(long collectionCode)
    {
        return workDao.GetWorkByCollectionCode(collectionCode);
    }

    public void AddWorkToCollection(long collectionCode, long workCode, long orderCode)
    {
        workDao.AddWorkToCollection(collectionCode, workCode, orderCode);
    }

    public long GetCollectionOrderMax(long collectionCode)
    {
        var maxCode = workDao.GetCollectionOrderMax(collectionCode);

        long newCode = 0;
        if (maxCode != null && maxCode.Code != 0)
        {
            newCode = (maxCode.Code + 1); // 生成新的 Code，从1开始递增
        }
        else
        {
            newCode = 1; // 如果没有标签，初始化为1
        }
        return newCode;
    }
}