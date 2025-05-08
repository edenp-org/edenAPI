using System.Text;

namespace WebApplication3.Foundation.Helper;

public class HttpHelper
{
    private static readonly HttpClient client = new HttpClient();

    /// <summary>
    /// ����HTTP GET����
    /// </summary>
    /// <param name="url">�����URL</param>
    /// <returns>������Ӧ����</returns>
    public static string HttpGet(string url)
    {
        try
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode(); // ȷ������ɹ�
            return response.Content.ReadAsStringAsync().Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GET�������: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// ����HTTP POST����
    /// </summary>
    /// <param name="url">�����URL</param>
    /// <param name="postData">���������</param>
    /// <returns>������Ӧ����</returns>
    public static string HttpPost(string url, string postData,Dictionary<string,string> headers)
    {
        try
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                // ������������
                request.Content = new StringContent(postData, Encoding.UTF8, "application/json");

                // ����Զ��� headers
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                // ��������
                HttpResponseMessage response = client.SendAsync(request).Result;
                response.EnsureSuccessStatusCode(); // ȷ������ɹ�
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"POST�������: {ex.Message}");
            return null;
        }
    }
}