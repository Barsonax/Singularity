﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Singularity.Collections;
using Xunit;

namespace Singularity.Test.Collections
{
    public class SinglyLinkedListNodeTests
    {
        [Fact]
        public void Enumerate_ConcreteType()
        {
            SinglyLinkedListNode<int> list = null;

            list = list.Add(1);
            list = list.Add(2);
            list = list.Add(3);

            int[] results = list.ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal(3, results[0]);
            Assert.Equal(2, results[1]);
            Assert.Equal(1, results[2]);
        }

        [Fact]
        public void Enumerate_EnumerableGenericType()
        {
            SinglyLinkedListNode<int> list = null;

            list = list.Add(1);
            list = list.Add(2);
            list = list.Add(3);

            int[] results = ((IEnumerable<int>)list).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal(3, results[0]);
            Assert.Equal(2, results[1]);
            Assert.Equal(1, results[2]);
        }

        [Fact]
        public void Enumerate_EnumerableNonGenericType()
        {
            SinglyLinkedListNode<int> list = null;

            list = list.Add(1);
            list = list.Add(2);
            list = list.Add(3);

            int[] results = ((IEnumerable)list).OfType<int>().ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal(3, results[0]);
            Assert.Equal(2, results[1]);
            Assert.Equal(1, results[2]);
        }
    }
}
