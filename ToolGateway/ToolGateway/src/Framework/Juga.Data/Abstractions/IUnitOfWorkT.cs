namespace Juga.Data.Abstractions;

public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : IUnitOfWork;