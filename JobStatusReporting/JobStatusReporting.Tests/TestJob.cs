using JobStatusReporting;

public class TestJob : IBatchJob
{
  private int _counter = 0;
  private List<JobStatusReport> _reports;

  public TestJob(List<JobStatusReport> reports)
  {
    _reports = reports;
  }

  public JobStatusReport GetStatusReport()
  {
    var report = _reports[_counter];
    _counter++;
    return report;
  }
}