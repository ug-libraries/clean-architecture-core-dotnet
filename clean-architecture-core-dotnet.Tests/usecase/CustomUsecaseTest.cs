// <copyright file="CustomUsecaseTest.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.LibException;
using BasePresenter = Ug.Presenter;
using Ug.Response;
using BaseRequest = Ug.Request;
using BaseUsecase = Ug.Usecase;
using Ug.Enums;

namespace Ug.Tests.Usecase
{
    public class CustomUsecaseTest
    {
        [Fact]
        public void TestExecuteCustomUsecaseWithPresenterWithoutRequest()
        {
            BaseUsecase.IUsecase usecase = GetUsecaseWithoutErrors();
            BasePresenter.IPresenter presenter = GetPresenter();
            usecase.WithPresenter(presenter).Execute();

            var formattedResponse = presenter.GetFormattedResponse();

            Assert.Equal(Status.Success.GetValue(), formattedResponse["status"]);
            Assert.Equal(StatusCode.OK.GetValue(), formattedResponse["code"]);
            Assert.Equal("success.message", formattedResponse["message"]);
            Assert.Equal("yes", presenter.GetResponse().Get<string>("field_1"));
        }

        [Fact]
        public void TestExecuteCustomUsecaseWithRequestWithoutPresenter()
        {
            BaseUsecase.IUsecase usecase = GetUsecaseWithErrors();
            BaseRequest.IRequest request = GetRequest();

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
                Assert.Equal(Status.Error.GetValue(), errorDetails["status"]);
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
            BaseUsecase.IUsecase usecase = GetUsecaseWithoutErrors();
            BaseRequest.IRequest request = GetRequest();
            BasePresenter.IPresenter presenter = GetPresenter();
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
            Assert.Equal("yes", presenter.GetResponse().Get<string>("field_1"));
        }

        private BaseRequest.IRequest GetRequest()
        {
            return new CustomRequest();
        }

        private BaseUsecase.IUsecase GetUsecaseWithoutErrors()
        {
            return new CustomUsecaseWithoutErrors();
        }

        private BaseUsecase.IUsecase GetUsecaseWithErrors()
        {
            return new CustomUsecaseWithErrors();
        }

        private BaseUsecase.IUsecase GetUsecaseWithError()
        {
            return new CustomUsecaseWithErrors();
        }

        private BasePresenter.IPresenter GetPresenter()
        {
            return new CustomPresenter();
        }

        private class CustomRequest : BaseRequest.Request
        {
            protected override Dictionary<string, object> RequestPossibleFields => new()
            {
                { "param1", true },
                { "param2", true },
            };
        }

        private class CustomUsecaseWithoutErrors : BaseUsecase.Usecase
        {
            public override Task Execute()
            {
                PresentResponse(
                    Ug.Response.Response.Create(
                        success: true,
                        statusCode: StatusCode.OK.GetValue(),
                        message: "success.message",
                        data: new Dictionary<string, object>
                        {
                            { "field_1", "yes" },
                        }
                    )
                );

                return Task.CompletedTask;
            }
        }

        private class CustomUsecaseWithErrors : BaseUsecase.Usecase
        {
            public override Task Execute()
            {
                throw new BadRequestContentException(
                new Dictionary<string, object>
                {
                    { "message", "custom.errors_message" },
                    { "details", GetRequestData() },
                });
            }
        }

        private class CustomPresenter : BasePresenter.Presenter
        {
        }
    }
}