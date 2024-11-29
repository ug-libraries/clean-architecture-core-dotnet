// <copyright file="Usecase.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Usecase;

using Ug.Response;
using Ug.Request;
using Ug.Presenter;

public abstract class Usecase : IUsecase
{
    private IRequest request = default!;
    private IPresenter presenter = default!;

    public abstract void Execute();

    public IUsecase WithRequest(IRequest request)
    {
        this.request = request;
        return this;
    }

    public IUsecase WithPresenter(IPresenter presenter)
    {
        this.presenter = presenter;
        return this;
    }

    protected void PresentResponse(IResponse response)
    {
        this.presenter.Present(response);
    }

    protected Dictionary<string, object> GetRequestData()
    {
        return this.request.ToArray();
    }

    protected string GetRequestId()
    {
        return this.request.GetRequestId();
    }

    protected object GetField(string fieldName, object? defaultValue = null)
    {
        return this.request.Get(fieldName, defaultValue);
    }
}