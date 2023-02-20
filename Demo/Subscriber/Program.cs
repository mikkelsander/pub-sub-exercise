/**
  In this demo we subscribe to a redis channel provided in the command line arguments.
  You can run multiple instances of this app and subscribe to the same or different channels to test out the pub/sub functionality.
**/

using JobStatusReporting;

if (args.Length < 1)
{
  Console.WriteLine("Job status report channel name missing in command arguments");
  Environment.Exit(0);
}

var reportChannel = args[0];
var redisConnectionString = "localhost:6379";

await using (RedisJobStatusSubscriber subscriber = RedisJobStatusSubscriber.Build(redisConnectionString))
{
  await subscriber.SubscribeAsync(reportChannel, (JobStatusReport report) =>
  {
    Console.WriteLine("Job: " + reportChannel);
    Console.WriteLine(report.Print());
  });

  Console.ReadLine();
}