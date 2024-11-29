// <copyright file="CustomResponseTests.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Tests.Response;

using Ug.Response;
using Ug.Enums;

public class CustomResponseTests
{
    [Fact]
    public void TestGetCustomSuccessResponse()
    {
        IResponse instanceResponse = Response.Create(
            success: true,
            statusCode: StatusCode.OK.GetValue(),
            message: "success.message",
            data: new Dictionary<string, object>());

        Assert.True(instanceResponse.IsSuccess());
        Assert.Equal(StatusCode.OK.GetValue(), instanceResponse.GetStatusCode());
        Assert.Equal("success.message", instanceResponse.GetMessage());
        Assert.Equal(new Dictionary<string, object>(), instanceResponse.GetData());
    }

    [Fact]
    public void TestGetCustomFailedResponse()
    {
        IResponse instanceResponse = Response.Create(
            success: false,
            statusCode: StatusCode.OK.GetValue(),
            message: "success.message",
            data: new Dictionary<string, object>());

        Assert.False(instanceResponse.IsSuccess());
        Assert.Equal(StatusCode.OK.GetValue(), instanceResponse.GetStatusCode());
        Assert.Equal("success.message", instanceResponse.GetMessage());
        Assert.Equal(new Dictionary<string, object>(), instanceResponse.GetData());
    }

    [Fact]
    public void TestGetCustomResponseWithContent()
    {
        IResponse instanceResponse = GetInstanceResponse();

        Assert.True(instanceResponse.IsSuccess());
        Assert.Equal(StatusCode.OK.GetValue(), instanceResponse.GetStatusCode());
        Assert.Equal("success.response", instanceResponse.GetMessage());
        Assert.Equal("yes", instanceResponse.Get("field_1"));
        Assert.Null(instanceResponse.Get("field_6"));
        Assert.Equal(3, instanceResponse.Get("field_2.field_3"));

        var result = instanceResponse.Get("field_2.field_4.field_5");
        Assert.IsType<string[]>(result);
        Assert.Equal(new[] { "nice" }, (string[])result);

        Dictionary<string, object> expectedOutput = instanceResponse.Output();
        Dictionary<string, object> expectedData = (Dictionary<string, object>)expectedOutput["data"];

        Assert.Equal(Status.Success.GetValue(), expectedOutput["status"]);
        Assert.Equal(StatusCode.OK.GetValue(), expectedOutput["code"]);
        Assert.Equal("success.response", expectedOutput["message"]);
        Assert.Equal("yes", expectedData["field_1"]);

        Dictionary<string, object> retrievedField2 = (Dictionary<string, object>)expectedData["field_2"];
        Assert.Equal(3, retrievedField2["field_3"]);
    }

    private static Response GetInstanceResponse()
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "field_1", "yes" },
            {
                "field_2", new Dictionary<string, object>
                {
                    { "field_3", 3 },
                    {
                        "field_4", new Dictionary<string, object>
                        {
                            { "field_5", new[] { "nice" } },
                            { "field_2", true }
                        }
                    }
                }
            },
        };

        return Response.Create(
            success: true,
            statusCode: StatusCode.OK.GetValue(),
            message: "success.response",
            data: data);
    }
}
