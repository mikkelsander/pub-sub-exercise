
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace JobStatusReporting.Tests
{
  public class RedisContainerFixture : IAsyncLifetime
  {
    public IContainer Container { get; }

    public string ConnectionString { get; set; }

    private int PORT = 6379;

    public RedisContainerFixture()
    {
      Container = new ContainerBuilder()
        .WithName("redis-integration-test")
        .WithImage("redis")
        .WithExposedPort(PORT)
        .WithPortBinding(PORT, true)
        .Build();
    }

    public async Task InitializeAsync()
    {
      await Container.StartAsync();
      ConnectionString = $"{Container.Hostname}:{Container.GetMappedPublicPort(PORT)}";
    }

    public async Task DisposeAsync()
    {
      await Container.StopAsync();
    }
  }
}
