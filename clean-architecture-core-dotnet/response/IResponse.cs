// <copyright file="IResponse.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Response
{
    public interface IResponse
    {
        bool IsSuccess();

        int GetStatusCode();

        string GetMessage();

        T? Get<T>(string fieldName);

        Dictionary<string, object> GetData();

        Dictionary<string, object> Output();
    }
}