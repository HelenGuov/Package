namespace EventAPIProcessor.Models;

public record ResponseRror : IError
{
    public List<string> Errors { get; set; }

    public ResponseRror(string error)
    {
        Errors = new List<string> {error};   
    }

}

public record JsonError : IError
{
    public List<string> Errors { get; set; }

    public JsonError(string error)
    {
        Errors = new List<string> {error};   
    }

}

public record StoreError : IError
{
    public List<string> Errors { get; set; }

    public StoreError(string error)
    {
        Errors = new List<string> {error};   
    }

}

