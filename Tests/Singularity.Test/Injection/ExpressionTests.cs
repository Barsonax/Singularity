using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class ExpressionTests
    {
        [Fact]
        public void GetInstance_AsExpression()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var expression = container.GetInstance<Expression<Func<IPlugin>>>();

            //ASSERT
            var newexpression = Assert.IsType<NewExpression>(expression.Body);
            Assert.Equal(typeof(Plugin1), newexpression.Type);
            Assert.Empty(newexpression.Arguments);
        }

        [Fact]
        public void GetInstance_Inject_AsExpression()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var pluginExpression = container.GetInstance<PluginExpression>();

            //ASSERT
            var expression = pluginExpression.Expression;
            var newexpression = Assert.IsType<NewExpression>(expression.Body);
            Assert.Equal(typeof(Plugin1), newexpression.Type);
            Assert.Empty(newexpression.Arguments);
        }

        [Fact]
        public void GetInstance_AsEnumerableExpression()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            config.Register<IPlugin, Plugin2>();
            config.Register<IPlugin, Plugin3>();
            var container = new Container(config);

            //ACT
            var expressions = container.GetInstance<IReadOnlyList<Expression<Func<IPlugin>>>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<Expression<Func<IPlugin>>>>(expressions);
            Assert.Equal(3, expressions.Count);
        }
    }
}
