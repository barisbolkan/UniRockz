using System;
using System.Threading.Tasks;
using Hangfire;

namespace UniRockz.Threading.Core
{
    public interface ICronJob
    {
        string Name { get; }
        string Schedule { get; }
        bool Enabled { get; }

        Task Execute(IJobCancellationToken cancellationToken);
    }
}
