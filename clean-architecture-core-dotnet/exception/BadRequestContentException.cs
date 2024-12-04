// <copyright file="BadRequestContentException.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.Response;

namespace Ug.LibException
{
    public class BadRequestContentException : BaseException
    {
        protected override int errorCode { get; } = StatusCode.BadRequest.GetValue();
        public BadRequestContentException(Dictionary<string, object> errors)
            : base(errors)
        {
        }
    }
}