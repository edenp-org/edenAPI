﻿using Azure;
using Microsoft.AspNetCore.Mvc;
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
            // 检查输入参数
            if (!pairs.TryGetValue("data", out object dataObj)) throw new CustomException("没有入参！");
            var data = dataObj.ToString().FromJsonString<Dictionary<string, string>>();
            if (!data.TryGetValue("Name", out string Name)) throw new CustomException("没有Tag名");
            var userId = HttpContext.Items["UserId"]?.ToString();

            TagBiz tagBiz = new TagBiz();
            // 检查标签是否已存在
            if (tagBiz.GetTagByName(Name) != null) throw new CustomException("该Tag已存在");

            Tag tag;
            // 使用锁确保线程安全
            lock (_lock)
            {
                tag = new Tag()
                {
                    CreatedAt = DateTime.UtcNow,
                    Name = Name,
                    UId = int.Parse(userId),
                    Code = tagBiz.GetNewTagCode()
                };
                tag = tagBiz.AddTag(tag);
            }

            dic.Add("data", tag);
            dic.Add("status", 200);
            dic.Add("message", "成功");
            return dic;
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="name">标签名称</param>
        /// <param name="code">标签代码</param>
        /// <returns>包含操作结果的字典</returns>
        [HttpGet("GetTag")]
        public Dictionary<string, object> GetTag(string name = "", long code = 0)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
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
            return dic;
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <param name="page">第几页</param>
        /// <param name="pageSize">每页多少</param>
        /// <returns>Tag信息</returns>
        [HttpGet("GetALLTag")]
        public Dictionary<string, object> GetALLTags(int page = 0, int pageSize = 0)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            TagBiz tagBiz = new TagBiz();
            var tags = tagBiz.GetAllTag(page, pageSize);
            //if (tags == null) throw new CustomException("未查询到数据！");
            dic.Add("status", 200);
            dic.Add("message", "成功");
            dic.Add("data", tags.Select(a => new { a.Code, a.Name, a.CreatedAt }));

            return dic;
        }
    }
}
