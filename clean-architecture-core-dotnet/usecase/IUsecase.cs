// <copyright file="IUsecase.cs" company="Ulrich Geraud AHOGLA">
// Copyright (c) Ulrich Geraud AHOGLA. All rights reserved.
// </copyright>

namespace Ug.Usecase;

using Ug.Request;
using Ug.Presenter;

public interface IUsecase
{
    void Execute();

    IUsecase WithRequest(IRequest request);

    IUsecase WithPresenter(IPresenter presenter);
}