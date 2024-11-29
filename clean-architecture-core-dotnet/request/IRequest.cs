// <copyright file="IRequest.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Request;

public interface IRequest
{
    IRequest CreateFromPayload(Dictionary<string, object> payload);

    string GetRequestId();

    Dictionary<string, object> ToArray();

    object? Get(string fieldName, object? defaultValue = null);
}