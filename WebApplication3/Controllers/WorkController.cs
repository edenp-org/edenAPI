using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Models.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkController(IWebHostEnvironment env) : ControllerBase
    {
        private static readonly object Lock = new object();


        // 添加嵌套请求模型
        public class ReleaseWorkRequest
        {
            [Required(ErrorMessage = "请求数据不能为空！")] public WorkData Data { get; set; }

            public class WorkData
            {
                [Required(ErrorMessage = "请输入标题！")] public string Title { get; set; }

                [Required(ErrorMessage = "请输入介绍！")] public string Description { get; set; }

                [Required(ErrorMessage = "请输入内容！")] public string Content { get; set; }

                [Required(ErrorMessage = "请至少添加一个标签！")] public List<string> Tags { get; set; }
                [Required(ErrorMessage = "Code不可为空！")] public long Code { get; set; }

            }
        }

        /// <summary>
        /// 发布作品
        /// </summary>
        /// <param name="pairs">入参</param>
        /// <returns>返回发布结果</returns>
        [Authorize(false), HttpPost("ReleaseWork")]
        public Dictionary<string, object> ReleaseWork([FromBody] ReleaseWorkRequest request)
        {
            var dic = new Dictionary<string, object>();

            // 模型验证
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage ?? "参数验证失败";
                throw new CustomException(errorMessage);
            }
            // 从嵌套对象中获取数据
            var data = request.Data;
            var title = data.Title;
            var description = data.Description;
            var content = data.Content;
            var tags = data.Tags;
            var code = data.Code;

            // 获取用户信息
            var userId = HttpContext.Items["UserId"]?.ToString();
            var Uname = HttpContext.Items["Uname"]?.ToString();
            var UCode = HttpContext.Items["Code"]?.ToString();

            if (string.IsNullOrEmpty(userId)) throw new CustomException("用户未授权！");
            if (!long.TryParse(UCode, out long UCodeLong)) throw new CustomException("用户未授权");
            // 处理标签
            var tagList = tags;
            var tagBiz = new TagBiz();
            var tagCodes = new ConcurrentBag<long>();

            tagList.ForEach(tagName =>
            {
                if (string.IsNullOrEmpty(tagName)) throw new CustomException("标签不能为空！");
                var tag = tagBiz.GetTagByName(tagName);
                if (tag == null) throw new CustomException($"标签“{tagName}”不存在！");
                tagCodes.Add(tag.Code);
            });

            // 获取用户信息
            var userBiz = new UserBiz();
            var workBiz = new WorkBiz();
            var user = userBiz.GetUserByUname(Uname);
            Work work;
            if (code == 0)
            {
                // 添加作品
                lock (Lock)
                {
                    work = workBiz.AddWork(new Work
                    {
                        Title = title,
                        Description = description,
                        AuthorCode = UCodeLong,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        AuthorName = user.Username,
                        Tags = tagList.ToJsonString(),
                        Code = workBiz.GetNewWorkCode()
                    });
                }

                // 关联作品和标签
                tagBiz.AddWorkAndTag(tagCodes.ToList(), work.Code);

                // 保存作品内容到文件
                var root = Path.Combine(env.WebRootPath, "Work", UCodeLong.ToString());
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                System.IO.File.WriteAllText(Path.Combine(root, work.Code + ".TXT"), content);
            }
            else
            {
                work = workBiz.GetWorkByGetWorkCode(code);
                if (work == null) throw new CustomException("没有该作品！");
                work = workBiz.UpdateWork(code, title, description);

                tagBiz.RemoveAllAssociatedTags(work.Code);

                // 关联作品和标签
                tagBiz.AddWorkAndTag(tagCodes.ToList(), work.Code);

                var root = Path.Combine(env.WebRootPath, "Work", UCodeLong.ToString());
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                System.IO.File.WriteAllText(Path.Combine(root, work.Code + ".TXT"), content);
            }

            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", new
            {
                work.Code,
                work.Title,
                work.Description,
                work.CreatedAt,
                work.UpdatedAt,
                work.AuthorCode,
                work.AuthorName,
            });
            return dic;
        }

        /// <summary>
        /// 获取作品
        /// </summary>
        /// <param name="workCode">作品ID</param>
        /// <returns>返回作品信息</returns>
        [HttpGet("GetWork")]
        public Dictionary<string, object> GetWork(long workCode = 0)
        {
            var dic = new Dictionary<string, object>();

            var workBiz = new WorkBiz();
            var work = workBiz.GetWorkByGetWorkCode(workCode);
            if (work == null) throw new CustomException("未查询到数据！");
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", new { work, url = Path.Combine("Work", work.AuthorCode.ToString(), work.Id.ToString() + ".TXT") });
            return dic;
        }

        /// <summary>
        /// 获取自己喜欢的标签的文章
        /// </summary>
        /// <param name="page">第几页</param>
        /// <param name="pageSize">每页几个</param>
        /// <returns>返回作品信息</returns>
        [Authorize(false), HttpGet("GetArticlesByUserFavoriteTags")]
        public Dictionary<string, object> GetArticlesByUserFavoriteTags(int page = 0, int pageSize = 0)
        {
            var dic = new Dictionary<string, object>();

            // 获取用户Code
            var uCode = HttpContext.Items["Code"]?.ToString();
            if (string.IsNullOrEmpty(uCode) || !long.TryParse(uCode, out long userCode))
            {
                throw new CustomException("用户未授权！");
            }

            var workBiz = new WorkBiz();
            var workList = workBiz.GetArticlesByUserFavoriteTags(userCode, page, pageSize);
            if (workList == null) throw new CustomException("未查询到数据！");
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", workList.Select(a => new
            {
                a.Code,
                a.Tags,
                a.Description
            }));
            return dic;
        }

        /// <summary>
        /// 根据TagCode获取作品
        /// </summary>
        /// <param name="tagCode">TagCode</param>
        /// <param name="page">第几页</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("GetWorksByTagCode")]
        public Dictionary<string, object> GetWorksByTagCode(long tagCode = 0, int page = 0, int pageSize = 0)
        {
            var dic = new Dictionary<string, object>();
            if (tagCode == 0) throw new CustomException("没有标签！");
            var workBiz = new WorkBiz();
            var workList = workBiz.GetWorksByTagCode(tagCode, page, pageSize);
            if (workList == null) throw new CustomException("未查询到数据！");
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", workList.Select(a => new
            {
                a.Code,
                a.Tags,
                a.Description,
                a.Title
            }).ToList());
            return dic;
        }
    }
}
