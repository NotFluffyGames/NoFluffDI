using System.Linq;

namespace NotFluffy.NoFluffRx
{
    public class BinarySemaphore : ObservablesGroup<bool>
    {
        public BinarySemaphore() : base(sources => sources.Any(v => v))
        {
        }

        public void Lock() => AddValue(true);
    }
}
