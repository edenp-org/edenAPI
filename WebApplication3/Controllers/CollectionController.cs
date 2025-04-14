using Microsoft.AspNetCore.Mvc;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Controllers;

[Route("Collection")]
[ApiController]
public class CollectionController: Controller
{

    private readonly IWebHostEnvironment _env;
    private static readonly object _lock = new object();

    public CollectionController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public class AddNewCollectionRequest
    {
        public AddNewCollectionRequestData data { get; set; }
        public class AddNewCollectionRequestData
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }

    [Authorize(false), HttpPost("AddNewCollection")]
    public Dictionary<string,object> AddNewCollection(AddNewCollectionRequest request)
    {
        var dic = new Dictionary<string, object>();
        try
        {
            // 检查输入参数
            if (request == null) throw new Exception("没有入参！");

            if (string.IsNullOrEmpty(request.data.Name)) throw new Exception("没有合集名称！");
            if (string.IsNullOrEmpty(request.data.Description)) throw new Exception("没有合集简介！");

            var user =  UserHelper.GetUserFromContext(HttpContext);

            CollectionBiz collectionBiz = new CollectionBiz();
            lock (_lock)
            {
                collectionBiz.AddNewCollection(new Collection()
                {
                    Name = request.data.Name,
                    CreatedAt = DateTime.Now,
                    Description = request.data.Description,
                    Code = collectionBiz.GetMaxCode(),
                    UName = user.Username,
                    UCode = user.Code,
                });
            }

            dic.Add("status", 200);
            dic.Add("message", "成功");
        }
        catch (Exception e)
        {
            dic.Add("status", 400);
            dic.Add("message", e.Message);
        }

        return dic;
    }

    [Authorize(false), HttpPost("AddWorkToCollection")]
    public Dictionary<string, object> AddWorkToCollection(Dictionary<string, object> pairs)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        try
        {
            // 检查输入参数
            if (!pairs.TryGetValue("data", out object dataObj)) throw new Exception("没有入参！");
            var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
            if (!data.TryGetValue("CollectionCode", out string collectionCode)) throw new Exception("没有合集代码");
            if (!data.TryGetValue("WorkCode", out string workCode)) throw new Exception("没有作品代码");
            if (!data.TryGetValue("CollectionOrder", out string CollectionOrder)) CollectionOrder = "0";
            var user = UserHelper.GetUserFromContext(HttpContext);
            CollectionBiz collectionBiz = new CollectionBiz();
            WorkBiz workBiz = new WorkBiz();
            if (workBiz.GetWorkByGetWorkCode(long.Parse(workCode)) == null) throw new Exception("没有查询到作品");
            lock (_lock)
            {
                collectionBiz.AddWorkToCollection(long.Parse(collectionCode), long.Parse(workCode), collectionBiz.GetCollectionOrderMax(long.Parse(collectionCode)));
            }

            dic.Add("status", 200);
            dic.Add("message", "成功");
        }
        catch (Exception ex)
        {
            dic.Add("status", 400);
            dic.Add("message", ex.Message);
        }
        return dic;
    }

    [HttpGet("GetWorkByCollectionCode")]
    public Dictionary<string, object> GetWorkByCollectionCode(long collectionCode)
    {
        var dic = new Dictionary<string, object>();
        try
        {
            if (collectionCode == 0) throw new Exception("没有合集代码！");
            var workBiz = new WorkBiz();
            CollectionBiz collectionBiz = new CollectionBiz();
            var collection = collectionBiz.GetCollectionByCode(collectionCode);
            var workList = collectionBiz.GetWorkByCollectionCode(collectionCode);
            if (workList == null) throw new Exception("未查询到数据！");
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", workList.Select(a => new
            {
                a.Code,
                a.Tags,
                a.Description,
                a.CollectionOrder
            }).ToList());
        }
        catch (Exception e)
        {
            dic.Add("status", 400);
            dic.Add("message", e.Message);
        }
        return dic;
    }
}