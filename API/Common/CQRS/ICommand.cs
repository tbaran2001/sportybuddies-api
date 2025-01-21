﻿using MediatR;

namespace API.Common.CQRS;

public interface ICommand: ICommand<Unit>
{

}

public interface ICommand<out TResponse>: IRequest<TResponse>
{
    
}