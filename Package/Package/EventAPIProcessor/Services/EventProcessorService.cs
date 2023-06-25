using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using EventAPIProcessor.Models;
using EventAPIProcessor.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventAPIProcessor;

public class EventProcessorService
{
    private readonly IClientService _clientService;
    private readonly ILogger<EventProcessorService> _logger;
    
    public EventProcessorService(IClientService apiService)
    {
        _clientService = apiService; 
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        _logger = new Logger<EventProcessorService>(loggerFactory); 
    }
    
    public async Task Start()
    {
        var filePath = GetStoreFilePath();
        var storedScanEvents = GetEventsFromStore(filePath);
        _logger.LogInformation($"Total events in store: {storedScanEvents.ScanEvents.Count}");

        var latestEventId = storedScanEvents.ScanEvents
            .Select(x => x.EventId)
            .OrderByDescending(x => x)
            .FirstOrDefault();
        
        var scanEventR = await _clientService.GetResponseAsync(latestEventId);

        var newEventSaved = scanEventR
            .Bind(response =>
            {
                var isSaveSuccess = response.Match(scanEvent =>
                {
                    var storedEventIds = storedScanEvents.ScanEvents.Select(x => x.EventId).ToHashSet();
                    //log when detecting new event Type 
                    var newEvents = new List<ScanEvent>();
                    scanEvent.ScanEvents.ForEach(et =>
                    {
                        DetectNewEventType(et);
                        if (!storedEventIds.Contains(et.EventId))
                        {
                            newEvents.Add(et);
                        }
                    });

                    SaveEvents(newEvents, storedScanEvents, filePath);

                    _logger.LogInformation("New scan events saved to store. Count={EventCount}. FromEventId:{EventId}", newEvents.Count, latestEventId);
                    return true;
                }, () => false);

                if (isSaveSuccess)
                    return UnitResult.Success<IError>();
                return UnitResult.Failure<IError>(
                    new StoreError($"Failed to save new events to store. FromEventId: {latestEventId}") as IError);
            })
            .MapError(e =>
            {
                e.Errors.ForEach(errorMessage => _logger.LogError(errorMessage));
                return e;
            }); 
    }

    public EventResponse GetEventsFromStore(string filePath)
    {
        var allText = File.ReadAllText(filePath);


       return JsonConvert.DeserializeObject<EventResponse>(allText);
    }

    public string GetStoreFilePath()
    {
        var dirParent = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;

        while (dirParent.Name != "Package")
        {
            dirParent = dirParent.Parent;
        }

        string filePath = dirParent + "/EventAPIProcessor/store.json";
        return filePath;
    }

    public void SaveEvents(List<ScanEvent> newEvents, EventResponse storedScanEvents, string filePath)
    {
        //save to store
        if (newEvents.Count > 0)
        {
            storedScanEvents.ScanEvents.AddRange(newEvents);
            var eventsToSave = JsonConvert.SerializeObject(storedScanEvents);
            File.WriteAllText(filePath, eventsToSave);
        }
    }

    private void DetectNewEventType(ScanEvent et)
    {
        var eventTypes = ((EventType[]) Enum.GetValues(typeof(EventType)))
            .Select(x => x.ToString().ToUpper())
            .ToList();
        if (!eventTypes.Contains(et.Type))
        {
            //Error because it raise the attention that the requirements changed
            _logger.LogError($"New event type detected: {et.Type}");
        }
    }
}