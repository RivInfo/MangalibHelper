using System.Net;
using System.Net.Http.Json;

const string BaseUrl = "https://cdkey.lilith.com/api/";

HttpClient client = new HttpClient();

client.DefaultRequestHeaders.Add("Accept", "application/json");
client.DefaultRequestHeaders.Add("AcceptEncoding", "gzip, deflate, br");
client.DefaultRequestHeaders.Add("AcceptLanguage", "ru,en;q=0.9");
client.DefaultRequestHeaders.Add("ContentType", "application/json");
client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
    "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 " +
    "YaBrowser/23.3.1.895 Yowser/2.5 Safari/537.36");
                                              
