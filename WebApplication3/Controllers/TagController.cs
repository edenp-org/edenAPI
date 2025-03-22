using Azure;
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
    public class TagController : ControllerBase
    {

        [Authorize(false), HttpPost("AddTag")]
        public Dictionary<string, object> AddTag(Dictionary<string,object> pairs)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                if (!pairs.TryGetValue("data", out object dataObj)) throw new Exception("没有入参！");
                var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
                if (!data.TryGetValue("Name", out string Name)) throw new Exception("没有Tag名");
                var userId = HttpContext.Items["UserId"]?.ToString();

                TagBiz tagBiz = new TagBiz();
                if (tagBiz.GetTagByName(Name) != null) throw new Exception("该Tag已存在");
                var tag = tagBiz.AddTag(new Tag()
                {
                    CreatedAt = DateTime.Now,
                    Name = Name,
                    UId = int.Parse(userId)
                    //Description = data["Description"]
                });
                dic.Add("data", tag);
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

        public Dictionary<string, object> GetTag(string name,int id = 0) 
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try 
            {
                TagBiz tagBiz = new TagBiz();
                List<Tag> tag = new List<Tag>();
                if (id == 0)
                {
                    tag.Add(tagBiz.GetTagById(id));
                }
                else
                {
                    tag = tagBiz.GetTagByFuzzyName(name);
                }
                dic.Add("status", 200);
                dic.Add("message", "成功");
                dic.Add("data", tag);
            }
            catch (Exception e)
            {
                dic.Add("status", 400);
                dic.Add("message", e.Message);
            }
            return dic;  
        }
    }
}
