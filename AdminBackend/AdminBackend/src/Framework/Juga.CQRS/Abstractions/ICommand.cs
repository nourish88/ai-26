
namespace Juga.CQRS.Abstractions;
//INFO: Without Response
public interface ICommand : ICommand<Unit>;
public interface  ICommand<out TResponse>:IRequest<TResponse>;