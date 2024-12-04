// <copyright file="Usecase.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.Response;
using Ug.Request;
using Ug.Presenter;

namespace Ug.Usecase
{
    public abstract class Usecase : IUsecase
    {
        private IRequest _request = default!;
        private IPresenter _presenter = default!;

        public abstract void Execute();

        public IUsecase WithRequest(IRequest request)
        {
            _request = request;
            return this;
        }

        public IUsecase WithPresenter(IPresenter presenter)
        {
            _presenter = presenter;
            return this;
        }

        protected void PresentResponse(IResponse response)
        {
            _presenter.Present(response);
        }

        protected Dictionary<string, object> GetRequestData()
        {
            return _request.ToArray();
        }

        protected string GetRequestId()
        {
            return _request.GetRequestId();
        }

        protected T GetField<T>(string fieldName, object? defaultValue = null)
        {
            var value = _request.Get<T>(fieldName, defaultValue);

            if (value == null && defaultValue == null)
            {
                return default!;
            }

            return value ?? (T)defaultValue!;
        }
    }
}