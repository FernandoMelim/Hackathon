using System.Net;

namespace HealthMed.Api.Controllers.Base;

public class ApiBaseResponse
{
    public HttpStatusCode StatusCode { get; set; }

    public List<KeyValuePair<string, List<string>>>? Errors { get; set; } = new List<KeyValuePair<string, List<string>>>();
}

public class ApiBaseResponse<T> : ApiBaseResponse
{
    public T? Data { get; set; }
}
