using DistributedLock101;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
Environment.SetEnvironmentVariable("PROCESS_GUID", Guid.NewGuid().ToString(), EnvironmentVariableTarget.Process);

var host = builder.Build();
host.Run();