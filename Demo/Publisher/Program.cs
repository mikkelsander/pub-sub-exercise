/**
  In this demo we create three different jobs using the "FakeBatchJob" class and publish them on three different channels
**/

using JobStatusReporting;

string redisConnectionString = "localhost:6379";

await using (RedisJobStatusPublisher publisher = RedisJobStatusPublisher.Build(redisConnectionString))
{
  var collector = new JobStatusCollector();
  var jobs = new List<FakeBatchJob>();

  for (var i = 1; i <= 3; i++)
  {
    jobs.Add(new FakeBatchJob(name: "job_" + i, updateIntervalMS: i * 3000));
  }

  var tasks = new List<Task>();

  for (var i = 0; i < jobs.Count; i++)
  {
    tasks.Add(publisher.PublishAsync(collector.CollectStatusReportsAsync(jobs[i], pullIntervalMS: (i + 1) * 2000), jobs[i].Name));
    tasks.Add(jobs[i].DoWork());
  }

  Console.ReadLine();
}