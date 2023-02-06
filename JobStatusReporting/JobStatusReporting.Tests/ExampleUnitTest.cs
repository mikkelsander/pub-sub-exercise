using FluentAssertions;
using JobStatusReporting;
using Xunit;

namespace JobStatusReporting.Tests
{
  public class UnitTests
  {

    [Fact]
    public async void TestJobStatusCollector()
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

      CancellationTokenSource cts = new CancellationTokenSource(5000);

      var index = 0;
      await foreach (var report in reportStream.WithCancellation(cts.Token))
      {
        report.Should().Be(testReports[index]);
        collectedReports.Add(report);
        index++;
      }

      collectedReports.Count.Should().Be(testReports.Count);
      collectedReports.Should().BeEquivalentTo(testReports);
    }
  }
}

