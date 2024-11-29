// <copyright file="IPresenter.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Presenter;

using Ug.Response;

public interface IPresenter
{
    void Present(IResponse response);

    IResponse GetResponse();

    Dictionary<string, object> GetFormattedResponse();
}