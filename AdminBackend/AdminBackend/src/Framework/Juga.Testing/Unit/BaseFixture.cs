using Xunit;

namespace Juga.Testing.Unit
{
    public abstract class BaseFixture<TFixture> : IClassFixture<TFixture> where TFixture : class
    {
        public string GetFixtureName() => GetType()?.FullName ?? Guid.NewGuid().ToString("D");
    }
}
