namespace JobStatusReporting;

public class JobStatusReport
{
  public int PercentageCompleted { get; set; } = 0;
  public JobStatus Status { get; set; } = JobStatus.NotStarted;
  public string? Message { get; set; }
  public string? ErrorMessage { get; set; }

  public string Print()
  {
    return $"Status report: \nStatus: {Status}\nPercentage completed: {PercentageCompleted}\nMessage: {Message}\nError message: {ErrorMessage}\n";
  }
}

public enum JobStatus
{
  NotStarted,
  Running,
  Completed,
  Error,
}