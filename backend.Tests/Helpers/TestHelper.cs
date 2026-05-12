using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

public static class TestHelper
{
    public static StringContent ToJson(object obj)
    {
        return new StringContent(
            JsonConvert.SerializeObject(obj),
            Encoding.UTF8,
            "application/json");
    }

    public static void SetAuth(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public static void SetTestAuth(HttpClient client, Guid userId, string email)
    {
        client.DefaultRequestHeaders.Authorization = null;
        client.DefaultRequestHeaders.Remove("X-Test-UserId");
        client.DefaultRequestHeaders.Remove("X-Test-Email");
        client.DefaultRequestHeaders.Add("X-Test-UserId", userId.ToString());
        client.DefaultRequestHeaders.Add("X-Test-Email", email);
    }
}
