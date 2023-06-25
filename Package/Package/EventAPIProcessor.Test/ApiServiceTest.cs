using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using EventAPIProcessor.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace EventAPIProcessor.Test;

public class ApiServiceTest
{
    [Test]
    public async Task ParseResponse_GivenInvalidJsonResponse_ThenShouldReturnResultError()
    {
        var apiService = new ApiService();

        //"eventId" property is incorrectly name to "eventIds", this will cause exception 
        //Type should be string but given int
        string jsonStr = "{\"ScanEvents\": [{\"eventIds\": 0, \"parcelId\":2,\"type\":3,\"createdDateTimeUtc\":\"0001-01-01T00:00:00Z\",\"statusCode\":\"\",\"device\":{\"deviceTransactionId\":2,\"deviceId\":1},\"user\":{\"userId\":\"1\",\"carrierId\":\"2\",\"runId\":\"3\"}}]}";
        var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonStr, Encoding.UTF8, "application/json")

        };
        var result = await apiService.ParseResponseAsync(httpResponse, 123);
        result.IsSuccess.Should().BeFalse();
        result.Error.Errors.Should().NotBeEmpty(); 
    }


    [Test]
    public async Task ParseResponse_GivenValidJsonResponse_ThenShouldReturnSucessWithScanEvents()
    {
        var apiService = new ApiService();

        var scanEvents = new EventResponse()
        {
            ScanEvents = new List<ScanEvent>
            {
                new ScanEvent()
                {
                    EventId = 1,
                    ParcelId = 2,
                    CreatedDateTimeUtc = new DateTime().AsUtc(),
                    Type = EventType.Delivery.ToString().ToUpper(),
                    StatusCode = "",
                    Device = new Device()
                    {
                        DeviceId = 1,
                        DeviceTransactionId = 2
                    },
                    User = new User()
                    {
                        UserId = "1",
                        CarrierId = "2",
                        RunId = "3"
                    }
                }
            }
        }; 

    var httpResponse = new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(scanEvents)

        };
        var result = await apiService.ParseResponseAsync(httpResponse, 123);
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().BeEquivalentTo(scanEvents);
    }
}
