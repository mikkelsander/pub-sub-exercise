# JobStatusReporting

JobStatusReporting is a small library intended to make it easy for your long running batch jobs to produce a live stream of status progression to multiple consumers using Redis Pup/Sub.

# How to use

The `JobStatusReport` class represents the model of the streamed status updates:

```
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
```

Your batch jobs must implement the `IBatchJob` interface, which exposes a single method that returns a `JobStatusReport` instance:

```
public interface IBatchJob
{
  public JobStatusReport GetStatusReport();
}
```

Note: This interface does not enforce any rules on how you should implement or update the state in your batch jobs. That is completely up to you to decide.

An instance of the `JobStatusCollector` class can be used to generate an async stream of a batch job's status progression:

```
  IBatchJob batchJob = new SomeLongRunningJob();
  JobStatusCollector collector = new JobStatusCollector();

  IAsyncEnumerable<JobStatusReport> reportStream = collector.CollectStatusReportsAsync(job: batchJob, pullIntervalMS: 2000);
```

An instance of the `JobStatusPublisher` class can be used to publish an async stream of status updates to an arbitrary channel on Redis:

```
  string redisConnectionString = "localhost:6379";

  await using (JobStatusPublisher publisher = JobStatusPublisher.Build(redisConnectionString)
  {
    publisher.PublishAsync(reportStream, "someRedisChannel")
  }
```

An instance of the `JobStatusSubscriber` class can be used to subscribe to Redis channels and consume the status updates in a callback function:

```
  string redisConnectionString = "localhost:6379";

  await using (JobStatusSubscriber subscriber = JobStatusSubscriber.Build(redisConnectionString)
  {
    subscriber.SubscribeAsync("test", (report) =>
    {
      Console.WriteLine(report.Print());
    });
  }
```

# Running Redis locally using Docker

```
docker run -p 6379:6379 redis
```
