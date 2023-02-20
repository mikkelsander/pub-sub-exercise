using JobStatusReporting;

public interface IJobStatusSubscriber
{
  public Task SubscribeAsync(string channel, Action<JobStatusReport> callback);
  public Task UnsubscribeAsync(string channel);
  public Task UnsubscribeAllAsync();
}