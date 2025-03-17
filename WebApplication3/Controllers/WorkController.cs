using Microsoft.AspNetCore.Mvc;
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
                if (!pairs.TryGetValue("data", out object dataObj)) throw new Exception("没有入参！");
                var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
                if (!data.TryGetValue("title", out string title)) throw new Exception("请输入标题！");
                if (!data.TryGetValue("description", out string description)) throw new Exception("请输入介绍！");
                if (!data.TryGetValue("content", out string content)) throw new Exception("请输入内容！");

                var userId = HttpContext.Items["UserId"]?.ToString();
                if (string.IsNullOrEmpty(userId)) throw new Exception("用户未授权！");

                WorkBiz workBiz = new WorkBiz();
                var work = workBiz.AddWork(new Work
                {
                    Title = title,
                    Description = description,
                    AuthorId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
                string root = Path.Combine(_env.WebRootPath, "Work", userId);
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                System.IO.File.WriteAllText(Path.Combine(root, work.Id + ".TXT"), content);

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
