// <copyright file="StatusCode.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Response
{
    public enum StatusCode
    {
        OK = 200,
        NoContent = 204,
        BadRequest = 400,
    }

    public static class StatusCodeExtensions
    {
        public static int GetValue(this StatusCode statusCode)
        {
            return (int)statusCode;
        }
    }
}