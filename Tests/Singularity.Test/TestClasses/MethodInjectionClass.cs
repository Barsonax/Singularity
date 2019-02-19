using System;

using Singularity.Attributes;

namespace Singularity.Test.TestClasses
{
    public class MethodInjectionClass
    {
        public ITestService10? TestService10 { get; private set; }

        public void FakeInject(ITestService10 testService10)
        {
            throw new NotImplementedException();
        }

        [Inject]
        public void Inject(ITestService10 testService10)
        {
            TestService10 = testService10;
        }
    }
}
