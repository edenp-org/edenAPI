using Microsoft.AspNetCore.Mvc;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Exceptions;
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

    // 请求类，用于接收添加新合集的请求数据
    public class AddNewCollectionRequest
    {
        public AddNewCollectionRequestData data { get; set; }
        public class AddNewCollectionRequestData
        {
            public string Name { get; set; } // 合集名称
            public string Description { get; set; } // 合集描述
        }
    }

    /// <summary>
    /// 添加新合集
    /// </summary>
    /// <param name="request">包含合集名称和描述的请求数据</param>
    /// <returns>操作结果的字典</returns>
    [Authorize(false), HttpPost("AddNewCollection")]
    public Dictionary<string,object> AddNewCollection([FromBody]AddNewCollectionRequest request)
    {
        var dic = new Dictionary<string, object>();
        try
        {
            // 检查输入参数
            if (request == null) throw new CustomException("没有入参！");
            if (string.IsNullOrEmpty(request.data.Name)) throw new CustomException("没有合集名称！");
            if (string.IsNullOrEmpty(request.data.Description)) throw new CustomException("没有合集简介！");

            // 获取当前用户信息
            var user = UserHelper.GetUserFromContext(HttpContext);

            CollectionBiz collectionBiz = new CollectionBiz();
            lock (_lock)
            {
                // 添加新合集
                collectionBiz.AddNewCollection(new Collection()
                {
                    Name = request.data.Name,
                    CreatedAt = DateTime.UtcNow,
                    Description = request.data.Description,
                    Code = collectionBiz.GetMaxCode(),
                    UName = user.Username,
                    UCode = user.Code,
                });
            }

            dic.Add("status", 200); // 成功状态
            dic.Add("message", "成功");
        }
        catch (CustomException e)
        {
            dic.Add("status", 400);
            dic.Add("message", e.Message);
        }
        catch (Exception e)
        {
            dic.Add("status", 400);
            dic.Add("message", "系统错误！错误代码！" + e.HResult);
        }
        return dic;
    }

    /// <summary>
    /// 将作品添加到合集
    /// </summary>
    /// <param name="pairs">包含合集代码、作品代码和排序的字典</param>
    /// <returns>操作结果的字典</returns>
    [Authorize(false), HttpPost("AddWorkToCollection")]
    public Dictionary<string, object> AddWorkToCollection([FromBody]Dictionary<string, object> pairs)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        try
        {
            // 检查输入参数
            if (!pairs.TryGetValue("data", out object dataObj)) throw new CustomException("没有入参！");
            var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
            if (!data.TryGetValue("CollectionCode", out string collectionCode)) throw new CustomException("没有合集代码");
            if (!data.TryGetValue("WorkCode", out string workCode)) throw new CustomException("没有作品代码");
            if (!data.TryGetValue("CollectionOrder", out string CollectionOrder)) CollectionOrder = "0";

            // 获取当前用户信息
            var user = UserHelper.GetUserFromContext(HttpContext);

            CollectionBiz collectionBiz = new CollectionBiz();
            WorkBiz workBiz = new WorkBiz();

            // 检查作品是否存在
            if (workBiz.GetWorkByGetWorkCode(long.Parse(workCode)) == null) throw new CustomException("没有查询到作品");

            lock (_lock)
            {
                // 添加作品到合集
                collectionBiz.AddWorkToCollection(long.Parse(collectionCode), long.Parse(workCode), collectionBiz.GetCollectionOrderMax(long.Parse(collectionCode)));
            }

            dic.Add("status", 200); // 成功状态
            dic.Add("message", "成功");
        }
        catch (CustomException e)
        {
            dic.Add("status", 400);
            dic.Add("message", e.Message);
        }
        catch (Exception e)
        {
            dic.Add("status", 400);
            dic.Add("message", "系统错误！错误代码！" + e.HResult);
        }
        return dic;
    }

    /// <summary>
    /// 根据合集代码获取作品列表
    /// </summary>
    /// <param name="collectionCode">合集代码</param>
    /// <returns>包含作品列表的字典</returns>
    [HttpGet("GetWorkByCollectionCode")]
    public Dictionary<string, object> GetWorkByCollectionCode(long collectionCode)
    {
        var dic = new Dictionary<string, object>();
        try
        {
            // 检查输入参数
            if (collectionCode == 0) throw new CustomException("没有合集代码！");

            var workBiz = new WorkBiz();
            CollectionBiz collectionBiz = new CollectionBiz();

            // 获取合集信息
            var collection = collectionBiz.GetCollectionByCode(collectionCode);

            // 获取作品列表
            var workList = collectionBiz.GetWorkByCollectionCode(collectionCode);
            if (workList == null) throw new CustomException("未查询到数据！");

            dic.Add("status", 200); // 成功状态
            dic.Add("message", "成功");
            dic.Add("data", workList.Select(a => new
            {
                a.Code, // 作品代码
                a.Tags, // 作品标签
                a.Description, // 作品描述
                a.CollectionOrder // 合集排序
            }).ToList());
        }
        catch (CustomException e)
        {
            dic.Add("status", 400);
            dic.Add("message", e.Message);
        }
        catch (Exception e)
        {
            dic.Add("status", 400);
            dic.Add("message", "系统错误！错误代码！" + e.HResult);
        }
        return dic;
    }
}