using JobStatusReporting;

public interface IJobStatusPublisher
{
  public Task PublishAsync(IAsyncEnumerable<JobStatusReport> statusReports, string channel);
  public Task UnsubscribeAsync(string channel);
  public Task UnsubscribeAllAsync();
}