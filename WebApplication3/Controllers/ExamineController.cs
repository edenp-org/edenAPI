using Microsoft.AspNetCore.Mvc;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;
using WebApplication3.Models.DB;

namespace WebApplication3.Controllers;

[Route("[controller]")]
[ApiController]
public class ExamineController(IWebHostEnvironment env) : ControllerBase
{
    [Authorize(false), HttpPost("GetExamineResult")]
    public Dictionary<string, object> GetExamineResult(Dictionary<string, object> pairs)
    {
        var user = UserHelper.GetUserFromContext(HttpContext);
        var userBiz = new UserBiz();

        if (user.LastExamineTime <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
        {
            userBiz.ResetExamineCount(user.Code);
            user.ExamineCount = 0;
        }

        userBiz.IncreasExamineCount(user.Code);
        if (1000 - user.ExamineCount <= 0) throw new CustomException("审核次数已用完！");

        Dictionary<string, object> dic = new Dictionary<string, object>();
        if (!pairs.TryGetValue("data", out var dataObj) || string.IsNullOrEmpty(dataObj.ToString())) throw new CustomException("没有入参！");
        Dictionary<string, object> data = dataObj.ToString().FromJsonString<Dictionary<string, object>>();

        if (!data.TryGetValue("workcode", out var workCodeObj) || string.IsNullOrEmpty(workCodeObj.ToString()) || !long.TryParse(workCodeObj.ToString(), out long workCode)) throw new CustomException("未获取到文章code！");

        WorkBiz workBiz = new WorkBiz();
        var work = workBiz.GetWorkByGetWorkCode(workCode);
        if (work == null) throw new CustomException("没有该作品！");
        if (work.AuthorCode != user.Code) throw new CustomException("该作品不属于您！");

        var root = Path.Combine(env.WebRootPath, "Work", user.Code.ToString());
        if (!Directory.Exists(root)) Directory.CreateDirectory(root);
        var content = System.IO.File.ReadAllText(Path.Combine(root, work.Code + ".TXT"));

        TextModerationAutoRouteHelper textModerationAutoRouteHelper = new TextModerationAutoRouteHelper();
        TextModerationAutoRouteHelper.TextModerationResponse textModerationResponse = textModerationAutoRouteHelper.Examine(content);


        if (textModerationResponse.choices == null || textModerationResponse.choices.Count == 0) throw new CustomException("未获取到审核结果！");

        ExamineRecordBiz examineRecordBiz = new ExamineRecordBiz();
        examineRecordBiz.Add(new ExamineRecord()
        {
            CreatedAt = DateTime.UtcNow,
            WorkCode = work.Code,
            Result = textModerationResponse.ToJsonString()
        });
        var msgTextModerationResponseContent = textModerationResponse.choices[0].message.content.FromJsonString<TextModerationAutoRouteHelper.TextModerationResponseContent>();
        if (msgTextModerationResponseContent.PoliticsResultCode != 1 && msgTextModerationResponseContent.PoliticsScore > 50 &&
            msgTextModerationResponseContent.AIResultCode != 1 && msgTextModerationResponseContent.AIScore > 50 &&
            msgTextModerationResponseContent.AdultResultCode != 1 && msgTextModerationResponseContent.AdultScore < 50)
            workBiz.ApproveArticleReview(workCode);

        dic.Add("status", 200);
        dic.Add("message", "成功");
            dic.Add("data", msgTextModerationResponseContent);
        return dic;
    }

    [Authorize(false), HttpPost("CheckReviewQuota")]
    public Dictionary<string, object> CheckReviewQuota()
    {
        var user = UserHelper.GetUserFromContext(HttpContext);
        if (user.LastExamineTime <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
        {
            UserBiz userBiz = new UserBiz();
            userBiz.ResetExamineCount(user.Code);
            user.ExamineCount = 0;
        }
        return new Dictionary<string, object>()
        {
            { "status", 200 },
            { "message", "成功" },
            {
                "data", new Dictionary<string, object>()
                {
                    { "Count", 1000-user.ExamineCount }
                }
            }
        };
    }
}