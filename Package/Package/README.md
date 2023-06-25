Assumptions:
* Worker application? 
    * I am assuming it's a background service, it's always running
* EventId, ParcelId, Type, CreatedDateTimeUtc, StatusCode, RunId are required in JSON response, because it's used to lookup stored scan events. 
* How the worker application will be used?
    * Schdule in AWS lambda? Via a custom interface that start the scheduler? Azure etc..?
    * I don't know, and the EventAPIProcessor is in its own project and can be invoked in your needs. In this case, it's invoked in Console app.
* Store data that easily fetch for the mentioned fields
    * Scan events are stored in json file for simplicity. 
    * It can be further fetched by EventId, RunId, etc.. 
    * It's out of scope for current delivery - it's considered, but not yet impmlement. 
* Resilence 
    * When there is a new event Type, it will be detected and send out log errors, which reflects requirements changed. 
    * It also log errors on Derialisation exceptions 
    * It gets the latest EventId from store, then fetch new ScanEvents after the latest Event stored.
    * An improvement, app can save new Events in a smaller batches, to data is saved more often, and prevent smaller loss if app is crashed/down. 
* Architecture changes when there is another worker application downstream? 
    * Current architecture: 
        [ Conole app -> EventProcessorService ] <-> DataStore 
    * Let's say we want to add another worker service for send emails to customers to update them with the latest parcel event
        [ Conole app -> EventProcessorService ]ScanEventWorker <-> STORE <- EmailWorker
        The Email service will subscribe to data changes in the store when there is a new nvent, it then send the events to customers in Email Service.
        If use DynamoDB as store, EmailWorker can listen to db stream changes, and EmailWorker can lambda here. It doesn't need a scheduler as ScanEventWorker
        I think we want to add another downstream work app, so that we have application with single responsibility and enforce horizontal scalability. 
* Layering
    The API Client specific can be separate into another project for endpoint fetching only. 
