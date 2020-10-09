using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace UniRockz.Domain.Utils
{
    public static class AsyncExtensions
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }

        public static async Task<K> FlatMap<T, K>(this Task<T> task, Func<T, Task<K>> f)
        {
            return await f(await task);
        }

        public static async Task<K> Map<T, K>(this Task<T> task, Func<T, K> f)
        {
            return f(await task);
        }

        public static async Task<T> FlatMap<T>(this Task task, Func<Task<T>> f)
        {
            await task;
            return (await f());
        }

        public static async Task<T> Map<T>(this Task task, Func<T> f)
        {
            await task;
            return f();
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> enumerable,
            Func<T, Task> func, int paralellism = DataflowBlockOptions.Unbounded,
            TaskScheduler scheduler = null)
        {
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = paralellism
            };

            if (scheduler != null)
                options.TaskScheduler = scheduler;

            var block = new ActionBlock<T>(func, options);

            foreach (var item in enumerable)
                block.Post(item);

            block.Complete();

            return block.Completion;
        }
    }
}
