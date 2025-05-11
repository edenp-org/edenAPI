using Microsoft.AspNetCore.Mvc;
using TouchSocket.Core;
using WebApplication3.Biz;
using WebApplication3.Foundation;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;

namespace WebApplication3.Controllers;

[Route("[controller]")]
[ApiController]
public class ExamineController : ControllerBase
{
    [Authorize(false),HttpPost("GetExamineResult")]
    public Dictionary<string, object> GetExamineResult(Dictionary<string, object> pairs)
    {
        var user = UserHelper.GetUserFromContext(HttpContext);
        if (user.LastExamineTime <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
        {
            UserBiz userbiz = new UserBiz();
            userbiz.ResetExamineCount(user.Code);
            user.ExamineCount = 0;
        }

        if (1000 - user.ExamineCount <=0) throw new CustomException("审核次数已用完！");

        Dictionary<string, object> dic = new Dictionary<string, object>();
        if (!pairs.TryGetValue("data", out object dataObj) || string.IsNullOrEmpty(dataObj.ToString())) throw new CustomException("没有入参！");
        Dictionary<string, object> data = dataObj.ToString().FromJsonString<Dictionary<string, object>>();
        if (!data.TryGetValue("content", out object content) || string.IsNullOrEmpty(content.ToString())) throw new CustomException("请输入内容！");
        TextModerationAutoRouteHelper textModerationAutoRouteHelper = new TextModerationAutoRouteHelper();
        TextModerationAutoRouteHelper.TextModerationResponse textModerationResponse = textModerationAutoRouteHelper.Examine(content.ToString());
        dic.Add("status", 200);
        dic.Add("message", "成功");
        dic.Add("data", textModerationResponse.choices[0].message.content.FromJsonString<TextModerationAutoRouteHelper.TextModerationResponseContent>());
        textModerationResponse.choices[0].message.content = "";
        textModerationResponse.id = "";
        dic.Add("textModerationResponse", textModerationResponse);
        return dic;
    }

    [Authorize(false), HttpPost("CheckReviewQuota")]
    public Dictionary<string, object> CheckReviewQuota()
    {
        var user = UserHelper.GetUserFromContext(HttpContext);
        if (user.LastExamineTime <= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1))
        {
            UserBiz userbiz = new UserBiz();
            userbiz.ResetExamineCount(user.Code);
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