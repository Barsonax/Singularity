using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    internal sealed class ImmutableAvlNode<TKey, TValue>
    {
        public static readonly ImmutableAvlNode<TKey, TValue> Empty = new ImmutableAvlNode<TKey, TValue>();
        public readonly TKey Key;
        public readonly TValue Value;
        public readonly ImmutableList<KeyValue<TKey, TValue>> Duplicates;
        public readonly int HashCode;
        public readonly ImmutableAvlNode<TKey, TValue> Left;
        public readonly ImmutableAvlNode<TKey, TValue> Right;
        public readonly int Height;
        public readonly bool IsEmpty;

        public ImmutableAvlNode(TKey key, TValue value, ImmutableAvlNode<TKey, TValue> avlNode)
        {
            Duplicates = avlNode.Duplicates.Add(new KeyValue<TKey, TValue>(key, value));
            Key = avlNode.Key;
            Value = avlNode.Value;
            Height = avlNode.Height;
            HashCode = avlNode.HashCode;
            Left = avlNode.Left;
            Right = avlNode.Right;
        }

        public ImmutableAvlNode(TKey key, TValue value, ImmutableAvlNode<TKey, TValue> left, ImmutableAvlNode<TKey, TValue> right)
        {
            int balance = left.Height - right.Height;

            if (balance == -2)
            {
                if (right.IsLeftHeavy())
                {
                    right = RotateRight(right);
                }

                // Rotate left
                Key = right.Key;
                Value = right.Value;
                Left = new ImmutableAvlNode<TKey, TValue>(key, value, left, right.Left);
                Right = right.Right;
            }
            else if (balance == 2)
            {
                if (left.IsRightHeavy())
                {
                    left = RotateLeft(left);
                }

                // Rotate right
                Key = left.Key;
                Value = left.Value;
                Right = new ImmutableAvlNode<TKey, TValue>(key, value, left.Right, right);
                Left = left.Left;
            }
            else
            {
                Key = key;
                Value = value;
                Left = left;
                Right = right;
            }

            Height = 1 + Math.Max(Left.Height, Right.Height);

            Duplicates = ImmutableList<KeyValue<TKey, TValue>>.Utils.Empty;

            HashCode = RuntimeHelpers.GetHashCode(Key);
        }

        public ImmutableAvlNode<TKey, TValue> Add(TKey key, TValue value)
        {
            if (IsEmpty)
            {
                return new ImmutableAvlNode<TKey, TValue>(key, value, this, this);
            }

            int hashCode = RuntimeHelpers.GetHashCode(key);

            if (hashCode > HashCode)
            {
                return AddToRightBranch(this, key, value);
            }

            if (hashCode < HashCode)
            {
                return AddToLeftBranch(this, key, value);
            }

            return new ImmutableAvlNode<TKey, TValue>(key, value, this);
        }

        private static ImmutableAvlNode<TKey, TValue> AddToLeftBranch(ImmutableAvlNode<TKey, TValue> tree, TKey key, TValue value)
        {
            return new ImmutableAvlNode<TKey, TValue>(tree.Key, tree.Value, tree.Left.Add(key, value), tree.Right);
        }

        private static ImmutableAvlNode<TKey, TValue> AddToRightBranch(ImmutableAvlNode<TKey, TValue> tree, TKey key, TValue value)
        {
            return new ImmutableAvlNode<TKey, TValue>(tree.Key, tree.Value, tree.Left, tree.Right.Add(key, value));
        }

        public IEnumerable<KeyValue<TKey, TValue>> InOrder()
        {
            return InOrder(this);
        }

        private static IEnumerable<KeyValue<TKey, TValue>> InOrder(ImmutableAvlNode<TKey, TValue> avlNode)
        {
            if (!avlNode.IsEmpty)
            {
                foreach (KeyValue<TKey, TValue> left in InOrder(avlNode.Left))
                {
                    yield return new KeyValue<TKey, TValue>(left.Key, left.Value);
                }

                yield return new KeyValue<TKey, TValue>(avlNode.Key, avlNode.Value);

                for (int i = 0; i < avlNode.Duplicates.Items.Length; i++)
                {
                    yield return avlNode.Duplicates.Items[i];
                }

                foreach (KeyValue<TKey, TValue> right in InOrder(avlNode.Right))
                {
                    yield return new KeyValue<TKey, TValue>(right.Key, right.Value);
                }
            }
        }

        private ImmutableAvlNode()
        {
            IsEmpty = true;
            Duplicates = ImmutableList<KeyValue<TKey, TValue>>.Utils.Empty;
        }

        private static ImmutableAvlNode<TKey, TValue> RotateLeft(ImmutableAvlNode<TKey, TValue> left)
        {
            return new ImmutableAvlNode<TKey, TValue>(
                left.Right.Key,
                left.Right.Value,
                new ImmutableAvlNode<TKey, TValue>(left.Key, left.Value, left.Right.Left, left.Left),
                left.Right.Right);
        }

        private static ImmutableAvlNode<TKey, TValue> RotateRight(ImmutableAvlNode<TKey, TValue> right)
        {
            return new ImmutableAvlNode<TKey, TValue>(
                right.Left.Key,
                right.Left.Value,
                right.Left.Left,
                new ImmutableAvlNode<TKey, TValue>(right.Key, right.Value, right.Left.Right, right.Right));
        }

        private bool IsLeftHeavy()
        {
            return Left.Height > Right.Height;
        }

        private bool IsRightHeavy()
        {
            return Right.Height > Left.Height;
        }
    }
}
