namespace Orc.NuGetExplorer.Pagination
{
    using System.Linq;
    using Catel.Logging;

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

        public PageContinuation(PageContinuation continuation, bool onlyLocal) 
            : this(continuation)
        {
            OnlyLocal = onlyLocal;
        }


        public int LastNumber { get => _lastNumber; private set => _lastNumber = value; }

        public int Size => _pageSize;

        public int Next => LastNumber + 1;

        public int Current => _lastNumber;

        public bool OnlyLocal { get; set; } = false;

        public bool IsValid => Source.PackageSources.Any() || OnlyLocal;

        public PackageSourceWrapper Source { get; private set; }

        public int GetNext()
        {
            Log.Debug($"Got next {Size} positions, starts from {Next}");

            var next = Next;

            LastNumber += Size;

            return next;
        }

        public int GetNext(int count)
        {
            Log.Debug($"Got next {count} positions, starts from {Next}");

            var next = Next;

            LastNumber += Size;

            return next;
        }

        public int GetPrevious()
        {
            LastNumber -= Size;
            return Next;
        }
    }
}
