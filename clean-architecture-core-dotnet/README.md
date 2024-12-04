# Core library for clean architecture in .Net (C#)

## Introduction

This documentation guides you through the utilization of the core library for implementing clean architecture in project
using c#.

We'll explore the creation of custom application request and use cases, paying special attention to handling missing and
unauthorized fields.

Practical examples are provided using code snippets from a test suite to showcase the library's usage in building a
modular and clean c# application.

## Prerequisites

Ensure that you have the following:

- `.net9` installed on your machine.

## Installation

x.x.x represents the version that you want to install.
ex: 1.0.0
ex: 1.0.1

```
via .net cli
> dotnet add package clean-architecture-core-dotnet --version x.x.x

via package manager
> NuGet\Install-Package clean-architecture-core-dotnet -Version x.x.x

via package reference
<PackageReference Include="clean-architecture-core-dotnet" Version="x.x.x" />

via paket cli
> paket add clean-architecture-core-dotnet --version x.x.x

via script & interactive
> #r "nuget: clean-architecture-core-dotnet, x.x.x"

via cake
// Install clean-architecture-core-dotnet as a Cake Addin
#addin nuget:?package=clean-architecture-core-dotnet&version=x.x.x

// Install clean-architecture-core-dotnet as a Cake Tool
#tool nuget:?package=clean-architecture-core-dotnet&version=x.x.x
```

## Core Overview

### Application Request

Requests serve as input object, encapsulating data from your http controller. In the core library, use the `Ug.Request.Request` class
as the foundation for creating custom application request object and implements `Ug.Request.IRequest` interface.
Define the expected fields using the `RequestPossibleFields` property.

### Presenter

Presenters handle the output logic of your usecase. You have to extends `Ug.Presenter.Presenter` and
implements `Ug.Presenter.IPresenter` interface.

### Usecase

Use cases encapsulate business logic and orchestrate the flow of data between requests, entities, and presenters.
Extends the `Ug.Usecase.Usecase` class and implements `Ug.Usecase.IUsecase` with the execute method.

### Response

- Use `Ug.Response.Response` to create usecase `response`.
- Supports success/failure status, custom message, HTTP status codes, and response data.
- I recommend you to extends `Ug.Response.Response` class to create your own response

## Example of how to use the core library

> NB: I recommend you to @see all tests in `tests` projects to get more about examples.

1. Creating a custom request and handling missing/unauthorized fields

- Extends `Ug.Request.Request` to create custom application request objects.
- Define the possible fields in the `RequestPossibleFields` methods.

```csharp
namespace DefaultNamespace;

using Ug.Request;

public class AppRequest : Request
{
    protected override Dictionary<string, object> RequestPossibleFields => new Dictionary<string, object>
    {
        {"param1", true }, // true means fields is required, false means fields is not required (can not be set into the request)
        {"param2", true }
    };
    
    protected override void ApplyConstraintsOnRequestFields(Dictionary<string, object> requestData)
    {
        // this method is not mandatory
        // but you can add here your logic to validate the request fields if you need to make it
        // throws exception if needed
        
        throw new BadRequestContentException(
           new Dictionary<string, object>
           {
               { "message", "custom.errors_message" },
               { "details", requestData },
           });
    }
}

// without "ApplyConstraintsOnRequestFields" method
public class AppRequest : Request
{
    protected override Dictionary<string, object> RequestPossibleFields => new Dictionary<string, object>
    {
        {
            "param1", new Dictionary<string, object>()
            {
                {"type", false }
            }
        },
        {"param2", false }
    };
}

// handling missing required fields
Dictionary<string, object> payload = new Dictionary<string, object>
{
    {"param1", "value1" },
};
        
try
{
    IRequest request = _customRequest.CreateFromPayload(payload); 
}
catch (BadRequestContentException error)
{
    var errorDetails = error.Format();
    Assert.Equal(Status.Error.GetValue(), errorDetails["status"].ToString());
    Assert.Equal(StatusCode.BadRequest.GetValue(), errorDetails["error_code"]);
    Assert.Contains("missing.required.fields", errorDetails["message"].ToString());
    
    var details = (Dictionary<string, object>)errorDetails["details"];
    var missingFields = (Dictionary<string, string>)details["missing_fields"];
    
    Assert.True(missingFields.ContainsKey("param2"));
    Assert.Equal("required", missingFields["param2"]);
}

// handling unknown fields
Dictionary<string, object> payload = new Dictionary<string, object>
{
    {
        "param1", new Dictionary<string, object>
         {
            "deep", new Dictionary<string, object>
            {
                {"illegalField", "value4" }
            }
        }
    },
    {"param2", 3 },
    {"notAllowedField", "value2" }
};

try
{
    IRequest request = _customRequest.CreateFromPayload(payload); 
}
catch (BadRequestContentException error)
{
    var errorDetails = error.GetDetails();
    var missingFields = (List<string>)errorDetails["unrequired_fields"];
    
    Assert.Contains("illegal.fields", error.GetMessage());
    Assert.Contains("notAllowedField", missingFields);
    Assert.Contains("param1.deep", missingFields);
}

// when request is successfully created
Dictionary<string, object> payload = new Dictionary<string, object>
{
    {"param1", "value1" },
    {"param2", 3 },
};

IRequest request = _customRequest.CreateFromPayload(payload);
request.GetRequestId() // request Guid
"21600ac2-5e6e-48e5-af15-3aace4722362"

request.ToArray() // request payload as dictionary
{
  "param1": "value1",
  "param2": {
    "param3": "value3",
    "param4": {
      "param5": 5
    }
  }
}

// get request fields
var param1Value = request.Get<string>("param1"); // value1
var param5Value = request.Get<int>("param2.param4.param5"); // nested params (5)
var unKnownParams = request.Get<object>("param2.unkownParams"); // return null if not exists (null)
var othersUnKnownParams = request.Get<int>("param2.unkownParams", 4); // return param2.unkownParams value if exists or return 4 if not exists 
```

#### Exceptions handling

You can create a custom exception to add more context when errors are triggered. You have to extend `Ug.LibException.BaseException`
to create you custom exception.

Some methods are available to get exception information.

```csharp
namespace Ug.LibException;

public class BadRequestContentException : BaseException
{
    protected override int errorCode { get; } = StatusCode.BadRequest.GetValue();

    public BadRequestContentException(Dictionary<string, object> errors)
        : base(errors)
    {}
}

// exceptions 
error.Format() // format as dictonary
{
  "status": "error",
  "error_code": 400,
  "message": "missing.required.fields",
  "details": {
    "missing_fields": {
      "param2": "required"
    }
  }
}

error.GetErrors(); // exception errors
{
  "message": "missing.required.fields",
  "details": {
    "missing_fields": {
      "param1.type": "required"
    }
  }
}

// or
{
  "message": "missing.required.fields",
  "details": {
    "error": "custom.message",
  }
}

exception.GetDetails(); // exception details
{
    "missing_fields": {
        "param1.type": "required"
    }
}
// or
{
  "unrequired_fields": [
    "notAllowedField",
    "param1.deep"
  ]
}

exception.GetDetailsMessage(); // exception detail message
"custom.message"
 
exception.GetMessage(); // exception message
"missing.required.fields"
```

2. Creating a custom presenter and use it

- Extends `Ug.Presenter.Presenter` and implements `Ug.Presenter.Presenter` to create your use presenter.

Presenter is call in usecase when you call `PresentResponse` method. The response is sent into the presenter and you can
use it in your `userInterface or presentation` layers.

```csharp
namespace DefaultNamespace;

using Ug.Presenter;

public class AppPresenter : Presenter
{
    public Dictionary<string, object> PresentResponseForApi()
    {
        // add your logic here
    }
    
    public Dictionary<string, object> PresentResponseForWebApp()
    {
        // add your logic here
    }
    
    public Dictionary<string, object> PresentResponseForMobileApp()
    {
        // add your logic here
    }
}
```

3. Creating a custom usecase and execute it

- Extends `Ug.Usecase.Usecase` and implements `Ug.Usecase.IUsecase` to create your use cases.
- Implement the `execute` method for the use case logic.

```csharp
namespace DefaultNamespace;

using Ug.Usecase;
using Ug.Response;

public class AppUsecase : Usecase
{
    // you can add constructor here with usecase dependencies
    
    public override Task Execute()
    {
        PresentResponse(
            Response.Create(
                success: true,
                statusCode: StatusCode.OK.GetValue(),
                message: "success.message",
                data: new Dictionary<string, object>
                {
                    { "field_1", "yes" }
                }
            )
        );
        return Task.CompletedTask;
    }
}
```

Call usecase without request, with presenter:

```csharp
IUsecase usecase = new AppUsecase();
IPresenter presenter = new AppPresenter();
usecase
    .WithPresenter(presenter)
    .Execute();

var formattedResponse = presenter.GetFormattedResponse();

{
  "status": "success",
  "code": 200,
  "message": "success.message",
  "data": {
    "field_1": "yes"
  }
}
```

Call usecase with request, with presenter:

```csharp
IRequest request = new AppRequest().CreateFromPayload(new Dictionary<string, object>
{
    { "field_1", "value_1" },
    { "field_2", new Dictionary<string, object> { { "field_3", 3 }, { "field_4", new [] { "nice" } } } }
});
IUsecase usecase = new AppUsecase();
IPresenter presenter = new AppPresenter();
usecase
    .WithRequest(request)
    .WithPresenter(presenter)
    .Execute();

var formattedResponse = presenter.GetFormattedResponse();

{
  "status": "success",
  "code": 200,
  "message": "success.message",
  "data": {
    "field_1": "yes"
  }
}
```

Call usecase with request, without presenter:

```csharp
IRequest request = new AppRequest().CreateFromPayload(new Dictionary<string, object>
{
    { "field_1", "value_1" },
    { "field_2", new Dictionary<string, object> { { "field_3", 3 }, { "field_4", new [] { "nice" } } } }
});
IUsecase usecase = new AppUsecase();
usecase
    .WithRequest(request)
    .Execute();

// you can not call PresentResponse method into the usecase.
// If you throw an exception into the usecase
var exception = await Assert.ThrowsAsync<ContactListAlreadyExistsException>(async () =>
{
    await _usecase
        .WithRequest(_request.CreateFromPayload(payload))
        .WithPresenter(_presenter)
        .Execute();
});
Dictionary<string, object> errors = exception.Format();
```

## License

- Written and copyrighted ©2023-present by Ulrich Geraud AHOGLA. <iamcleancoder@gmail.com>
- Clean architecture core is open-sourced software licensed under the [MIT license](http://www.opensource.org/licenses/mit-license.php)