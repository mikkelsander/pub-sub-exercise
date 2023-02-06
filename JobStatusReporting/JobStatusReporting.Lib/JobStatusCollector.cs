using System.Runtime.CompilerServices;

namespace JobStatusReporting;

public class JobStatusCollector
{
  public async IAsyncEnumerable<JobStatusReport> CollectStatusReportsAsync(IBatchJob job, int pullIntervalMS, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    Console.WriteLine($"Collecting status reports from job '{nameof(job)}'");

    var keepCollecting = true;

    while (keepCollecting)
    {
      await Task.Delay(pullIntervalMS, cancellationToken);

      JobStatusReport report = job.GetStatusReport();

      if (report.Status == JobStatus.Completed || report.Status == JobStatus.Error)
      {
        keepCollecting = false;
      }

      yield return report;
    }
  }
}