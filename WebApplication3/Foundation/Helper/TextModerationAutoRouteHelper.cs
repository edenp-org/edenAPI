using System.Text;
using System.Text.RegularExpressions;
using COSXML;
using COSXML.Auth;
using COSXML.Model.CI;
using TouchSocket.Core;

namespace WebApplication3.Foundation.Helper;

public class TextModerationAutoRouteHelper
{
    string AI_API_KEY= ConfigHelper.GetString("AIKey");

    public TextModerationResponse Examine(string text)
    {
        const string url = "https://api.deepseek.com/chat/completions";
        var textModerationRequest = new TextModerationRequest
        {
            messages = new List<MessagesItem>(),
            model = "deepseek-reasoner",
            stream = false
        };

        // 构建审核请求内容
        var resultTemplate = BuildResultTemplate();
        textModerationRequest.messages.Add(new MessagesItem
        {
            role = "system",
            content = $"你现在是一个审核员,接下来我将给你一个文本，请你告诉我这个文本是否包含淫秽内容、涉及敏感政治内容、是否有AI生成的可能性，如果包含请告诉我包含片段 结果请以 JSON 的形式输出，输出的 JSON 需遵守以下的格式：{resultTemplate.ToJsonString()}"
        });

        textModerationRequest.messages.Add(new MessagesItem
        {
            role = "user",
            content = text
        });

        // 发送请求
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {AI_API_KEY}" }
        };
        var response = HttpHelper.HttpPost(url, textModerationRequest.ToJsonString(), headers);
        var cleanedResponse = CleanMarkdownCode(response);

        return cleanedResponse.FromJsonString<TextModerationResponse>();
    }

    private static Dictionary<string, object> BuildResultTemplate()
    {
        return new Dictionary<string, object>
        {
            { "AdultResultCode", "<是否涉及淫秽内容,0-正常，1-违规>" },
            { "AdultScore", "淫秽内容的违规评分 0-100 ，越高违规概率就越高>" },
            { "AdultContent", new[] { "<淫秽内容,展示原文，不设置数组数量设置上限，需尽量展示完全>" } },
            { "AdultEvaluate", "<淫秽内容的评价，说明违规原因>" },
            { "AdultSuggestion", "<淫秽内容的修改建议>" },
            { "AIResultCode", "<是否AI生成,0-正常，1-违规>" },
            { "AIScore", "AI生成的概率评分 0-100 ，越高违规概率就越高>" },
            { "AIEvaluate", "<AI内容的评价，说明违规原因>" },
            { "PoliticsResultCode", "<是否涉及涉政内容,0-正常，1-违规>" },
            { "PoliticsScore", "敏感涉政内容的违规评分 0-100 ，越高违规概率就越高>" },
            { "PoliticsContent", new[] { "<敏感涉政内容,展示原文，不设置数组数量设置上限，需尽量展示完全>" } },
            { "PoliticsEvaluate", "<敏感涉政内容的评价，说明违规原因>" },
            { "PoliticsSuggestion", "<敏感涉政内容的修改建议>" }
        };
    }

    public static string CleanMarkdownCode(string input)
    {
        const string pattern = @"```\w*\s*([\s\S]*?)```";
        return Regex.Replace(input, pattern, "$1");
    }

    // 内部类定义
    public class TextModerationRequest
    {
        public string model { get; set; }
        public List<MessagesItem> messages { get; set; }
        public bool stream { get; set; }
    }

    public class MessagesItem
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class TextModerationResponse
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public List<ChoicesItem> choices { get; set; }
        public Usage usage { get; set; }
        public string system_fingerprint { get; set; }
    }

    public class ChoicesItem
    {
        public int index { get; set; }
        public Message message { get; set; }
        public string logprobs { get; set; }
        public string finish_reason { get; set; }

    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
        public string reasoning_content { get;set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
        public PromptTokensDetails prompt_tokens_details { get; set; }
        public int prompt_cache_hit_tokens { get; set; }
        public int prompt_cache_miss_tokens { get; set; }
    }

    public class PromptTokensDetails
    {
        public int cached_tokens { get; set; }
    }

    public class TextModerationResponseContent
    {
        public string AdultResultCode { get; set; }
        public string AdultScore { get; set; }
        public List<string> AdultContent { get; set; }
        public string AdultEvaluate { get; set; }
        public string AdultSuggestion { get; set; }
        public string AIResultCode { get; set; }
        public string AIScore { get; set; }
        public string AIEvaluate { get; set; }
        public string PoliticsResultCode { get; set; }
        public string PoliticsScore { get; set; }
        public List<string> PoliticsContent { get; set; }
        public string PoliticsEvaluate { get; set; }
        public string PoliticsSuggestion { get; set; }
    }
}