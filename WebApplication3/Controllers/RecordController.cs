using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;
using WebApplication3.Sundry;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private readonly ILogger<RecordController> _logger;

        public RecordController(ILogger<RecordController> logger)
        {
            _logger = logger;
        }

        //[HttpPost("SetRecord")]
        //public IActionResult SetRecord([FromBody] Dictionary<string, string> pairs)
        //{
        //    try
        //    {
        //        if (pairs == null || !pairs.Any())
        //        {
        //            return BadRequest("输入数据无效。");
        //        }
        //
        //        pairs.TryGetValue("URL", out string URL);
        //        pairs.TryGetValue("UUID", out string UUID);
        //        pairs.TryGetValue("Host", out string Host);
        //
        //        if (string.IsNullOrEmpty(URL) || string.IsNullOrEmpty(UUID) || string.IsNullOrEmpty(Host))
        //        {
        //            return BadRequest("缺少必填字段。");
        //        }
        //
        //        FreeSqlHelper.Instance.Insert(new BrowsingRecord
        //        {
        //            Host = Host,
        //            Url = URL,
        //            VisitTime = DateTime.Now,
        //            UserId = UUID
        //        }).ExecuteAffrows();
        //
        //        return Ok(pairs);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "设置记录时发生错误。");
        //        return StatusCode(StatusCodes.Status500InternalServerError, "内部服务器错误。");
        //    }
        //}
    }
}
