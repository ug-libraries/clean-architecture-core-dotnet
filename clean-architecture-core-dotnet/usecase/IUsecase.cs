// <copyright file="IUsecase.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

using Ug.Request;
using Ug.Presenter;

namespace Ug.Usecase
{
    public interface IUsecase
    {
        Task Execute();

        IUsecase WithRequest(IRequest request);

        IUsecase WithPresenter(IPresenter presenter);
    }
}