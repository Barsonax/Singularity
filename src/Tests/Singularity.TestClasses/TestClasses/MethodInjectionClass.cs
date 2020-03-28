using System;

namespace Singularity.TestClasses.TestClasses
{
    public class MethodInjectionClass
    {
        public ITestService10 TestService10Field;
        public ITestService10 TestService10 { get; set; }

        public ITestService10 TestService10noSetter { get; }

        public void FakeInject(ITestService10 testService10)
        {
            throw new NotImplementedException();
        }

        public object FakeMethodWithReturn()
        {
            return new object();
        }

        public void Inject(ITestService10 testService10)
        {
            TestService10 = testService10;
        }

        private void PrivateInject(ITestService10 testService10)
        {
            TestService10 = testService10;
        }
    }
}
