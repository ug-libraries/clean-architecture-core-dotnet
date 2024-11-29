// <copyright file="BaseException.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.LibException;

using Ug.Response;
using Ug.Enums;

public class BaseException : Exception, IException
{
    protected virtual int errorCode { get; } = StatusCode.BadRequest.GetValue();
    protected Dictionary<string, object> _errors;

    public BaseException(Dictionary<string, object> errors)
        : base(errors.ContainsKey("message") ? errors["message"].ToString() : string.Empty)
    {
        _errors = errors;
    }

    public Dictionary<string, object> Format()
    {
        return new Dictionary<string, object>
        {
            { "status", Status.Error.GetValue() },
            { "error_code", errorCode },
            { "message", GetMessage() },
            { "details", GetDetails() },
        };
    }

    public Dictionary<string, object> GetErrors()
    {
        return _errors;
    }

    public Dictionary<string, object> GetDetails()
    {
        return _errors.ContainsKey("details") ? (Dictionary<string, object>)_errors["details"] : new Dictionary<string, object>();
    }

    public string GetDetailsMessage()
    {
        if (GetDetails().TryGetValue("error", out var error) && error is string errorMessage)
        {
            return errorMessage;
        }
        return string.Empty;
    }

    public string GetMessage()
    {
        return Message;
    }
}