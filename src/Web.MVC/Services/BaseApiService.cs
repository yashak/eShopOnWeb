using Web.MVC.ApiClient;
namespace Web.MVC.Services;

public class BaseApiService
{
    protected readonly IHttpClientFactory _clientFactory;
    protected readonly Web.MVC.ApiClient.ApiClient _client;

    public BaseApiService(IHttpClientFactory clientFactory) {

        _clientFactory = clientFactory;
        _client = new Web.MVC.ApiClient.ApiClient("https://localhost:7067/", _clientFactory.CreateClient());
    }
}
