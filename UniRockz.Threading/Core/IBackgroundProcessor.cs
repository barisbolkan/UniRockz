using System;

namespace UniRockz.Threading.Core
{
    public interface IBackgroundProcessor
    {
        string AddJob(Action action);
        void AddCronJob(ICronJob cronJob);
    }
}
