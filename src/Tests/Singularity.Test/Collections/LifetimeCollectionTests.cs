using Xunit;

namespace Singularity.Test.Collections
{
    public class LifetimeCollectionTests
    {
        [Fact]
        public void Contains_ComparesOnType()
        {
            LifetimeCollection collection = new ILifetime[] {new Transient(), new Transient(),};

            Assert.True(collection.Contains(new Transient()));
            Assert.False(collection.Contains(new PerGraph()));
            Assert.Contains(new Transient(), collection, LifetimeCollection.Comparer);
            Assert.DoesNotContain(new PerContainer(), collection, LifetimeCollection.Comparer);
        }
    }
}
