using System;
using System.Collections.Generic;

namespace Singularity.Collections
{
    internal sealed class ImmutableAvlNode<TKey, TValue>
    {
        public static readonly ImmutableAvlNode<TKey, TValue> Empty = new ImmutableAvlNode<TKey, TValue>();
        public readonly int Height;
        public readonly bool IsEmpty;
        public readonly KeyValue<TKey, TValue> KeyValue;
        public readonly ImmutableList<KeyValue<TKey, TValue>> Duplicates;
        public readonly ImmutableAvlNode<TKey, TValue>? Left;
        public readonly ImmutableAvlNode<TKey, TValue>? Right;

        public ImmutableAvlNode<TKey, TValue> Add(KeyValue<TKey, TValue> keyValue)
        {
            if (IsEmpty)
            {
                return new ImmutableAvlNode<TKey, TValue>(keyValue, this, this);
            }

            if (keyValue.HashCode > KeyValue.HashCode)
            {
                return AddToRightBranch(this, keyValue);
            }

            if (keyValue.HashCode < KeyValue.HashCode)
            {
                return AddToLeftBranch(this, keyValue);
            }

            return new ImmutableAvlNode<TKey, TValue>(keyValue, this);
        }

        private ImmutableAvlNode()
        {
            IsEmpty = true;
            Duplicates = ImmutableList<KeyValue<TKey, TValue>>.Utils.Empty;
        }

        private ImmutableAvlNode(KeyValue<TKey, TValue> keyValue, ImmutableAvlNode<TKey, TValue> avlNode)
        {
            Duplicates = avlNode.Duplicates.Add(keyValue);
            KeyValue = avlNode.KeyValue;
            Height = avlNode.Height;
            Left = avlNode.Left;
            Right = avlNode.Right;
        }

        private ImmutableAvlNode(KeyValue<TKey, TValue> keyValue, ImmutableAvlNode<TKey, TValue> left, ImmutableAvlNode<TKey, TValue> right)
        {
            int balance = left.Height - right.Height;

            if (balance == -2)
            {
                if (right.IsLeftHeavy())
                {
                    right = RotateRight(right);
                }

                // Rotate left
                KeyValue = right.KeyValue;
                Left = new ImmutableAvlNode<TKey, TValue>(keyValue, left, right.Left!);
                Right = right.Right;
            }
            else if (balance == 2)
            {
                if (left.IsRightHeavy())
                {
                    left = RotateLeft(left);
                }

                // Rotate right
                KeyValue = left.KeyValue;
                Right = new ImmutableAvlNode<TKey, TValue>(keyValue, left.Right!, right);
                Left = left.Left;
            }
            else
            {
                KeyValue = keyValue;
                Left = left;
                Right = right;
            }

            Height = 1 + Math.Max(Left!.Height, Right!.Height);

            Duplicates = ImmutableList<KeyValue<TKey, TValue>>.Utils.Empty;
        }

        private static ImmutableAvlNode<TKey, TValue> AddToLeftBranch(ImmutableAvlNode<TKey, TValue> tree, KeyValue<TKey, TValue> keyValue)
        {
            return new ImmutableAvlNode<TKey, TValue>(tree.KeyValue, tree.Left!.Add(keyValue), tree.Right!);
        }

        private static ImmutableAvlNode<TKey, TValue> AddToRightBranch(ImmutableAvlNode<TKey, TValue> tree, KeyValue<TKey, TValue> keyValue)
        {
            return new ImmutableAvlNode<TKey, TValue>(tree.KeyValue, tree.Left!, tree.Right!.Add(keyValue));
        }

        public IEnumerable<KeyValue<TKey, TValue>> InOrder()
        {
            return InOrder(this);
        }

        private static IEnumerable<KeyValue<TKey, TValue>> InOrder(ImmutableAvlNode<TKey, TValue> avlNode)
        {
            if (!avlNode.IsEmpty)
            {
                foreach (KeyValue<TKey, TValue> left in InOrder(avlNode.Left!))
                {
                    yield return new KeyValue<TKey, TValue>(left.Key, left.Value);
                }

                yield return new KeyValue<TKey, TValue>(avlNode.KeyValue.Key, avlNode.KeyValue.Value);

                for (int i = 0; i < avlNode.Duplicates.Items.Length; i++)
                {
                    yield return avlNode.Duplicates.Items[i];
                }

                foreach (KeyValue<TKey, TValue> right in InOrder(avlNode.Right!))
                {
                    yield return new KeyValue<TKey, TValue>(right.Key, right.Value);
                }
            }
        }

        private static ImmutableAvlNode<TKey, TValue> RotateLeft(ImmutableAvlNode<TKey, TValue> left)
        {
            return new ImmutableAvlNode<TKey, TValue>(
                left.Right!.KeyValue,
                new ImmutableAvlNode<TKey, TValue>(left.KeyValue, left.Right.Left!, left.Left!),
                left.Right.Right!);
        }

        private static ImmutableAvlNode<TKey, TValue> RotateRight(ImmutableAvlNode<TKey, TValue> right)
        {
            return new ImmutableAvlNode<TKey, TValue>(
                right.Left!.KeyValue,
                right.Left.Left!,
                new ImmutableAvlNode<TKey, TValue>(right.KeyValue, right.Left.Right!, right.Right!));
        }

        private bool IsLeftHeavy()
        {
            return Left!.Height > Right!.Height;
        }

        private bool IsRightHeavy()
        {
            return Right!.Height > Left!.Height;
        }
    }
}
