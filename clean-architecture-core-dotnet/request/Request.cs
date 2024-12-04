// <copyright file="Request.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.LibException;

namespace Ug.Request
{

    public abstract class Request : RequestFilter, IRequest
    {
        private string _requestId = default!;
        private Dictionary<string, object> _requestParams = new();

        public IRequest CreateFromPayload(Dictionary<string, object> payload)
        {
            var requestValidationResult = RequestPayloadFilter(payload);
            ThrowMissingFieldsExceptionIfNeeded((Dictionary<string, string>)requestValidationResult["missing_fields"]);
            ThrowUnRequiredFieldsExceptionIfNeeded((List<string>)requestValidationResult["unauthorized_fields"]);
            
            ApplyConstraintsOnRequestFields(payload);
            
            _requestId = Guid.NewGuid().ToString();
            _requestParams = payload;

            return this;
        }

        protected void ThrowMissingFieldsExceptionIfNeeded(Dictionary<string, string> missingFields)
        {
            if (missingFields.Any())
            {
                throw new BadRequestContentException(new Dictionary<string, object>
                {
                    { "message", "missing.required.fields" },
                    { "details", new Dictionary<string, object> { { "missing_fields", missingFields } } },
                });
            }
        }

        protected void ThrowUnRequiredFieldsExceptionIfNeeded(List<string> unauthorizedFields)
        {
            if (unauthorizedFields.Any())
            {
                throw new BadRequestContentException(new Dictionary<string, object>
                {
                    { "message", "illegal.fields" },
                    { "details", new Dictionary<string, object> { { "unrequired_fields", unauthorizedFields } } },
                });
            }
        }

        protected virtual void ApplyConstraintsOnRequestFields(Dictionary<string, object> requestData)
        {
            // To be implemented by subclasses if necessary.
        }

        public string GetRequestId()
        {
            return _requestId;
        }

        public Dictionary<string, object> ToArray()
        {
            return _requestParams;
        }

        public T? Get<T>(string fieldName, object? defaultValue = null)
        {
            object? data = _requestParams;

            foreach (var key in fieldName.Split('.'))
            {
                if (data is not Dictionary<string, object> dataMap)
                {
                    return (T?)defaultValue;
                }

                if (!dataMap.TryGetValue(key, out var value))
                {
                    return (T?)defaultValue;
                }

                data = value;
            }

            if (data is T typedData)
            {
                return typedData;
            }

            return (T?)defaultValue;
        }
    }
}