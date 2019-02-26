using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public static class HttpClientHelper
{
    private static HttpClient httpClient = new HttpClient();

    public static async Task<(HttpStatusCode,string)> GetAsync(string apiUrl)
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        var response = await httpClient.SendAsync(httpRequestMessage);
        if (response.IsSuccessStatusCode)
        {
            return (HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        }

        return (response.StatusCode, $"Post to {apiUrl} failed with code {response.StatusCode}");
    }

    public static async Task<(HttpStatusCode, string)> PostAsync(string apiUrl, string jsonBodyPayLoad)
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        httpRequestMessage.Content = new StringContent(jsonBodyPayLoad, Encoding.UTF8, "application/json");
        var response = await httpClient.SendAsync(httpRequestMessage);
        if (response.IsSuccessStatusCode)
        {
            return (HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        }

        return (response.StatusCode, $"Post to {apiUrl} failed with code {response.StatusCode}");
    }
}
