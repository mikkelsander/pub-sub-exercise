using JobStatusReporting;


if (args.Length < 1)
{
  Console.WriteLine("Job status report channel name missing in command arguments");
  Environment.Exit(0);
}

var reportChannel = args[0];
var redisConnectionString = "localhost:6379";

await using (JobStatusSubscriber subscriber = JobStatusSubscriber.Build(redisConnectionString))
{
  await subscriber.SubscribeAsync(reportChannel, (JobStatusReport report) =>
  {
    Console.WriteLine("Job: " + reportChannel);
    Console.WriteLine(report.Print());
  });

  Console.ReadLine();
}




