// <copyright file="RequestFilter.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Request
{
    public abstract class RequestFilter
    {
        protected virtual Dictionary<string, object> RequestPossibleFields => new();

        protected Dictionary<string, object> RequestPayloadFilter(Dictionary<string, object> requestPayload)
        {
            var result = new Dictionary<string, object>
            {
                { "unauthorized_fields", FindUnAuthorizedFields(requestPayload, RequestPossibleFields, string.Empty) },
                { "missing_fields", FindMissingFields(RequestPossibleFields, requestPayload, string.Empty) },
            };
            return result;
        }

        protected static List<string> FindUnAuthorizedFields(
            Dictionary<string, object> requestPayload,
            Dictionary<string, object> authorizedFields,
            string prefix)
        {
            List<string> unAuthorizedFields = new List<string>();

            foreach (var entry in requestPayload)
            {
                string field = entry.Key;
                object value = entry.Value;
                string fullKey = prefix + field;

                if (!authorizedFields.ContainsKey(field))
                {
                    unAuthorizedFields.Add(fullKey);
                }
                else if (IsMap(value) && IsMap(authorizedFields[field]))
                {
                    unAuthorizedFields.AddRange(
                        FindUnAuthorizedFields(
                            (Dictionary<string, object>)value,
                            (Dictionary<string, object>)authorizedFields[field],
                            fullKey + "."
                        )
                    );
                }
            }

            return unAuthorizedFields;
        }

        protected static Dictionary<string, string> FindMissingFields(
            Dictionary<string, object> authorizedFields,
            Dictionary<string, object> requestPayload,
            string prefix)
        {
            Dictionary<string, string> missingFields = new Dictionary<string, string>();

            foreach (var entry in authorizedFields)
            {
                string field = entry.Key;
                object value = entry.Value;
                string fullKey = prefix + field;

                if (value is bool && (bool)value && !requestPayload.ContainsKey(field))
                {
                    missingFields.Add(fullKey, IsMap(value) ? "required field type not matching object" : "required");
                }
                else if (requestPayload.ContainsKey(field))
                {
                    if (IsMap(value) && !(requestPayload[field] is Dictionary<string, object>))
                    {
                        missingFields.Add(fullKey, "required field type not matching array");
                    }
                    else if (IsMap(requestPayload[field]) && IsMap(value))
                    {
                        var nestedMissingFields = FindMissingFields(
                            (Dictionary<string, object>)value,
                            (Dictionary<string, object>)requestPayload[field],
                            fullKey + ".");
                        foreach (var nestedField in nestedMissingFields)
                        {
                            missingFields.Add(nestedField.Key, nestedField.Value);
                        }
                    }
                }
            }

            return missingFields;
        }

        protected static bool IsMap(object value)
        {
            return value is Dictionary<string, object>;
        }
    }
}