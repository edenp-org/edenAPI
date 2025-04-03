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
        // 用于确保线程安全的锁对象
        private static readonly object _lock = new object();

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="pairs">包含标签信息的字典</param>
        /// <returns>包含操作结果的字典</returns>
        [Authorize(false), HttpPost("AddTag")]
        public Dictionary<string, object> AddTag(Dictionary<string, object> pairs)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                // 检查输入参数
                if (!pairs.TryGetValue("data", out object dataObj)) throw new Exception("没有入参！");
                var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
                if (!data.TryGetValue("Name", out string Name)) throw new Exception("没有Tag名");
                var userId = HttpContext.Items["UserId"]?.ToString();

                TagBiz tagBiz = new TagBiz();
                // 检查标签是否已存在
                if (tagBiz.GetTagByName(Name) != null) throw new Exception("该Tag已存在");

                Tag tag;
                // 使用锁确保线程安全
                lock (_lock)
                {
                    tag = new Tag()
                    {
                        CreatedAt = DateTime.Now,
                        Name = Name,
                        UId = int.Parse(userId),
                        Code = tagBiz.GetNewTagCode()
                    };
                    tag = tagBiz.AddTag(tag);
                }

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

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="name">标签名称</param>
        /// <param name="code">标签代码</param>
        /// <returns>包含操作结果的字典</returns>
        [HttpGet("GetTag")]
        public Dictionary<string, object> GetTag(string name="", long code = 0)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                TagBiz tagBiz = new TagBiz();
                List<Tag> tag = new List<Tag>();
                // 根据代码或名称获取标签
                if (code != 0)
                {
                    tag.Add(tagBiz.GetTagByCode(code));
                }
                else
                {
                    tag = tagBiz.GetTagByFuzzyName(name);
                }
                dic.Add("status", 200);
                dic.Add("message", "成功");
                dic.Add("data", tag.Select(a => new { a.Code, a.Name, a.CreatedAt }));
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
