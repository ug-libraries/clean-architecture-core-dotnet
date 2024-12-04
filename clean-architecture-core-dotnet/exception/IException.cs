// <copyright file="IException.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.LibException
{
    public interface IException
    {
        Dictionary<string, object> Format();

        Dictionary<string, object> GetErrors();

        Dictionary<string, object> GetDetails();

        string GetDetailsMessage();

        string GetMessage();
    }
}