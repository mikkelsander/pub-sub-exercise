namespace JobStatusReporting;
using StackExchange.Redis;
using System.Text.Json;

public class JobStatusSubscriber
{
  private ISubscriber _conn;

  public JobStatusSubscriber(ConnectionMultiplexer redisConnection)
  {
    _conn = redisConnection.GetSubscriber();
  }

  public static JobStatusSubscriber Build(string redisConnectionString)
  {
    ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
    return new JobStatusSubscriber(redisConnection);
  }

  public async Task SubscribeAsync(string channel, Action<JobStatusReport> callback)
  {
    Console.WriteLine($"Subscribing to job status reports on channel '{channel}'");

    await _conn.SubscribeAsync(channel, (channel, jsonReport) =>
    {
      try
      {
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