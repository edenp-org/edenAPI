using Microsoft.AspNetCore.Mvc;
using TouchSocket.Core;
using WebApplication3.Foundation;

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
                
                
                
                dic.Add("URL", _env.WebRootPath);
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
