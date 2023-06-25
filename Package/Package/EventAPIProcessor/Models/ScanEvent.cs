using Newtonsoft.Json;

namespace EventAPIProcessor.Models;

[JsonObject("ScanEvents")]
public record EventResponse
{
    public List<ScanEvent> ScanEvents { get; init; }
}
public record ScanEvent
{
    [JsonRequired]
    public int EventId { get; init; }
    [JsonRequired]
    public int ParcelId { get; init; }
    [JsonRequired]
    public string Type { get; init; }
    
    [JsonRequired]
    public DateTime CreatedDateTimeUtc { get; init; }
    [JsonRequired]
    public string StatusCode { get; init; }
    public Device Device { get; init; }
    public User User { get; init; }
    }
    
    public record Device
    {
        public int DeviceTransactionId { get; init; }
        public int DeviceId { get; init; }
    }

    public record User
    {
        public string UserId { get; init; }
        public string CarrierId { get; init; }
        [JsonRequired]
        public string RunId { get; init; }
}