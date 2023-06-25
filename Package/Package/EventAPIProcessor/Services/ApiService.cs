using System.Net.Http.Headers;
using CSharpFunctionalExtensions;
using EventAPIProcessor.Models;
using EventAPIProcessor.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventAPIProcessor;

public class ApiService : IClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService() 
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost");  //can set in the json environment variable 
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        _logger = new Logger<ApiService>(loggerFactory); 
    }

    public async Task<Result<Maybe<EventResponse>, IError>> GetResponseAsync(int fromEventId = 1, int limit = 100)
    {
        if (fromEventId < 0)
            return Result.Failure<Maybe<EventResponse>, IError>(new ResponseRror($"Cannot fetch scan events due to invalid fromEventId: {fromEventId}") as IError);

        var url = $"/v1/scans/scanevents?FromEventId={fromEventId}&Limit={limit}"; 
        _logger.LogInformation($"Fetching scan events from {url}");
        var response = await _httpClient.GetAsync(url);

        return await ParseResponseAsync(response, fromEventId);
    }

    public async Task<Result<Maybe<EventResponse>, IError>> ParseResponseAsync(HttpResponseMessage response, int fromEventId)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var scanEventResponse = new EventResponse(); 
            try {
                scanEventResponse = JsonConvert.DeserializeObject<EventResponse>(content);
            } catch (Exception e) {
                return Result.Failure<Maybe<EventResponse>, IError>(new JsonError($"Exceptions deserialise ScanEvents: {JsonConvert.SerializeObject(e)}. \nResponse: {JsonConvert.SerializeObject(response)}") as IError);
            }

            return Result.Success<Maybe<EventResponse>, IError>(Maybe<EventResponse>.From(scanEventResponse));
        }
        
        return Result.Failure<Maybe<EventResponse>, IError>(new ResponseRror($"Failed to fetch scan events from {fromEventId}. Response {response}"));
    }
    
}