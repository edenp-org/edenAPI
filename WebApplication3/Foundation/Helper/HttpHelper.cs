using System.Text;

namespace WebApplication3.Foundation.Helper;

public class HttpHelper
{
    private static readonly HttpClient client = new HttpClient();

    /// <summary>
    /// 发送HTTP GET请求
    /// </summary>
    /// <param name="url">请求的URL</param>
    /// <returns>返回响应内容</returns>
    public static string HttpGet(string url)
    {
        try
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode(); // 确保请求成功
            return response.Content.ReadAsStringAsync().Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GET请求出错: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 发送HTTP POST请求
    /// </summary>
    /// <param name="url">请求的URL</param>
    /// <param name="postData">请求的内容</param>
    /// <returns>返回响应内容</returns>
    public static string HttpPost(string url, string postData,Dictionary<string,string> headers)
    {
        try
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                // 设置请求内容
                request.Content = new StringContent(postData, Encoding.UTF8, "application/json");

                // 添加自定义 headers
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                // 发送请求
                HttpResponseMessage response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode(); // 确保请求成功
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"POST请求出错: {ex.Message}");
            return null;
        }
    }
}