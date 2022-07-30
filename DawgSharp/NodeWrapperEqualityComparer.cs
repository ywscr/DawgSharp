﻿namespace DawgSharp;

class NodeWrapperEqualityComparer <TPayload> : IEqualityComparer <NodeWrapper <TPayload>>
{
    private readonly IEqualityComparer<TPayload> payloadComparer;

    public NodeWrapperEqualityComparer(IEqualityComparer<TPayload> payloadComparer)
    {
        this.payloadComparer = payloadComparer;
    }
        
    public bool Equals (NodeWrapper <TPayload> x, NodeWrapper <TPayload> y)
    {
        // ReSharper disable PossibleNullReferenceException
        bool equals = AreEqual(x.Node, y.Node);
        // ReSharper restore PossibleNullReferenceException

        return equals;
    }

    private bool AreEqual(Node<TPayload> xNode, Node<TPayload> yNode)
    {
        bool equals = payloadComparer.Equals(xNode.Payload, yNode.Payload)
                      && SequenceEqual(xNode.SortedChildren, yNode.SortedChildren);
        return equals;
    }

    private bool SequenceEqual(
        IEnumerable<KeyValuePair<char, Node<TPayload>>> x, 
        IEnumerable<KeyValuePair<char, Node<TPayload>>> y)
    {
        // Do not bother disposing of these enumerators.

        // ReSharper disable GenericEnumeratorNotDisposed
        var xe = x.GetEnumerator();
        var ye = y.GetEnumerator();
        // ReSharper restore GenericEnumeratorNotDisposed

        while (xe.MoveNext())
        {
            if (!ye.MoveNext()) return false;

            var xcurrent = xe.Current;
            var ycurrent = ye.Current;

            if (xcurrent.Key != ycurrent.Key) return false;
            if (!AreEqual(xcurrent.Value, ycurrent.Value)) return false;
        }

        return !ye.MoveNext();
    }

    private int ComputeHashCode(Node<TPayload> node)
    {
        int hashCode = payloadComparer.GetHashCode(node.Payload);

        foreach (var c in node.Children)
        {
            hashCode ^= c.Key ^ c.Value.GetHashCode();
        }

        return hashCode;
    }

    public int GetHashCode (NodeWrapper <TPayload> wrapper)
    {
        return wrapper.HashCode ??= ComputeHashCode(wrapper.Node);
    }
}