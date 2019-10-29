namespace Orc.NuGetExplorer.Pagination
{
    using Catel.Logging;
    using System.Linq;

    public class PageContinuation
    {
        private int _lastNumber = -1;

        private readonly int _pageSize = -1;

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public PageContinuation(int pageSize, PackageSourceWrapper packageSourceWrapper)
        {
            _pageSize = pageSize;

            Source = packageSourceWrapper;
        }

        public PageContinuation(PageContinuation continuation)
        {
            Source = continuation.Source;
            _lastNumber = continuation.Current;
        }


        public int LastNumber { get => _lastNumber; private set => _lastNumber = value; }

        public int Size => _pageSize;

        public int Next => LastNumber + 1;

        public int Current => _lastNumber;

        public bool IsValid => Source.PackageSources.Any();

        public PackageSourceWrapper Source { get; private set; }

        public int GetNext()
        {
            Log.Info($"Got next {Size} positions, starts from {Next}");

            var next = Next;

            LastNumber = LastNumber + Size;

            return next;
        }

        public int GetNext(int count)
        {
            Log.Info($"Got next {count} positions, starts from {Next}");

            var next = Next;

            LastNumber = LastNumber + Size;

            return next;
        }

        public int GetPrevious()
        {
            LastNumber = LastNumber - Size;
            return Next;
        }
    }
}
