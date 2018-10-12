# Webex Teams API Client for .NET

[![nuget](https://img.shields.io/nuget/v/Thrzn41.WebexTeams.svg)](https://www.nuget.org/packages/Thrzn41.WebexTeams) [![MIT license](https://img.shields.io/github/license/thrzn41/WebexTeamsAPIClient.svg)](https://github.com/thrzn41/WebexTeamsAPIClient/blob/master/LICENSE)

`Webex Teams API Client` is a Library that wraps `Cisco Webex Teams REST API`.   
Also, some useful features for developers are provided.

#### README in other language
* [日本語のREADMEはこちら](https://github.com/thrzn41/WebexTeamsAPIClient/blob/master/README.ja-JP.md) ([Japanese README is here](https://github.com/thrzn41/WebexTeamsAPIClient/blob/master/README.ja-JP.md))

---
By using `Webex Teams API Client`, you can invoke `Cisco Webex Teams REST API` easily.  

* A Simple example to post a message as markdown.

``` csharp
// Load encrypted bot token from storage.
ProtectedString token = LoadEncryptedBotToken();

// Create a TeamsAPIClient instance.
var teams = TeamsAPI.CreateVersion1Client(token);

// Build markdown.
var markdown = new MarkdownBuilder();
markdown.Append("Hi, ").AppendBold("Webex Teams").Append("!!");

// Post a message.
var message = (await teams.CreateDirectMessageAsync("your_webex_teams_account@example.com", markdown.ToString())).GetData();

Console.WriteLine("Message was posted: ID = {0}", message.Id);
```

* An example to use Guest Issuer

It is an example that facilitates [Guest Issuer](https://developer.webex.com/guest-issuer.html) of Webex Teams.  

``` csharp
// Load encrypted Guest Issuer secret from storage.
ProtectedString secret = LoadEncryptedGuestIssuerSecret();

// Create a GuestIssuerClient instance.
var guestIssuer = TeamsAPI.CreateVersion1GuestIssuerClient(secret, "your_guest_issuer_id");


// Create a Guest User.
var guest = (await guestIssuer.CreateGuestUserAsync("my-guest-id", "GuestUserName")).GetData();

// Create a TeamsAPIClient instance for the Guest User.
var teams = TeamsAPI.CreateVersion1Client(guest);

// Post a message from the Guest User.
var message = (await teams.CreateDirectMessageAsync("your_webex_teams_account@example.com", "Hello, I am a guest!!")).GetData();

Console.WriteLine("Message was posted: ID = {0}", message.Id);
```

* An example to facilitate Pagination

It is an example that facilitates [Pagination feature](https://developer.webex.com/pagination.html) of Webex Teams REST API.  
It demonstrates to get and iterate for each 50 spaces, then say "Hello" to the specific named space.

``` csharp
// Gets Enumerator to list all the Group spaces.
var e = (await teams.ListSpacesAsync(
                       type: SpaceType.Group,
                       max: 50)
        ).GetListResultEnumerator();

// Iterates until getting all the spaces.
while (await e.MoveNextAsync())
{
  var r = e.CurrentResult;

  if (r.IsSuccessStatus && r.Data.HasItems)
  {
    // Each result has space list.
    foreach (var space in r.Data.Items)
    {
      // Say "Hello" to the specific named space.
      if (space.Title == "Demo space for Webex Teams API Client(Thrzn41.WebexTeams)")
      {
        await teams.CreateMessageAsync(space, "Hello, Webex Teams!!");
      }
    }

  }
}
```

More samples are [here](https://github.com/thrzn41/WebexTeamsAPIClientSamples).

---
## Available Platforms

* .NET Standard 1.3 or later
* .NET Core 1.0 or later
* .NET Framework 4.5.2 or later

> NOTE: If you use Simple Webhook Listener/Server feature,  
> .NET Standard 2.0+, .NET Core 2.0+ or .NET Framework 4.5.2+ is required.

---
## Samples

Samples for Webex Teams API Client is available on [here](https://github.com/thrzn41/WebexTeamsAPIClientSamples).


---
## Available Features

* Basic Webex Teams APIs(List/Get/Create Message, Space, etc.).
* Webex Teams Admin APIs(List/Get Event, License, etc.).
* Encrypt/Decrypt Webex Teams token in storage.
* Pagination for list APIs. Enumerator to facilitate the pagination.
* Retry-after value, Retry executor.
* Markdown builder.
* Error code, error description.
* Webhook secret validator, Webhook notification manager, Webhook event handler.
* Guest Issuer helper.
* OAuth2 helper.
* Simple Webhook Listener/Server(.NET Standard 2.0+, .NET Core 2.0+, .NET Framework 4.5.2+).

### Basic Features

| Teams Resource              | Available Feature             | Description                                     |
| :-------------------------- | :---------------------------- | :---------------------------------------------- |
| Person/People               | List/Get                      | Available. Get Me is also available                        |
| Space(Room)                 | List/Create/Get/Update/Delete | Available. Room is called 'Space' in this API Client.   |
| SpaceMembership(Membership) | List/Create/Get/Update/Delete | Available. Membership is called 'SpaceMembership' in this API Client.                                               |
| Message                     | List/Create/Get/Delete        | Available. Attach file from local stream is also available |
| Team                        | List/Create/Get/Update/Delete | Available.                                      |
| TeamMembership              | List/Create/Get/Update/Delete | Available.                                      |
| Webhook                     | List/Create/Get/Update/Delete | Available.                                      |
| File                        | GetInfo/GetData/Upload        | Available.                                      |

### Admin Features

| Teams Resource | Available Feature | Description |
| :-------------- | :---------------------------- | :---------------------------------------------- |
| Person/People   | Create/Update/Delete          | Available.                                      |
| Event           | List/Get                      | Available.                                      |
| Organization    | List/Get                      | Available.                                      |
| License         | List/Get                      | Available.                                      |
| Role            | List/Get                      | Available.                                      |
| GroupResource            | List/Get                      | Available.                                      |
| GroupResourceMembership  | List/Get/Update                      | Available.                                      |

### Token encryption/decryption in storage
`ProtectedString` provides token encryption/decryption.  
More details are described later.

### Pagination

Cisco Webex Teams API pagination is described on [here](https://developer.webex.com/pagination.html).

`result.HasNext` and `result.ListNextAsync()` are available in the Webex Teams API Client.  
Also, `TeamsListResultEnumerator` is available.  
More details are described later.

### Gets retry-after

`result.HasRetryAfter` and `result.RetryAfter` are available in the Webex Teams API Client.  
Also, `RetryExecutor` is available.  
More details are described later.

### Gets HttpStatus code

`result.HttpStatusCode` is available in the Webex Teams API Client.  
More details are described later.

### Gets Error code/description

There are cases when Cisco Webex Teams API returns error with error code and description.  
`result.Data.HasErrors` and `result.Data.GetErrors()` are available in the Webex Teams API Client.

### Gets Partial Errors

There are cases when Cisco Webex Teams API returns partial errors.  
This is described on [here](https://developer.webex.com/errors.html).    
`Item.HasErrors` and `Item.GetPartialErrors()` are available in the Webex Teams API Client.

### Gets trackingId

The trackingId may be used on technical support of Cisco Webex Teams API side.

`result.TrackingId` is available in the Webex Teams API Client.  
More details are described later.

### Validates webhook secret

`Webhook.CreateEventValidator()` is available in the Webex Teams API Client.  
Also, `WebhookNotificationManager` is available to facilicate event handling.  
More details are described later.

CreateWebhookAsync() method in the Webex Teams API Client generates `webhook secret` dynamically by default option.

### Markdonw builder

`MarkdownBuilder` is available in the Webex Teams API Client.  
More details are described later.

### OAuth2 Helper

`TeamsOauth2Client` is available in the Webex Teams API Client.  

### Webhook Listener(.NET Standard 2.0+, .NET Core 2.0+, .NET Framework 4.5.2+)

Webhook listener feature provides simple Webhook server feature.  
> **NOTE: This feature is intended to be used for quick testing purpose.  
> In production environment, more reliable server solution should be used.**

`WebhookListener` is available in the Webex Teams API Client.  
More details are described later.

---
## Basic Usage

### Install Webex Teams API Client

You can install `Webex Teams API Client` from `NuGet` package manager by any of the following methods.

* NuGet Package Manager GUI  
Search "`Thrzn41.WebexTeams`" package and install.

* NuGet Package Manager CLI  
```
PM> Install-Package Thrzn41.WebexTeams
```

* .NET Client  
```
> dotnet add package Thrzn41.WebexTeams
```

### using Directive to import Webex Teams API Client related types

If you want to use using directive, the following namespaces could be used.

``` csharp
using Thrzn41.Util
using Thrzn41.WebexTeams
using Thrzn41.WebexTeams.Version1
```

You can also use the following namespaces, if needed.
* `Thrzn41.WebexTeams.Version1.GuestIssuer` to use Guest Issuer helper.
* `Thrzn41.WebexTeams.Version1.OAuth2` to use OAuth2 helper.
* `Thrzn41.WebexTeams.Version1.Admin` to use Admin APIs.

### Create a Webex Teams API Client instance

A Webex Teams API Client instance should be re-used as long as possible.

``` csharp
/// Basic APIs.
TeamsAPIClient teams = TeamsAPI.CreateVersion1Client(token);
```

If you use Admin APIs, an instance for Admin APIs is required.  
`TeamsAdminAPIClient` has all the features of `TeamsAPIClient` and Admin Features.
``` csharp
/// Admin APIs.
TeamsAdminAPIClient teams = TeamsAPI.CreateVersion1AdminClient(token);
```

> **NOTE: The 'token' is very sensitive information for Cisco Webex Teams API.  
> You MUST protect the 'token' carefully.  
> NEVER put it in source code directly or NEVER save it in unsecure manner.  
> `Webex Teams API Client` provides some token encryption/decryption methods.  
> If you use your own token encryption/decryption/protection methods, you can create an instance with the decrypted token string.**

### Save encrypted token to storage

``` csharp
char[] tokens = GetBotTokenFromBotOwner();

var protectedToken = LocalProtectedString.FromChars(tokens);
LocalProtectedString.ClearChars(tokens);

Save("token.dat",   protectedToken.EncryptedData);
Save("entropy.dat", protectedToken.Entropy);
```

> **NOTE: `LocalProtectedString` does not provide in-memory protection.  
> This is intended to be used to save and load encrypted token.**

### Load encrypted token from storage and create a Webex Teams API Client

``` csharp
byte[] encryptedData = Load("token.dat");
byte[] entropy       = Load("entropy.dat");

var protectedToken = LocalProtectedString.FromEncryptedData(encryptedData, entropy);

/// Basic APIs.
TeamsAPIClient teams = TeamsAPI.CreateVersion1Client(protectedToken);
```

> **NOTE: The encrypted data can be decrypted only on the same local user or local machine as encrypted based on option parameter.**

### Post a message to a Cisco Webex Teams Space

``` csharp
var result = await teams.CreateMessageAsync("xyz_space_id", "Hello, Teams!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message was posted: id = {0}", result.Data.Id);
}
```

### Success, fail, error handling

You can use `result.IsSuccessStatus` to check if the request is succeeded or not.
You can also use `result.Data.HasErrors` and `result.Data.GetErrorMessage()` to retrieve error code value of error description from Cisco Webex Teams API service.
``` csharp
var result = await teams.CreateMessageAsync("xyz_space_id", "Hello, Teams!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message was posted: id = {0}", result.Data.Id);
}
else
{
  Console.WriteLine("Failed to post a message: status = {0}, trackingId = {1}", result.HttpStatusCode, result.TrackingId);

  if(result.Data.HasErrors)
  {
    Console.WriteLine( result.Data.GetErrorMessage() );
  }
}
```

If you preferred to catch Exception, you can use `result.GetData()` to get data.  
The `result.GetData()` will throw `TeamsResultException` on request error.  
(On the other hand, `result.Data` does not throw `TeamsResultException`.)

``` csharp
try
{
  var result = await teams.CreateMessageAsync("xyz_space_id", "Hello, Teams!");

  var message = result.GetData();

  Console.WriteLine("Message was posted: id = {0}", message.Id);
}
catch(TeamsResultException tre)
{
  Console.WriteLine("Failed to post a message: status = {0}, trackingId = {1}, description = {2}",
                      tre.HttpStatusCode, tre.TrackingId, tre.Message);
}
```

### Post a message with attachment to a Cisco Webex Teams Space

``` csharp
using (var fs   = new FileStream("path/myfile.png", FileMode.Open, FileAccess.Read, FileShare.Read))
using (var data = new TeamsFileData(fs, "imagefile.png", TeamsMediaType.ImagePNG))
{
    var result = await teams.CreateMessageAsync("xyz_space_id", "Hello Teams with Attachment", data);

    if(result.IsSuccessStatus)
    {
       Console.WriteLine("Message was posted with attachment: id = {0}", result.Data.Id);
    }
}
```

### Post a message to a Cisco Webex Teams 1:1 Space

``` csharp
var result = await teams.CreateDirectMessageAsync("targetuser@example.com", "Hello, Teams!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message was posted: id = {0}", result.Data.Id);
}
```

### List spaces

``` csharp
var result = await teams.ListSpacesAsync();

if(result.IsSuccessStatus && result.Data.HasItems)
{
  foreach (var item in result.Data.Items) {
    Console.WriteLine("Space: title = {0}", item.Title);
  }  
}
```

### Get File info or File data

Get File info without downloading the file.
``` csharp
var result = await teams.GetFileInfoAsync(new Uri("https://api.example.com/path/to/file.png"));

if(result.IsSuccessStatus)
{
  var file = result.Data;

  Console.WriteLine("File: Name = {0}, Size = {1}, Type = {2}", file.Name, file.Size?.Value, file.MediaType?.Name);
}
```

Download the file.
``` csharp
var result = await teams.GetFileDataAsync(new Uri("https://api.example.com/path/to/file.png"));

if(result.IsSuccessStatus)
{
  var file = result.Data;

  Console.WriteLine("File: Name = {0}, Size = {1}, Type = {2}", file.Name, file.Size?.Value, file.MediaType?.Name);

  using(var stream = file.Stream)
  {
    // File data will be contained in the stream...
  }
}
```

### Pagination

``` csharp
var result = await teams.ListSpacesAsync();

if(result.IsSuccessStatus)
{
  //
  // Do something...
  //

  if(result.HasNext)
  {
    // List next result.
    result = await result.ListNextAsync();

    if(result.IsSuccessStatus)
    {
      // ...
    }
  }
}
```

### Enumerator for Pagination

``` csharp
// Gets Enumerator to list all the Group spaces.
var e = (await teams.ListSpacesAsync(
                       type: SpaceType.Group,
                       max: 50)
        ).GetListResultEnumerator();

// Iterates until getting all the spaces.
while (await e.MoveNextAsync())
{
  var r = e.CurrentResult;

  if (r.IsSuccessStatus && r.Data.HasItems)
  {
    // Each result has space list.
    foreach (var space in r.Data.Items)
    {
      Console.WriteLine("Title = {0}", space.Title);
    }

  }
}
```

### Gets Http status code of the request

``` csharp
var result = await teams.ListSpacesAsync();

Console.WriteLine("Status is {0}", result.HttpStatusCode);
```

### Gets retry after value

``` csharp
var result = await teams.ListSpacesAsync();

if(result.IsSuccessStatus)
{
  //
  // Do something...
  //
}
else if(result.HasRetryAfter)
{
  Console.WriteLine("Let's retry: {0}", result.RetryAfter.Delta);  
}
```

### Retry Executor

`RetryExecutor` facilitates retry.

``` csharp
// RetryExecutor.One requests with 1 time retry at most.
var result = await RetryExecutor.One.ListAsync(
  () =>
  {
      // This function will be executed again if needed.
      return teams.ListSpacesAsync();
  },

  (r, retryCount) =>
  {
      // This function will be executed before evry retry request.

      // You can output logs or other things at this point.
      Log.Info("Retry is required: delta = {0}, counter = {1}", r.RetryAfter.Delta, retryCount);

      // Return 'true' when you want to proceed retry.
      return true;
  }
);
```

### Gets TrackingId

``` csharp
var result = await teams.ListSpacesAsync();

Console.WriteLine("Tracking id is {0}", result.TrackingId);  
```

### Markdown Builder

``` csharp
var md = new MarkdownBuilder();

// Creates markdown with mention and ordered list.
md.Append("Hi ").AppendMentionToPerson("xyz_person_id", "PersonName").AppendLine();
md.AppendOrderedList("Item1");
md.AppendOrderedList("Item2");
md.AppendOrderedList("Item3");

var result = await teams.CreateMessageAsync("xyz_space_id", md.ToString());
```

### Validates webhook event data

``` csharp
var webhook = await teams.GetWebhookAsync("xyz_webhook_id");

var validator = webhook.CreateEventValidator();
```

When an event is notified on webhook uri,  
X-Spark-Signature will have hash code.

The validator can be used to validate the data.

``` csharp
byte[] webhookEventData = GetWebhookEventData();

if( validator.Validate(webhookEventData, "xyz_x_teams_signature_value") )
{
  Console.WriteLine("Notified data is valid!");
}
```

### Webhook Notification manager

Webhook notification manager manages webhooks and event notification.

* Create instance.

``` csharp
var notificationManager = new WebhookNotificationManager();
```

* Then, add webhook to manager with notification function.

``` csharp
var webhook = await teams.GetWebhookAsync("xyz_webhook_id");

notificationManager.AddNotification(
  webhook,
  (eventData) =>
  {
    Console.WriteLine("Event is notified, id = {0}", eventData.Id);
  }
);
```

* On receiving webhook event.

``` csharp
byte[] webhookEventData = GetWebhookEventData();

// Signature will be checked and notified to function which is added earlier.
notificationManager.ValidateAndNotify(webhookEventData, "xyz_x_teams_signature_value", encodingOfData);
```

### Webhook Listener

* Create webhook listener instance.

``` csharp
var listener = new WebhookListener();
```

* Add listening host and port.

The TLS/https connection SHOULD be used for listening port.  
To do this, a valid certificate for the listener SHOULD be bound in your environment first by `netsh` tools or related tools.

Add listening endpoint with the bound address and port.

``` csharp
var endpointUri = listener.AddListenerEndpoint("yourwebhookserver.example.com", 8443);
```

* Create webhook for the listener.

The `endpointUri` returned by `listener.AddListenerEndpoint()` is Uri of the webhook.  

``` csharp
var result = await teams.CreateWebhookAsync(
  "my webhook for test",
  endpointUri,
  EventResource.Message,
  EventType.Created);
```

* Add webhook to listener with notification function.

``` csharp
var webhook = result.Data;

listener.AddNotification(
  webhook,
  async (eventData) =>
  {
    Console.WriteLine("Event is notified, id = {0}", eventData.Id);

    if(eventData.Resource == EventResource.Message)
    {
      Console.WriteLine("Message, id = {0}", eventData.MessageData.Id);
    }
  }
);
```

* Start listener.

After starting listener, events will be notified to registered notification function.

``` csharp
listener.Start();
```

### Webhook Listener with ngrok

If you do not have global ip address.  
You may be able to use tunneling services such as [ngrok](https://ngrok.com/).

* Get ngrok and start it.

You can download ngrok command line tool from [here](https://ngrok.com/).

The following command will start tunneling that will forward to port 8080 of localhost.
```
prompt> ngrok http 8080 --bind-tls=true
```

* Create webhook listener instance.

``` csharp
var listener = new WebhookListener();
```

* Add listening host and port.

The ngrok will forward to `localhost`.

``` csharp
var endpointUri = listener.AddListenerEndpoint("localhost", 8080, false);
```

* Create webhook for the listener.

In this case, a tunneling service is used with ngrok,  
The `endpointUri` returned by `listener.AddListenerEndpoint()` is Uri to be forwarded.  

You should create webhook with ngrok uri.

If ngrok assigns `https://ngrok-xyz.example.com`,  
You need to create webhook with `String.Format("https://ngrok-xyz.example.com{0}", endpointUri.AbsolutePath)`.

``` csharp
var result = await teams.CreateWebhookAsync(
  "my webhook for test",
  new Uri(String.Format("https://ngrok-xyz.example.com{0}", endpointUri.AbsolutePath)),
  EventResource.Message,
  EventType.Created);
```

* Add webhook to listener with notification function.

``` csharp
var webhook = result.Data;

listener.AddNotification(
  webhook,
  async (eventData) =>
  {
    Console.WriteLine("Event is notified, id = {0}", eventData.Id);

    if(eventData.Resource == EventResource.Message)
    {
      Console.WriteLine("Message, id = {0}", eventData.MessageData.Id);
    }
  }
);
```

* Start listener.

After starting listener, events will be notified to registered notification function.

``` csharp
listener.Start();
```
