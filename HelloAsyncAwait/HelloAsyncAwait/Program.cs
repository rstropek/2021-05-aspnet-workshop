using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HelloAsyncAwait
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // ASYNC
            var fileContent = await File.ReadAllTextAsync(@"C:\temp\2021-05-aspnet-workshop\HelloAsyncAwait\HelloAsyncAwait\Program.cs");
            Console.WriteLine(fileContent);
            Console.WriteLine(await AddAsync(21, 21));

            // PARALLEL
            var numbers1 = new [] { 1, 2, 3, 4, 5 };
            var numbers2 = new[] { 10, 11, 12, 13, 14 };

            var tasks = new Task<int>[numbers1.Length];
            for (var i = 0; i < numbers1.Length; i++) tasks[i] = AddAsync(numbers1[i], numbers2[i]);
            await Task.WhenAll(tasks);

            foreach (var t in tasks) Console.WriteLine(t.Result);
        }

        static async Task<int> AddAsync(int x, int y)
        {
            // Simulate long-running math algorithm
            await Task.Delay(100);

            return x + y;
        }
    }
}
