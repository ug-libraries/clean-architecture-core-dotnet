// <copyright file="IPresenter.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.Response;

namespace Ug.Presenter
{
    public interface IPresenter
    {
        void Present(IResponse response);

        IResponse GetResponse();

        Dictionary<string, object> GetFormattedResponse();
    }
}