// <copyright file="Presenter.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Presenter;

using Ug.Response;

public abstract class Presenter : IPresenter
{
    protected IResponse response = default!;

    public void Present(IResponse response)
    {
        this.response = response;
    }

    public IResponse GetResponse()
    {
        return this.response;
    }

    public Dictionary<string, object> GetFormattedResponse()
    {
        return this.response.Output();
    }
}