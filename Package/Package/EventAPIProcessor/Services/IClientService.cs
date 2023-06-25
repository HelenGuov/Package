using CSharpFunctionalExtensions;
using EventAPIProcessor.Models;

namespace EventAPIProcessor.Services;

public interface IClientService
{
    Task<Result<Maybe<EventResponse>, IError>> GetResponseAsync(int fromEventId = 1, int limit = 100); 
}