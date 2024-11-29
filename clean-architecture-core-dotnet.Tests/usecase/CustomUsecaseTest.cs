// <copyright file="CustomUsecaseTest.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Tests.Usecase;

using Ug.LibException;
using Ug.Presenter;
using Ug.Enums;
using Ug.Response;
using Ug.Request;
using Ug.Usecase;

public class CustomUsecaseTest
{
    [Fact]
    public void TestExecuteCustomUsecaseWithPresenterWithoutRequest()
    {
        IUsecase usecase = this.GetUsecaseWithoutErrors();
        IPresenter presenter = this.GetPresenter();
        usecase.WithPresenter(presenter).Execute();

        var formattedResponse = presenter.GetFormattedResponse();

        Assert.Equal(Status.Success.GetValue(), formattedResponse["status"]);
        Assert.Equal(StatusCode.OK.GetValue(), formattedResponse["code"]);
        Assert.Equal("success.message", formattedResponse["message"]);
        Assert.Equal("yes", presenter.GetResponse().Get("field_1"));
    }

    [Fact]
    public void TestExecuteCustomUsecaseWithRequestWithoutPresenter()
    {
        IUsecase usecase = this.GetUsecaseWithErrors();
        IRequest request = this.GetRequest();

        try
        {
            usecase.WithRequest(request.CreateFromPayload(new Dictionary<string, object>
            {
                { "param1", "value1" },
                { "param2", 3 },
            })).Execute();
        }
        catch (BadRequestContentException error)
        {
            var errorDetails = error.Format();
            Assert.Equal(Status.Error.GetValue(), errorDetails["status"].ToString());
            Assert.Equal(StatusCode.BadRequest.GetValue(), errorDetails["error_code"]);
            Assert.Contains("custom.errors_message", errorDetails["message"].ToString());

            var details = (Dictionary<string, object>)errorDetails["details"];

            Assert.Equal("value1", details["param1"]);
            Assert.Equal(3, details["param2"]);
        }
    }

    [Fact]
    public void TestExecuteCustomUsecaseWithPresenterAndRequest()
    {
        IUsecase usecase = this.GetUsecaseWithoutErrors();
        IRequest request = this.GetRequest();
        IPresenter presenter = this.GetPresenter();
        usecase
            .WithRequest(request.CreateFromPayload(new Dictionary<string, object>
            {
                { "param1", "value1" },
                { "param2", 3 },
            }))
            .WithPresenter(presenter).Execute();

        var formattedResponse = presenter.GetFormattedResponse();

        Assert.Equal(Status.Success.GetValue(), formattedResponse["status"]);
        Assert.Equal(StatusCode.OK.GetValue(), formattedResponse["code"]);
        Assert.Equal("success.message", formattedResponse["message"]);
        Assert.Equal("yes", presenter.GetResponse().Get("field_1"));
    }

    private IRequest GetRequest()
    {
        return new CustomRequest();
    }

    private IUsecase GetUsecaseWithoutErrors()
    {
        return new CustomUsecaseWithoutErrors();
    }

    private IUsecase GetUsecaseWithErrors()
    {
        return new CustomUsecaseWithErrors();
    }

    private IUsecase GetUsecaseWithError()
    {
        return new CustomUsecaseWithErrors();
    }

    private IPresenter GetPresenter()
    {
        return new CustomPresenter();
    }

    private class CustomRequest : Request
    {
        protected override Dictionary<string, object> RequestPossibleFields => new Dictionary<string, object>
        {
            { "param1", true },
            { "param2", true },
        };
    }

    private class CustomUsecaseWithoutErrors : Usecase
    {
        public override void Execute()
        {
            this.PresentResponse(
                Response.Create(
                    success: true,
                    statusCode: StatusCode.OK.GetValue(),
                    message: "success.message",
                    data: new Dictionary<string, object>
                    {
                        { "field_1", "yes" },
                    }));
        }
    }

    private class CustomUsecaseWithErrors : Usecase
    {
        public override void Execute()
        {
            throw new BadRequestContentException(
               new Dictionary<string, object>
               {
                   { "message", "custom.errors_message" },
                   { "details", this.GetRequestData() },
               });
        }
    }

    private class CustomPresenter : Presenter
    {
    }
}