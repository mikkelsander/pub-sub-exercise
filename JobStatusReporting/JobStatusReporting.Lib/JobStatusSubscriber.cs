namespace JobStatusReporting;
using StackExchange.Redis;
using System.Text.Json;

public class JobStatusSubscriber : IAsyncDisposable
{
  private ISubscriber _conn;

  public JobStatusSubscriber(ISubscriber redisPubSubConnection)
  {
    _conn = redisPubSubConnection;
  }

  public static JobStatusSubscriber Build(string redisConnectionString)
  {
    ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
    return new JobStatusSubscriber(redisConnection.GetSubscriber());
  }

  public async Task SubscribeAsync(string channel, Action<JobStatusReport> callback)
  {
    Console.WriteLine($"Subscribing to job status reports on channel '{channel}'");

    await _conn.SubscribeAsync(channel, (RedisChannel channel, RedisValue jsonReport) =>
    {
      try
      {
        if (jsonReport.IsNullOrEmpty)
        {
          throw new Exception("Received empty report/null value");
        }

        var report = JsonSerializer.Deserialize<JobStatusReport>(jsonReport);
        callback(report);
      }
      catch (Exception e)
      {
        Console.Error.Write(e.Message);
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