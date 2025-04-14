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
            // ����������
            if (request == null) throw new Exception("û����Σ�");

            if (string.IsNullOrEmpty(request.data.Name)) throw new Exception("û�кϼ����ƣ�");
            if (string.IsNullOrEmpty(request.data.Description)) throw new Exception("û�кϼ���飡");

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
            dic.Add("message", "�ɹ�");
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
            // ����������
            if (!pairs.TryGetValue("data", out object dataObj)) throw new Exception("û����Σ�");
            var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
            if (!data.TryGetValue("CollectionCode", out string collectionCode)) throw new Exception("û�кϼ�����");
            if (!data.TryGetValue("WorkCode", out string workCode)) throw new Exception("û����Ʒ����");
            if (!data.TryGetValue("CollectionOrder", out string CollectionOrder)) CollectionOrder = "0";
            var user = UserHelper.GetUserFromContext(HttpContext);
            CollectionBiz collectionBiz = new CollectionBiz();
            WorkBiz workBiz = new WorkBiz();
            if (workBiz.GetWorkByGetWorkCode(long.Parse(workCode)) == null) throw new Exception("û�в�ѯ����Ʒ");
            lock (_lock)
            {
                collectionBiz.AddWorkToCollection(long.Parse(collectionCode), long.Parse(workCode), collectionBiz.GetCollectionOrderMax(long.Parse(collectionCode)));
            }

            dic.Add("status", 200);
            dic.Add("message", "�ɹ�");
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
            if (collectionCode == 0) throw new Exception("û�кϼ����룡");
            var workBiz = new WorkBiz();
            CollectionBiz collectionBiz = new CollectionBiz();
            var collection = collectionBiz.GetCollectionByCode(collectionCode);
            var workList = collectionBiz.GetWorkByCollectionCode(collectionCode);
            if (workList == null) throw new Exception("δ��ѯ�����ݣ�");
            dic.Add("status", 200);
            dic.Add("message", "�ɹ�");
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