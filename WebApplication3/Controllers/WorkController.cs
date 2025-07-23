using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

// 作品管理控制器
namespace WebApplication3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkController(IWebHostEnvironment env) : ControllerBase
    {
        // 线程同步锁，防止并发操作导致数据不一致
        private static readonly object Lock = new object();

        // 保存作品的请求模型
        public class SaveWorkRequest
        {
            [Required(ErrorMessage = "请求数据不能为空！")]
            public WorkData Data { get; set; }

            // 作品数据模型
            public class WorkData
            {
                [Required(ErrorMessage = "请输入标题！")]
                public string Title { get; set; }

                [Required(ErrorMessage = "请输入介绍！")]
                public string Description { get; set; }

                [Required(ErrorMessage = "请输入内容！")]
                public string Content { get; set; }

                [Required(ErrorMessage = "请至少添加一个标签！")]
                public List<string> Tags { get; set; }

                [Required(ErrorMessage = "Code不可为空！")]
                public long Code { get; set; }  // 0表示新建，非0表示修改
            }
        }
        /// 发布作品的请求模型
        public class PublisWorkRequest
        {
            [Required(ErrorMessage = "请求数据不能为空！")]
            public PublisWorkData Data { get; set; }

            public class PublisWorkData
            {
                [Required(ErrorMessage = "Code不可为空！")]
                public long Code { get; set; }  // 作品ID

                public bool isScheduledRelease { get; set; } = false;  // 是否定时发布
                public DateTime ScheduledReleaseTime { get; set; } = DateTime.UtcNow;  // 定时发布时间
            }
        }
        /// <summary>
        /// 保存作品（创建或更新）
        /// </summary>
        /// <param name="request">包含作品数据的请求对象</param>
        /// <returns>操作结果</returns>
        [Authorize(false), HttpPost("SaveWork")]
        public Dictionary<string, object> SaveWork([FromBody] SaveWorkRequest request)
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

            // 获取当前用户
            var user =  UserHelper.GetUserFromContext(HttpContext);
            if (user == null) throw new CustomException("用户未授权！");

            // 处理标签验证
            var tagList = tags;
            var tagBiz = new TagBiz();
            var tagCodes = new ConcurrentBag<Tag>();  // 线程安全的集合

            // 验证每个标签是否存在
            tagList.ForEach(tagName =>
            {
                if (string.IsNullOrEmpty(tagName)) throw new CustomException("标签不能为空！");
                var tag = tagBiz.GetTagByName(tagName);
                if (tag == null) throw new CustomException($"标签“{tagName}”不存在！");
                tagCodes.Add(new Tag()
                {
                    Name = tagName,
                    Code = tag.Code
                });  // 收集标签ID
            });

            var workBiz = new WorkBiz();
            Work work;

            // 根据code判断是新增还是修改
            if (code == 0)
            {
                // 使用锁确保新增操作的原子性
                lock (Lock)
                {
                    // 创建新作品
                    work = workBiz.AddWork(new Work
                    {
                        Title = title,
                        Description = description,
                        AuthorCode = user.Code,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        AuthorName = user.Username,
                        Tags = tagCodes.Select(t => new {
                            t.Code,
                            t.Name
                        }).ToList().ToJsonString(),  // 标签列表序列化为JSON
                        Code = workBiz.GetNewWorkCode()  // 生成唯一作品ID
                    });
                }

                // 建立作品与标签的关联
                tagBiz.AddWorkAndTag(tagCodes.Select(a=>a.Code).ToList(), work.Code);

                // 将作品内容保存到文件系统
                var root = Path.Combine(env.WebRootPath, "Work", user.Code.ToString());
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                System.IO.File.WriteAllText(Path.Combine(root, work.Code + ".TXT"), content);
            }
            else  // 修改现有作品
            {
                // 获取现有作品
                work = workBiz.GetWorkByGetWorkCode(code);
                if (work == null) throw new CustomException("没有该作品！");

                // 验证作者权限
                if(work.AuthorCode != user.Code) throw new CustomException("没有权限修改该作品！");

                // 更新作品元数据
                work = workBiz.UpdateWork(code, title, description);

                // 清除旧标签关联
                tagBiz.RemoveAllAssociatedTags(work.Code);

                // 重建标签关联
                tagBiz.AddWorkAndTag(tagCodes.Select(a=>a.Code).ToList(), work.Code);

                // 更新内容文件
                var root = Path.Combine(env.WebRootPath, "Work", user.Code.ToString());
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                System.IO.File.WriteAllText(Path.Combine(root, work.Code + ".TXT"), content);
            }

            // 构造响应
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
        /// 发布作品
        /// </summary>
        /// <param name="request">包含发布参数的请求</param>
        /// <returns>操作结果</returns>
        [Authorize(false), HttpPost("PublishWork")]
        public Dictionary<string, object> PublishWork([FromBody] PublisWorkRequest request)
        {
            var dic = new Dictionary<string, object>();
            long workCode = request.Data.Code;
            if (workCode == 0) throw new CustomException("没有作品ID！");

            var isScheduledRelease = request.Data.isScheduledRelease;
            var scheduledReleaseTime = request.Data.ScheduledReleaseTime;

            // 用户认证
            var user =  UserHelper.GetUserFromContext(HttpContext);
            if (user == null) throw new CustomException("用户未授权！");

            // 获取作品并验证权限
            WorkBiz workBiz = new WorkBiz();
            var work = workBiz.GetWorkByGetWorkCode(workCode);
            if (work == null) throw new CustomException("没有该作品！");
            if (work.AuthorCode != user.Code) throw new CustomException("该作品不属于您！");

            // 处理审核记录
            ExamineRecordBiz examineRecordBiz = new ExamineRecordBiz();
            var root = Path.Combine(env.WebRootPath, "Work", user.Code.ToString());
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            // 读取内容并计算哈希
            string text = System.IO.File.ReadAllText(Path.Combine(root, work.Code + ".TXT"));
            var textSHA = EncryptionHelper.ComputeSHA256(text);

            // 获取或创建审核记录
            var examines = examineRecordBiz.GetExamineRecords(work.Code);
            if (examines == null || examines.Count == 0)
            {
                examines = new List<ExamineRecord>() {
                    new ExamineRecord() {
                        DataSHA265 = textSHA,
                        IsOk = false,
                        CreatedAt = DateTime.UtcNow
                }};
            }

            // 匹配当前内容的审核记录
            var examine= examines.FirstOrDefault(e => e.DataSHA265.Equals(textSHA));

            // 更新作品发布状态
            workBiz.ApproveArticleReview(
                workCode,
                examine?.IsOk == true ? 1 : 0,  // 审核状态
                examine?.CreatedAt ?? DateTime.UtcNow,
                true,  // 设置为已发布
                isScheduledRelease,
                scheduledReleaseTime
            );

            // 返回成功响应
            dic.Add("status", 200);
            dic.Add("message", "成功");
            return dic;
        }

        /// <summary>
        /// 获取作品详情
        /// </summary>
        /// <param name="workCode">作品ID</param>
        /// <returns>作品信息及内容路径</returns>
        [HttpGet("GetWork")]
        public Dictionary<string, object> GetWork(long workCode = 0)
        {
            var dic = new Dictionary<string, object>();

            var workBiz = new WorkBiz();
            var work = workBiz.GetWorkByGetWorkCode(workCode);
            if (work == null) throw new CustomException("未查询到数据！");

            // 构造响应数据
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", new {
                work,
                url = Path.Combine("Work", work.AuthorCode.ToString(), work.Id.ToString() + ".TXT")
            });
            return dic;
        }

        /// <summary>
        /// 获取用户喜欢标签的文章
        /// </summary>
        /// <param name="page">分页页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>文章列表</returns>
        [Authorize(false), HttpGet("GetArticlesByUserFavoriteTags")]
        public Dictionary<string, object> GetArticlesByUserFavoriteTags(int page = 0, int pageSize = 0)
        {
            var dic = new Dictionary<string, object>();

            // 从上下文中获取用户ID
            var uCode = HttpContext.Items["Code"]?.ToString();
            if (string.IsNullOrEmpty(uCode) || !long.TryParse(uCode, out long userCode))
            {
                throw new CustomException("用户未授权！");
            }

            // 获取用户偏好的文章
            var workBiz = new WorkBiz();
            var workList = workBiz.GetArticlesByUserFavoriteTags(userCode, page, pageSize);
            if (workList == null) throw new CustomException("未查询到数据！");

            // 构造精简响应数据
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", workList.Select(a => new
            {
                a.Code,
                a.Tags,
                a.Description,
                a.AuthorName,
                a.AuthorCode

            }));
            return dic;
        }

        /// <summary>
        /// 按标签获取作品
        /// </summary>
        /// <param name="tagCode">标签ID</param>
        /// <param name="page">分页页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>作品列表</returns>
        [HttpGet("GetWorksByTagCode")]
        public Dictionary<string, object> GetWorksByTagCode(long tagCode = 0, int page = 0, int pageSize = 0)
        {
            var dic = new Dictionary<string, object>();
            if (tagCode == 0) throw new CustomException("标签ID无效！");

            var workBiz = new WorkBiz();
            var workList = workBiz.GetWorksByTagCode(tagCode, page, pageSize);
            if (workList == null) throw new CustomException("未查询到数据！");

            // 构造响应数据
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
