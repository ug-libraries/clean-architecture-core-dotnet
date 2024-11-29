// <copyright file="Request.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Request;

using Ug.LibException;

public abstract class Request : RequestFilter, IRequest
{
    protected string requestId = default!;
    protected Dictionary<string, object> requestParams = new Dictionary<string, object>();

    public IRequest CreateFromPayload(Dictionary<string, object> payload)
    {
        var requestValidationResult = this.RequestPayloadFilter(payload);
        this.ThrowMissingFieldsExceptionIfNeeded((Dictionary<string, string>)requestValidationResult["missing_fields"]);
        this.ThrowUnRequiredFieldsExceptionIfNeeded((List<string>)requestValidationResult["unauthorized_fields"]);
        
        this.ApplyConstraintsOnRequestFields(payload);
        
        this.requestId = Guid.NewGuid().ToString();
        this.requestParams = payload;

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
        return this.requestId;
    }

    public Dictionary<string, object> ToArray()
    {
        return this.requestParams;
    }

    public object? Get(string fieldName, object? defaultValue = null)
    {
        object? data = this.requestParams;
        foreach (var key in fieldName.Split('.'))
        {
            if (data is not Dictionary<string, object> dataMap)
            {
                return defaultValue;
            }

            if (!dataMap.ContainsKey(key))
            {
                return defaultValue;
            }

            data = dataMap[key];
            if (data == null)
            {
                return defaultValue;
            }
        }

        return data;
    }
}