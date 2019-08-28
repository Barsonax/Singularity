using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Singularity.Exceptions;
using Xunit;

namespace Singularity.Test.Exceptions
{
    public class ExceptionSerializationTests
    {
        [Theory]
        [ClassData(typeof(ExceptionSerializationTheoryData))]
        public void AuthorizationRequiredException_serialization_deserialization_test(Exception originalException)
        {
            //ARRANGE
            var buffer = new byte[4096];
            var ms = new MemoryStream(buffer);
            var ms2 = new MemoryStream(buffer);
            var formatter = new BinaryFormatter();

            //ACT
            formatter.Serialize(ms, originalException);
            var deserializedException = (Exception)formatter.Deserialize(ms2);

            //ASSERT
            Assert.Equal(originalException.InnerException.Message, deserializedException.InnerException.Message);
            Assert.Equal(originalException.Message, deserializedException.Message);
        }
    }

    public class ExceptionSerializationTheoryData : TheoryData<Exception>
    {
        public ExceptionSerializationTheoryData()
        {
            var message = "message";
            var inner = new Exception("inner");
            Add(new BindingConfigException(message, inner));
            Add(new CannotAutoResolveConstructorException(message, inner));
            Add(new CircularDependencyException(new[] { typeof(int), typeof(float), typeof(decimal), typeof(CircularDependencyException) }, inner));
            Add(new DependencyNotFoundException(typeof(int), inner));
            Add(new EnumerableRegistrationException(message, inner));
            Add(new InterfaceExpectedException(message, inner));
            Add(new InvalidExpressionArgumentsException(message, inner));
            Add(new InvalidEnumValueException<ServiceAutoDispose>(ServiceAutoDispose.Always, inner));
            Add(new NoConstructorException(message, inner));
            Add(new RegistrationAlreadyExistsException(message, inner));
            Add(new SingularityException(message, inner));
            Add(new SingularityAggregateException(message, new[] { inner }));
            Add(new TypeNotAssignableException(message, inner));
            Add(new AbstractTypeResolveException(message, inner));
        }
    }
}
