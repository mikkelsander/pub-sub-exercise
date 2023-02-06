namespace JobStatusReporting;
using StackExchange.Redis;
using System.Text.Json;

public class JobStatusPublisher : IAsyncDisposable
{
  private ISubscriber _conn;

  public JobStatusPublisher(ConnectionMultiplexer redisConnection)
  {
    _conn = redisConnection.GetSubscriber();
  }

  public static JobStatusPublisher Build(string redisConnectionString)
  {
    ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
    return new JobStatusPublisher(redisConnection);
  }

  public async Task PublishAsync(IAsyncEnumerable<JobStatusReport> statusReports, string channel)
  {
    await Task.Run(async () =>
    {
      Console.WriteLine($"Publishing reports on channel '{channel}'");
      await foreach (var report in statusReports)
      {
        try
        {
          var jsonReport = JsonSerializer.Serialize(report);
          await _conn.PublishAsync(channel, jsonReport, CommandFlags.FireAndForget);
        }
        catch (Exception e)
        {
          Console.Error.Write(e.Message);
        }
      }
    });
  }

  public async Task UnsubscribeAsync(string channel)
  {
    await _conn.UnsubscribeAsync(channel);
  }

  public async Task UnsubscribeAllAsync()
  {
    await _conn.UnsubscribeAllAsync();
  }

  public async ValueTask DisposeAsync()
  {
    await _conn.UnsubscribeAllAsync();
  }

}