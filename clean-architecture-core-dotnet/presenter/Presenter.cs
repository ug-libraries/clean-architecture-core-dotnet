// <copyright file="Presenter.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.Response;

namespace Ug.Presenter
{
    public abstract class Presenter : IPresenter
    {
        protected IResponse _response = default!;

        public void Present(IResponse response)
        {
            _response = response;
        }

        public IResponse GetResponse()
        {
            return _response;
        }

        public Dictionary<string, object> GetFormattedResponse()
        {
            return _response.Output();
        }
    }
}