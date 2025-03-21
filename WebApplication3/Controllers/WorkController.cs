using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Models.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public WorkController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Authorize(false),HttpPost("ReleaseWork")]
        public Dictionary<string, object> ReleaseWork(Dictionary<string,object> pairs) 
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                if (!pairs.TryGetValue("data", out object dataObj)  || string.IsNullOrEmpty(dataObj.ToString())) throw new Exception("没有入参！");
                var data = dataObj.ToString().FromJsonString<Dictionary<string, object>>();
                if (!data.TryGetValue("title", out object title) || string.IsNullOrEmpty(title.ToString())) throw new Exception("请输入标题！");
                if (!data.TryGetValue("description", out object description) || string.IsNullOrEmpty(description.ToString())) throw new Exception("请输入介绍！");
                if (!data.TryGetValue("content", out object content) || string.IsNullOrEmpty(content.ToString())) throw new Exception("请输入内容！");
                if (!data.TryGetValue("Tags", out object tags) || string.IsNullOrEmpty(tags.ToString())) throw new Exception("没有标签！");

                var userId = HttpContext.Items["UserId"]?.ToString();
                var Uname = HttpContext.Items["Uname"]?.ToString();
                if (string.IsNullOrEmpty(userId)) throw new Exception("用户未授权！");

                var tagList = tags.ToString().FromJsonString<List<string>>();
                TagBiz tagBiz = new TagBiz();

                ConcurrentBag<int> tagIds = new ConcurrentBag<int>();
                
                 
                tagList.ForEach(tagName =>
                {
                    if (string.IsNullOrEmpty(tagName)) throw new Exception("标签不能为空！");

                    var tag = tagBiz.GetTagByName(tagName);
                    if (tag == null) throw new Exception($"{tagName}不存在！");
                    tagIds.Add(tag.Id);
                });
                UserBiz userBiz = new UserBiz();
                WorkBiz workBiz = new WorkBiz();
                var user = userBiz.GetUserByCode(Uname);
                var work = workBiz.AddWork(new Work
                {
                    Title = title.ToString(),
                    Description = description.ToString(),
                    AuthorId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    AuthorName = user.Username,
                    Tags =  tagList.ToJsonString()
                });
                tagBiz.AddWorkAndTag(tagIds.ToList(), work.Id);

                string root = Path.Combine(_env.WebRootPath, "Work", userId);
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                System.IO.File.WriteAllText(Path.Combine(root, work.Id + ".TXT"), content.ToString());

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
    }
}
