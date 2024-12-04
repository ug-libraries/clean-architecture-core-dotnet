// <copyright file="Response.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.Enums;

namespace Ug.Response
{
    public class Response : IResponse
    {
        private readonly bool _success;
        private readonly int _statusCode;
        private readonly string _message;
        private readonly Dictionary<string, object> _data;

        public Response(bool success, int statusCode, string message, Dictionary<string, object> data)
        {
            _success = success;
            _statusCode = statusCode;
            _message = message;
            _data = data ?? new Dictionary<string, object>();
        }

        public static Response Create(bool success, int statusCode, string message, Dictionary<string, object> data)
        {
            return new Response(success, statusCode, message, data);
        }

        public bool IsSuccess()
        {
            return _success;
        }

        public int GetStatusCode()
        {
            return _statusCode;
        }

        public string GetMessage()
        {
            return _message;
        }

        public Dictionary<string, object> GetData()
        {
            return _data;
        }

        public T? Get<T>(string fieldName)
        {
            object? value = _data;

            foreach (var key in fieldName.Split('.'))
            {
                if (value is not Dictionary<string, object> map || !map.TryGetValue(key, out var tempValue))
                {
                    return default;
                }

                value = tempValue;
            }

            if (value is T typedValue)
            {
                return typedValue;
            }

            return default;
        }

        public Dictionary<string, object> Output()
        {
            var output = new Dictionary<string, object>
            {
                { "status", GetStatusLabel() },
                { "code", _statusCode },
                { "message", _message },
            };

            foreach (var entry in MapDataKeyAccordingToResponseStatus())
            {
                output[entry.Key] = entry.Value;
            }

            return output;
        }

        private string GetStatusLabel()
        {
            return _success ? Status.Success.GetValue() : Status.Error.GetValue();
        }

        private IEnumerable<KeyValuePair<string, object>> MapDataKeyAccordingToResponseStatus()
        {
            return new Dictionary<string, object>
            {
                { IsSuccess() ? "data" : "details", GetData() },
            };
        }
    }
}
