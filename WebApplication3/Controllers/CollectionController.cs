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

    // �����࣬���ڽ�������ºϼ�����������
    public class AddNewCollectionRequest
    {
        public AddNewCollectionRequestData data { get; set; }
        public class AddNewCollectionRequestData
        {
            public string Name { get; set; } // �ϼ�����
            public string Description { get; set; } // �ϼ�����
        }
    }

    /// <summary>
    /// ����ºϼ�
    /// </summary>
    /// <param name="request">�����ϼ����ƺ���������������</param>
    /// <returns>����������ֵ�</returns>
    [Authorize(false), HttpPost("AddNewCollection")]
    public Dictionary<string,object> AddNewCollection([FromBody]AddNewCollectionRequest request)
    {
        var dic = new Dictionary<string, object>();
        try
        {
            // ����������
            if (request == null) throw new CustomException("û����Σ�");
            if (string.IsNullOrEmpty(request.data.Name)) throw new CustomException("û�кϼ����ƣ�");
            if (string.IsNullOrEmpty(request.data.Description)) throw new CustomException("û�кϼ���飡");

            // ��ȡ��ǰ�û���Ϣ
            var user = UserHelper.GetUserFromContext(HttpContext);

            CollectionBiz collectionBiz = new CollectionBiz();
            lock (_lock)
            {
                // ����ºϼ�
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

            dic.Add("status", 200); // �ɹ�״̬
            dic.Add("message", "�ɹ�");
        }
        catch (CustomException e)
        {
            dic.Add("status", 400);
            dic.Add("message", e.Message);
        }
        catch (Exception e)
        {
            dic.Add("status", 400);
            dic.Add("message", "ϵͳ���󣡴�����룡" + e.HResult);
        }
        return dic;
    }

    /// <summary>
    /// ����Ʒ��ӵ��ϼ�
    /// </summary>
    /// <param name="pairs">�����ϼ����롢��Ʒ�����������ֵ�</param>
    /// <returns>����������ֵ�</returns>
    [Authorize(false), HttpPost("AddWorkToCollection")]
    public Dictionary<string, object> AddWorkToCollection([FromBody]Dictionary<string, object> pairs)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        try
        {
            // ����������
            if (!pairs.TryGetValue("data", out object dataObj)) throw new CustomException("û����Σ�");
            var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
            if (!data.TryGetValue("CollectionCode", out string collectionCode)) throw new CustomException("û�кϼ�����");
            if (!data.TryGetValue("WorkCode", out string workCode)) throw new CustomException("û����Ʒ����");
            if (!data.TryGetValue("CollectionOrder", out string CollectionOrder)) CollectionOrder = "0";

            // ��ȡ��ǰ�û���Ϣ
            var user = UserHelper.GetUserFromContext(HttpContext);

            CollectionBiz collectionBiz = new CollectionBiz();
            WorkBiz workBiz = new WorkBiz();

            // �����Ʒ�Ƿ����
            if (workBiz.GetWorkByGetWorkCode(long.Parse(workCode)) == null) throw new CustomException("û�в�ѯ����Ʒ");

            lock (_lock)
            {
                // �����Ʒ���ϼ�
                collectionBiz.AddWorkToCollection(long.Parse(collectionCode), long.Parse(workCode), collectionBiz.GetCollectionOrderMax(long.Parse(collectionCode)));
            }

            dic.Add("status", 200); // �ɹ�״̬
            dic.Add("message", "�ɹ�");
        }
        catch (CustomException e)
        {
            dic.Add("status", 400);
            dic.Add("message", e.Message);
        }
        catch (Exception e)
        {
            dic.Add("status", 400);
            dic.Add("message", "ϵͳ���󣡴�����룡" + e.HResult);
        }
        return dic;
    }

    /// <summary>
    /// ���ݺϼ������ȡ��Ʒ�б�
    /// </summary>
    /// <param name="collectionCode">�ϼ�����</param>
    /// <returns>������Ʒ�б���ֵ�</returns>
    [HttpGet("GetWorkByCollectionCode")]
    public Dictionary<string, object> GetWorkByCollectionCode(long collectionCode)
    {
        var dic = new Dictionary<string, object>();
        try
        {
            // ����������
            if (collectionCode == 0) throw new CustomException("û�кϼ����룡");

            var workBiz = new WorkBiz();
            CollectionBiz collectionBiz = new CollectionBiz();

            // ��ȡ�ϼ���Ϣ
            var collection = collectionBiz.GetCollectionByCode(collectionCode);

            // ��ȡ��Ʒ�б�
            var workList = collectionBiz.GetWorkByCollectionCode(collectionCode);
            if (workList == null) throw new CustomException("δ��ѯ�����ݣ�");

            dic.Add("status", 200); // �ɹ�״̬
            dic.Add("message", "�ɹ�");
            dic.Add("data", workList.Select(a => new
            {
                a.Code, // ��Ʒ����
                a.Tags, // ��Ʒ��ǩ
                a.Description, // ��Ʒ����
                a.CollectionOrder // �ϼ�����
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
            dic.Add("message", "ϵͳ���󣡴�����룡" + e.HResult);
        }
        return dic;
    }
}