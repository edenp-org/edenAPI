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
        private static readonly object _lock = new object();

        public WorkController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// 发布作品
        /// </summary>
        /// <param name="pairs">入参</param>
        /// <returns>返回发布结果</returns>
        [Authorize(false), HttpPost("ReleaseWork")]
        public Dictionary<string, object> ReleaseWork([FromBody] Dictionary<string, object> pairs)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                // 验证输入参数
                if (!pairs.TryGetValue("data", out var dataObj) || string.IsNullOrEmpty(dataObj.ToString()))
                    throw new Exception("没有入参！");
                var data = dataObj.ToString().FromJsonString<Dictionary<string, object>>();
                if (!data.TryGetValue("title", out var title) || string.IsNullOrEmpty(title.ToString()))
                    throw new Exception("请输入标题！");
                if (!data.TryGetValue("description", out var description) ||
                    string.IsNullOrEmpty(description.ToString())) throw new Exception("请输入介绍！");
                if (!data.TryGetValue("content", out var content) || string.IsNullOrEmpty(content.ToString()))
                    throw new Exception("请输入内容！");
                if (!data.TryGetValue("Tags", out var tags) || string.IsNullOrEmpty(tags.ToString()))
                    throw new Exception("没有标签！");

                // 获取用户信息
                var userId = HttpContext.Items["UserId"]?.ToString();
                var Uname = HttpContext.Items["Uname"]?.ToString();
                var UCode = HttpContext.Items["Code"]?.ToString();
                if (string.IsNullOrEmpty(userId)) throw new Exception("用户未授权！");
                if (!long.TryParse(UCode, out long UCodeLong)) throw new Exception("用户未授权");
                // 处理标签
                var tagList = tags.ToString().FromJsonString<List<string>>();
                var tagBiz = new TagBiz();
                var tagCodes = new ConcurrentBag<long>();

                tagList.ForEach(tagName =>
                {
                    if (string.IsNullOrEmpty(tagName)) throw new Exception("标签不能为空！");
                    var tag = tagBiz.GetTagByName(tagName);
                    if (tag == null) throw new Exception($"标签“{tagName}”不存在！");
                    tagCodes.Add(tag.Code);
                });

                // 获取用户信息
                var userBiz = new UserBiz();
                var workBiz = new WorkBiz();
                var user = userBiz.GetUserByUname(Uname);

                // 添加作品
                Work work;
                lock (_lock)
                {
                    work = workBiz.AddWork(new Work
                    {
                        Title = title.ToString(),
                        Description = description.ToString(),
                        AuthorCode = UCodeLong,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        AuthorName = user.Username,
                        Tags = tagList.ToJsonString(),
                        Code = workBiz.GetNewWorkCode()
                    });
                }

                // 关联作品和标签
                tagBiz.AddWorkAndTag(tagCodes.ToList(), work.Code);

                // 保存作品内容到文件
                var root = Path.Combine(_env.WebRootPath, "Work", UCodeLong.ToString());
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

        /// <summary>
        /// 获取作品
        /// </summary>
        /// <param name="workId">作品ID</param>
        /// <returns>返回作品信息</returns>
        [HttpGet("GetWork")]
        public Dictionary<string, object> GetWork(long workId = 0)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var workBiz = new WorkBiz();
                var work = workBiz.GetWorkByGetWorkCode(workId);
                if (work == null) throw new Exception("未查询到数据！");
                dic.Add("status", 200);
                dic.Add("message", "成功");
                dic.Add("data",
                    new { work, url = Path.Combine("Work", work.AuthorCode.ToString(), work.Id.ToString() + ".TXT") });
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
