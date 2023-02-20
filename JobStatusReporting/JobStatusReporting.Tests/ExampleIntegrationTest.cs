using FluentAssertions;
using JobStatusReporting;
using Xunit;

namespace JobStatusReporting.Tests
{
  public class IntegrationTests : IClassFixture<RedisContainerFixture>
  {

    private readonly RedisContainerFixture _redisFixture;

    public IntegrationTests(RedisContainerFixture redisContainerFixture)
    {
      _redisFixture = redisContainerFixture;
    }

    [Fact]
    public async void ExampleIntegrationTest()
    {

      await using (RedisJobStatusPublisher publisher = RedisJobStatusPublisher.Build(_redisFixture.ConnectionString))
      await using (RedisJobStatusSubscriber subscriber = RedisJobStatusSubscriber.Build(_redisFixture.ConnectionString))
      {
        var testReports = new List<JobStatusReport>() {
          new JobStatusReport
          {
            PercentageCompleted = 15,
            Status = JobStatus.Running,
            Message = "Test ...",
          },
          new JobStatusReport
            {
              PercentageCompleted = 61,
              Status = JobStatus.Running,
              Message = "Test ...",
            },
          new JobStatusReport
          {
            PercentageCompleted = 100,
            Status = JobStatus.Completed,
            Message = "Test ...",
          }
        };

        IBatchJob job = new TestJob(testReports);
        JobStatusCollector collector = new JobStatusCollector();
        List<JobStatusReport> collectedReports = new List<JobStatusReport>();
        IAsyncEnumerable<JobStatusReport> reportStream = collector.CollectStatusReportsAsync(job, 0);

        var receivedReports = new List<JobStatusReport>();
        var tasks = new List<Task>();

        tasks.Add(publisher.PublishAsync(reportStream, "test"));
        tasks.Add(subscriber.SubscribeAsync("test", (report) =>
        {
          receivedReports.Add(report);
        }));

        Task.WaitAll(tasks.ToArray());
        await Task.Delay(1000);

        receivedReports.Count.Should().Be(testReports.Count);
        receivedReports.Should().BeEquivalentTo(testReports);

      }
    }
  }
}

