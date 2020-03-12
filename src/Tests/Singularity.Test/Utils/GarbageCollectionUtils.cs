using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

namespace Singularity.Test.Utils
{
    public class GarbageCollectionUtils
    {
        public static void CheckIfCleanedUp(Func<WeakReference> func)
        {
            CheckIfCleanedUp(() => new[] { func() });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CheckIfCleanedUp(Func<WeakReference[]> func)
        {
            var weakRefs = func();

            // In some cases unloading something happens async and can take some time such as with AssemblyLoadContext. Workaround this by retrying a couple of times..
            for (int i = 0; weakRefs.Any(x => x.IsAlive) && i < 10; i++)
            {
                GC.Collect(2, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();
            }

            Assert.Collection(weakRefs, x => Assert.False(x.IsAlive));
        }
    }
}
