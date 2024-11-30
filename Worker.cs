using System.Diagnostics;
using Medallion.Threading.MySql;
using Medallion.Threading.SqlServer;

namespace DistributedLock101;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IConfiguration _configuration;
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {

            var id = Environment.GetEnvironmentVariable("PROCESS_GUID");

            string connectionString =
                "server=distributedlock.db;user=user;password=pass@word123!@#;database=DistributedLockDatabase";

            var @lock = new MySqlDistributedLock("Lock", connectionString);

            try
            {
                await using var handle =
                    await @lock.TryAcquireAsync(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);

                if (handle != null)
                {
                    _logger.LogInformation(id);
                    await Task.Delay(15000, cancellationToken);
                }
                else
                {
                    _logger.LogError("The resource is locked");
                }
            }
            catch (Exception)
            {
                _logger.LogCritical("Not connected");
            }


            await Task.Delay(1000, cancellationToken);
        }
    }
}