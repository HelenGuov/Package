using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EventAPIProcessor.Models;
using EventAPIProcessor.Services;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using NUnit.Framework;

namespace EventAPIProcessor.Test;

public class EventProcessorServiceTest
{
    [Test]
    public async Task StartEvent()
    {
        var scanEvents = new EventResponse()
        {
            ScanEvents = new List<ScanEvent>()
            {
                new ScanEvent()
                {
                    EventId = 2,
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
                },
                new ScanEvent()
                { 
                    EventId = 3,
                    ParcelId = 3,
                    CreatedDateTimeUtc = new DateTime().AsUtc(),
                    Type = EventType.Pickup.ToString().ToUpper(),
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
            

        Mock<IClientService> mockClientService = new Mock<IClientService>();
        mockClientService.Setup(x => x.GetResponseAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Success<Maybe<EventResponse>, IError>(scanEvents));
        var eventProcessorService = new EventProcessorService(mockClientService.Object);
        
        var filePath = eventProcessorService.GetStoreFilePath();
        string jsonStr = "{\"ScanEvents\": [{\"eventId\": 0, \"parcelId\":2,\"type\":\"PICKUP\",\"createdDateTimeUtc\":\"0001-01-01T00:00:00Z\",\"statusCode\":\"\",\"device\":{\"deviceTransactionId\":2,\"deviceId\":1},\"user\":{\"userId\":\"1\",\"carrierId\":\"2\",\"runId\":\"3\"}}]}";
        File.WriteAllText(filePath, jsonStr);
        var storedScanEventsBefore = eventProcessorService.GetEventsFromStore(filePath);

        //act 
        await eventProcessorService.Start();
        
        //assert 
        var storedScanEventsAfter = eventProcessorService.GetEventsFromStore(filePath);
        var beforeCount = storedScanEventsBefore.ScanEvents.Count(); 
        storedScanEventsAfter.ScanEvents.Count().Should().Be(beforeCount + scanEvents.ScanEvents.Count());
    }
}

