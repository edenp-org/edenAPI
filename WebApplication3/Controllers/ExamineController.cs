using Microsoft.AspNetCore.Mvc;
using TouchSocket.Core;
using WebApplication3.Foundation.Exceptions;
using WebApplication3.Foundation.Helper;

namespace WebApplication3.Controllers;

[Route("[controller]")]
[ApiController]
public class ExamineController: ControllerBase
{
    [HttpPost("GetExamineResult")]
    public Dictionary<string,object> GetExamineResult(Dictionary<string,object> pairs)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        try
        {
            if (!pairs.TryGetValue("data", out object dataObj) || string.IsNullOrEmpty(dataObj.ToString()))
                throw new CustomException("没有入参！");
            Dictionary<string, object> data = dataObj.ToString().FromJsonString<Dictionary<string, object>>();
            if (!data.TryGetValue("content", out object content) || string.IsNullOrEmpty(content.ToString()))
                throw new CustomException("请输入内容！");
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
        catch (CustomException ex)
        {
            dic.Add("status", 400);
            dic.Add("message", ex.Message);
        }
        catch (Exception ex)
        {
            dic.Add("status", 400);
            dic.Add("message", "系统错误！错误代码:" + ex.HResult);
        }

        return dic;
    }

}