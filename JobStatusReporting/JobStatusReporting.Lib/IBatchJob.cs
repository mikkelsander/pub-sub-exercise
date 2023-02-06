namespace JobStatusReporting;
public interface IBatchJob
{
  public JobStatusReport GetStatusReport();
}