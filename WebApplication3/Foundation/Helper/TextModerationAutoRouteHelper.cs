using TouchSocket.Core;

namespace WebApplication3.Foundation.Helper;

public class TextModerationAutoRouteHelper
{
    public static void Examine(string text)
    {
        // Construct text moderation request.
        AlibabaCloud.SDK.Green20220302.Models.TextModerationRequest textModerationRequest = new AlibabaCloud.SDK.Green20220302.Models.TextModerationRequest();
        // Setup service according to your requirement.
        textModerationRequest.Service = "comment_multilingual_global";
        Dictionary<string, object> task = new Dictionary<string, object>();
        task.Add("content", text);
        if (!task.ContainsKey("content") || Convert.ToString(task["content"]).Trim() == string.Empty)
        {
            Console.WriteLine("text moderation content is empty");
            return;
        }

        textModerationRequest.ServiceParameters = task.ToJsonString();


        // The leakage of code may lead to the leakage of AccessKey and security breakage of your account. The following examples are for reference only. It is recommended to use STS tokens for authorization, please refer to related documentation.
        // Please ensure that the environment variables ALIBABA_CLOUD_ACCESS_KEY_ID and ALIBABA_CLOUD_ACCESS_KEY_SECRET are set.
        AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
        {
            AccessKeyId = "",
            AccessKeySecret = "",
            // Modify region and endpoint according to your requirement.
            Endpoint = "green-cip.ap-southeast-1.aliyuncs.com",
        };
        // To improve performance, we strongly recommend you reuse client instance to avoid establishing duplicate connections.
        AlibabaCloud.SDK.Green20220302.Client client = new AlibabaCloud.SDK.Green20220302.Client(config);

        // Create RuntimeObject instance and set runtime parameters.
        AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
        runtime.ReadTimeout = 10000;
        runtime.ConnectTimeout = 10000;
        try
        {
            // Method to obtain detection result.
            AlibabaCloud.SDK.Green20220302.Models.TextModerationResponse response =
                client.TextModerationWithOptions(textModerationRequest, runtime);

            // Print detection result.
            Console.WriteLine(response.Body.RequestId);
            Console.WriteLine(response.Body.ToJsonString());
        }
        catch (Exception _err)
        {
            Console.WriteLine(_err);
        }
    }
}