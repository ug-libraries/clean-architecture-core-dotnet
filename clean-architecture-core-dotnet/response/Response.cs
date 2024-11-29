// <copyright file="Response.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.Enums;

namespace Ug.Response;

public class Response : IResponse
{
    private readonly bool success;
    private readonly int statusCode;
    private readonly string message;
    private readonly Dictionary<string, object> data;

    public Response(bool success, int statusCode, string message, Dictionary<string, object> data)
    {
        this.success = success;
        this.statusCode = statusCode;
        this.message = message;
        this.data = data ?? new Dictionary<string, object>();
    }

    public static Response Create(bool success, int statusCode, string message, Dictionary<string, object> data)
    {
        return new Response(success, statusCode, message, data);
    }

    public bool IsSuccess()
    {
        return this.success;
    }

    public int GetStatusCode()
    {
        return this.statusCode;
    }

    public string GetMessage()
    {
        return this.message;
    }

    public Dictionary<string, object> GetData()
    {
        return this.data;
    }

    public object? Get(string fieldName)
    {
        object? value = this.data;
        foreach (var key in fieldName.Split('.'))
        {
            if (value is not Dictionary<string, object> map || !map.TryGetValue(key, out var tempValue))
            {
                return null;
            }

            value = tempValue;
        }

        return value;
    }

    public Dictionary<string, object> Output()
    {
        var output = new Dictionary<string, object>
        {
            { "status", this.GetStatusLabel() },
            { "code", this.statusCode },
            { "message", this.message },
        };

        foreach (var entry in this.MapDataKeyAccordingToResponseStatus())
        {
            output[entry.Key] = entry.Value;
        }

        return output;
    }

    private string GetStatusLabel()
    {
        return this.success ? Status.Success.GetValue() : Status.Error.GetValue();
    }

    private IEnumerable<KeyValuePair<string, object>> MapDataKeyAccordingToResponseStatus()
    {
        return new Dictionary<string, object>
        {
            { this.IsSuccess() ? "data" : "details", this.GetData() },
        };
    }
}
