using System;
using Hangfire;

namespace UniRockz.Threading.Core
{
    internal class BackgroundProcessor : IBackgroundProcessor
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public BackgroundProcessor(IBackgroundJobClient jobClient,
            IRecurringJobManager recurringJobManager)
        {
            _jobClient = jobClient;
            _recurringJobManager = recurringJobManager;
        }

        public string AddJob(Action action) =>
            _jobClient.Enqueue(() => action());

        public void AddCronJob(ICronJob cronJob)
        {
            if (cronJob == null)
                throw new ArgumentNullException("cronJob");

            if (cronJob.Enabled)
            {
                _recurringJobManager.AddOrUpdate(cronJob.Name,
                    () => cronJob.Execute(JobCancellationToken.Null),
                    cronJob.Schedule);
            }
            else
            {
                _recurringJobManager.RemoveIfExists(cronJob.Name);
            }
        }
    }
}
