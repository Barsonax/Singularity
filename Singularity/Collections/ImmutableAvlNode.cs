using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Singularity.Collections
{
    internal sealed class ImmutableAvlNode<TKey, TValue>
        where TKey : class
    {
        public static readonly ImmutableAvlNode<TKey, TValue> Empty = new ImmutableAvlNode<TKey, TValue>();
        public readonly int Height;
        public readonly bool IsEmpty;
        public readonly KeyValue<TKey, TValue> KeyValue;
        public readonly ImmutableList<KeyValue<TKey, TValue>> Duplicates;
        public readonly ImmutableAvlNode<TKey, TValue>? Left;
        public readonly ImmutableAvlNode<TKey, TValue>? Right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableAvlNode<TKey, TValue> Add(in KeyValue<TKey, TValue> keyValue)
        {
            if (IsEmpty)
            {
                return new ImmutableAvlNode<TKey, TValue>(in keyValue, ImmutableList<KeyValue<TKey, TValue>>.Utils.Empty, this, this);
            }

            if (keyValue.HashCode > KeyValue.HashCode)
            {
                return AddToRightBranch(this, in keyValue);
            }
            else if (keyValue.HashCode < KeyValue.HashCode)
            {
                return AddToLeftBranch(this, in keyValue);
            }

            return new ImmutableAvlNode<TKey, TValue>(in keyValue, this);
        }

        private ImmutableAvlNode()
        {
            IsEmpty = true;
            Duplicates = ImmutableList<KeyValue<TKey, TValue>>.Utils.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableAvlNode(in KeyValue<TKey, TValue> keyValue, ImmutableAvlNode<TKey, TValue> avlNode)
        {
            Duplicates = avlNode.Duplicates.Add(in keyValue);
            KeyValue = avlNode.KeyValue;
            Height = avlNode.Height;
            Left = avlNode.Left;
            Right = avlNode.Right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableAvlNode(in KeyValue<TKey, TValue> keyValue, in ImmutableList<KeyValue<TKey, TValue>> duplicates, ImmutableAvlNode<TKey, TValue> left, ImmutableAvlNode<TKey, TValue> right)
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
                Duplicates = right.Duplicates;
                Left = new ImmutableAvlNode<TKey, TValue>(in keyValue, in duplicates, left, right.Left!);
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
                Duplicates = left.Duplicates;
                Right = new ImmutableAvlNode<TKey, TValue>(in keyValue, in duplicates, left.Right!, right);
                Left = left.Left;
            }
            else
            {
                KeyValue = keyValue;
                Duplicates = duplicates;
                Left = left;
                Right = right;
            }

            Height = 1 + Math.Max(Left!.Height, Right!.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableAvlNode<TKey, TValue> AddToLeftBranch(ImmutableAvlNode<TKey, TValue> tree, in KeyValue<TKey, TValue> keyValue)
        {
            return new ImmutableAvlNode<TKey, TValue>(in tree.KeyValue, in tree.Duplicates, tree.Left!.Add(in keyValue), tree.Right!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableAvlNode<TKey, TValue> AddToRightBranch(ImmutableAvlNode<TKey, TValue> tree, in KeyValue<TKey, TValue> keyValue)
        {
            return new ImmutableAvlNode<TKey, TValue>(in tree.KeyValue, in tree.Duplicates, tree.Left!, tree.Right!.Add(in keyValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableAvlNode<TKey, TValue> RotateLeft(ImmutableAvlNode<TKey, TValue> left)
        {
            return new ImmutableAvlNode<TKey, TValue>(
                in left.Right!.KeyValue, in left.Right!.Duplicates,
                new ImmutableAvlNode<TKey, TValue>(in left.KeyValue, in left.Duplicates, left.Right.Left!, left.Left!),
                left.Right.Right!);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ImmutableAvlNode<TKey, TValue> RotateRight(ImmutableAvlNode<TKey, TValue> right)
        {
            return new ImmutableAvlNode<TKey, TValue>(
                in right.Left!.KeyValue, in right.Left!.Duplicates,
                right.Left.Left!,
                new ImmutableAvlNode<TKey, TValue>(in right.KeyValue, in right.Duplicates, right.Left.Right!, right.Right!));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsLeftHeavy()
        {
            return Left!.Height > Right!.Height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsRightHeavy()
        {
            return Right!.Height > Left!.Height;
        }
    }
}
