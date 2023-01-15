using System;
using Medallion.Threading;
using Medallion.Threading.Redis;

namespace ProblemDetailsExample.Jobs
{
    public class DemoBackgroundService : BackgroundService
    {
        private readonly ILogger<DemoBackgroundService> _logger;

        private readonly RedisDistributedSynchronizationProvider _distributedSynchronizationProvider;

        public DemoBackgroundService(ILogger<DemoBackgroundService> logger, RedisDistributedSynchronizationProvider distributedSynchronizationProvider)
        {
            _logger = logger;
            _distributedSynchronizationProvider = distributedSynchronizationProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    await Execute(cancellationToken);

                    await Task.Delay(18000, cancellationToken);
                }
                while (!cancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                _logger.LogError($"DemoBackgroundService has error. Error Message: {ex.Message}");
            }

        }

        private async Task Execute(CancellationToken cancellationToken)
        {
            using (await _distributedSynchronizationProvider.AcquireLockAsync("DemoBackgroundService", null, cancellationToken))
            {
                //get process
            }
        }
    }
}

