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

        // ���������������
        var resultTemplate = BuildResultTemplate();
        textModerationRequest.messages.Add(new MessagesItem
        {
            role = "system",
            content = $"��������һ�����Ա,�������ҽ�����һ���ı����������������ı��Ƿ�����������ݡ��漰�����������ݡ��Ƿ���AI���ɵĿ����ԣ��������������Ұ���Ƭ�� ������� JSON ����ʽ���������� JSON ���������µĸ�ʽ��{resultTemplate.ToJsonString()}"
        });

        textModerationRequest.messages.Add(new MessagesItem
        {
            role = "user",
            content = text
        });

        // ��������
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
            { "AdultResultCode", "<�Ƿ��漰��������,0-������1-Υ��>" },
            { "AdultScore", "�������ݵ�Υ������ 0-100 ��Խ��Υ����ʾ�Խ��>" },
            { "AdultContent", new[] { "<��������,չʾԭ�ģ����������������������ޣ��辡��չʾ��ȫ>" } },
            { "AdultEvaluate", "<�������ݵ����ۣ�˵��Υ��ԭ��(ʹ�ý�Ϊ�º͵�����)>" },
            { "AdultSuggestion", "<�������ݵ��޸Ľ���(ʹ�ý�Ϊ�º͵�����)>" },
            { "AIResultCode", "<�Ƿ�AI����,0-������1-Υ��>" },
            { "AIScore", "AI���ɵĸ������� 0-100 ��Խ��Υ����ʾ�Խ��>" },
            { "AIEvaluate", "<AI���ݵ����ۣ�˵��Υ��ԭ��(ʹ�ý�Ϊ�º͵�����)>" },
            { "PoliticsResultCode", "<�Ƿ��漰��������,0-������1-Υ��>" },
            { "PoliticsScore", "�����������ݵ�Υ������ 0-100 ��Խ��Υ����ʾ�Խ��>" },
            { "PoliticsContent", new[] { "<������������,չʾԭ�ģ����������������������ޣ��辡��չʾ��ȫ>" } },
            { "PoliticsEvaluate", "<�����������ݵ����ۣ�˵��Υ��ԭ��(ʹ�ý�Ϊ�º͵�����)>" },
            { "PoliticsSuggestion", "<�����������ݵ��޸Ľ���(ʹ�ý�Ϊ�º͵�����)>" }
        };
    }

    public static string CleanMarkdownCode(string input)
    {
        const string pattern = @"```\w*\s*([\s\S]*?)```";
        return Regex.Replace(input, pattern, "$1");
    }

    // �ڲ��ඨ��
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
        /// <summary>
        /// �Ƿ������������
        /// </summary>
        public int AdultResultCode { get; set; }
        /// <summary>
        /// ������������
        /// </summary>
        public int AdultScore { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public List<string> AdultContent { get; set; }
        /// <summary>
        /// ������������
        /// </summary>
        public string AdultEvaluate { get; set; }
        /// <summary>
        /// ���������޸Ľ���
        /// </summary>
        public string AdultSuggestion { get; set; }
        /// <summary>
        /// �Ƿ�ΪAI����
        /// </summary>
        public int AIResultCode { get; set; }
        /// <summary>
        /// AI��������
        /// </summary>
        public int AIScore { get; set; }
        /// <summary>
        /// AI���ݵ�����
        /// </summary>
        public string AIEvaluate { get; set; }
        /// <summary>
        ///  �Ƿ�����
        /// </summary>
        public int PoliticsResultCode { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public int PoliticsScore { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public List<string> PoliticsContent { get; set; }
        /// <summary>
        /// ������������
        /// </summary>
        public string PoliticsEvaluate { get; set; }
        /// <summary>
        /// �����޸Ľ���
        /// </summary>
        public string PoliticsSuggestion { get; set; }
    }
}