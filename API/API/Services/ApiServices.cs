namespace API.Services
{
    public class ApiServices
    {
        public HttpClient GetHttp()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://apis.gometa.org/");
            return httpClient;
        }
    }
}
