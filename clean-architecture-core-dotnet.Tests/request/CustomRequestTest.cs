// <copyright file="CustomRequestTest.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using BaseRequest = Ug.Request;
using Ug.LibException;
using Ug.Response;
using Ug.Enums;

namespace Ug.Tests.Request
{
    public class CustomRequestTest
    {
        [Fact]
        public void ShouldBeAbleToBuildCustomNewRequest()
        {
            BaseRequest.IRequest customRequest = new CustomRequest();
            BaseRequest.IRequest request = customRequest.CreateFromPayload(new Dictionary<string, object>());
            Assert.IsType<CustomRequest>(request);
        }

        [Fact]
        public void ShouldBeAbleToBuildCustomNewRequestAndGetRequestId()
        {
            BaseRequest.IRequest customRequest = new CustomRequest();
            BaseRequest.IRequest request = customRequest.CreateFromPayload(new Dictionary<string, object>());
            Assert.NotNull(request.GetRequestId());
        }

        [Fact]
        public void ShouldBeAbleToBuildCustomNewRequestWithNoMandaoryParamsAndGetRequestId()
        {
            CustomRequestWithNoMandatoryParams customRequest = new CustomRequestWithNoMandatoryParams();
            BaseRequest.IRequest request = customRequest.CreateFromPayload(new Dictionary<string, object>());
            Assert.NotNull(request.GetRequestId());
        }

        [Fact]
        public void ShouldBeAbleToBuildCustomNewRequestWithParameters()
        {
            CustomRequestWithMandatoryParams customRequest = new CustomRequestWithMandatoryParams();
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "param1", "value1" },
                {
                    "param2", new Dictionary<string, object>()
                    {
                        { "param3", "value3" },
                        {
                            "param4", new Dictionary<string, object>()
                            {
                                { "param5", 5 }
                            }
                        }
                    }
                },
            };
            
            BaseRequest.IRequest request = customRequest.CreateFromPayload(payload);
            
            Assert.NotNull(request.GetRequestId());

            var param1Value = request.Get<string>("param1");
            var param5Value = request.Get<int>("param2.param4.param5");
            var unKnownParams = request.Get<object>("param2.unkownParams");
            var othersUnKnownParams = request.Get<int>("param2.unkownParams", 4);

            Assert.Equal("value1", param1Value);
            Assert.Equal(5, param5Value);
            Assert.Null(unKnownParams);
            Assert.Equal(4, othersUnKnownParams);

            Assert.Equal(payload, request.ToArray());
        }

        [Fact]
        public void ShouldNotBeAbleToBuildCustomNewRequestWithMissingParameters()
        {
            CustomRequestWithMandatoryParams customRequest = new CustomRequestWithMandatoryParams();
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "param1", "value1" },
            };

            try
            {
                BaseRequest.IRequest request = customRequest.CreateFromPayload(payload);
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
        }

        [Fact]
        public void ShouldNotBeAbleToBuildCustomNewRequestWithMissingPartialNestedParameters()
        {
            CustomRequestWithPartialMandatoryParams customRequest = new CustomRequestWithPartialMandatoryParams();
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                {
                    "param1", new Dictionary<string, object>()
                    {
                    }
                },
            };

            try
            {
                BaseRequest.IRequest request = customRequest.CreateFromPayload(payload);
            }
            catch (BadRequestContentException error)
            {
                var errors = error.GetErrors();
                Assert.Contains("missing.required.fields", errors["message"].ToString());

                var details = (Dictionary<string, object>)errors["details"];
                var missingFields = (Dictionary<string, string>)details["missing_fields"];

                Assert.True(missingFields.ContainsKey("param1.type"));
                Assert.Equal("required", missingFields["param1.type"]);
            }
        }

        [Fact]
        public void ShouldNotBeAbleToBuildCustomNewRequestWithNotAllowedParameters()
        {
            CustomRequestWithNotAllowedParams customRequest = new CustomRequestWithNotAllowedParams();
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                {
                    "param1", new Dictionary<string, object>()
                    {
                        { "type", "value1" },
                        { "notAllowedField", "value2" },
                        {
                            "deep", new Dictionary<string, object>
                            {
                                { "illegalField", "value4" }
                            }
                        },
                    }
                },
            };

            try
            {
                BaseRequest.IRequest request = customRequest.CreateFromPayload(payload);
            }
            catch (BadRequestContentException error)
            {
                var errorDetails = error.GetDetails();
                var missingFields = (List<string>)errorDetails["unrequired_fields"];

                Assert.Contains("illegal.fields", error.GetMessage());
                Assert.Contains("param1.notAllowedField", missingFields);
                Assert.Contains("param1.deep", missingFields);
            }
        }

        private class CustomRequest : BaseRequest.Request
        {
        }

        private class CustomRequestWithMandatoryParams : BaseRequest.Request
        {
            protected override Dictionary<string, object> RequestPossibleFields => new Dictionary<string, object>
            {
                { "param1", true },
                { "param2", true },
            };
        }

        private class CustomRequestWithPartialMandatoryParams : BaseRequest.Request
        {
            protected override Dictionary<string, object> RequestPossibleFields => new Dictionary<string, object>
            {
                {
                    "param1", new Dictionary<string, object>()
                    {
                        { "type", true },
                    }
                },
                { "param2", false },
            };
        }

        private class CustomRequestWithNoMandatoryParams : BaseRequest.Request
        {
            protected override Dictionary<string, object> RequestPossibleFields => new Dictionary<string, object>
            {
                {
                    "param1", new Dictionary<string, object>()
                    {
                        { "type", false },
                    }
                },
                { "param2", false },
            };
        }

        private class CustomRequestWithNotAllowedParams : BaseRequest.Request
        {
            protected override Dictionary<string, object> RequestPossibleFields => new Dictionary<string, object>
            {
                {
                    "param1", new Dictionary<string, object>()
                    {
                        { "type", true },
                    }
                },
                { "param2", false },
            };
        }
    }
}