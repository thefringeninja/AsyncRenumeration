using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncRenumeration
{
    internal class Program
    {
        public static async ValueTask<int> Main(string[] args)
        {
            var thing = new Thing();

            var result = thing.GetStuff();

            if (result is NoStuff)
            {
                Console.WriteLine("No stuff");
                return 1;
            }

            await foreach (var i in result)
            {
                Console.WriteLine(i);
            }

            return 0;
        }
    }

    class Thing
    {
        public async IAsyncEnumerable<int> GetStuff()
        {
            var (values, empty) = await GetPage();

            return empty ? default(NoStuff) : YieldStuff(values);
        }

        private async IAsyncEnumerable<int> YieldStuff(int[] values)
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }

        private async ValueTask<(int[], bool)> GetPage() => (Enumerable.Range(0, 20).ToArray(),
            Convert.ToBoolean(new Random().Next(0, 1)));
    }

    public struct NoStuff : IAsyncEnumerable<int>, IEquatable<NoStuff>
    {
        public bool Equals(NoStuff other) => true;
        public override bool Equals(object obj) => obj is NoStuff other && Equals(other);
        public override int GetHashCode() => 0;
        public static bool operator ==(NoStuff left, NoStuff right) => true;
        public static bool operator !=(NoStuff left, NoStuff right) => false;

        public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => AsyncEnumerable.Empty<int>().GetAsyncEnumerator();
    }
}