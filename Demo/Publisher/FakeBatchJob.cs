namespace JobStatusReporting;

public class FakeBatchJob : IBatchJob
{
  public string Name { get; set; }

  private int _updateIntervalMS;

  private JobStatusReport _status;

  public FakeBatchJob(string name, int updateIntervalMS)
  {
    Name = name;

    _updateIntervalMS = updateIntervalMS;

    _status = new JobStatusReport
    {
      PercentageCompleted = 0,
      Status = JobStatus.NotStarted,
    };

  }

  public JobStatusReport GetStatusReport()
  {
    return _status;
  }

  public async Task DoWork()
  {
    Console.WriteLine($"Job {Name} starting up....");

    try
    {
      _status.Status = JobStatus.Running;

      Random rnd = new Random();

      while (_status.PercentageCompleted < 100)
      {
        _status.PercentageCompleted = Math.Min(_status.PercentageCompleted + rnd.Next(0, 10), 100);
        _status.Message = $"{_status.PercentageCompleted * 100} files uploaded";
        _status.ErrorMessage = "None";
        await Task.Delay(_updateIntervalMS);
      }

      _status.PercentageCompleted = 100;
      _status.Status = JobStatus.Completed;

      Console.WriteLine($"Job {Name} completed.");
    }
    catch
    {
      _status.Status = JobStatus.Completed;
    }
  }
}