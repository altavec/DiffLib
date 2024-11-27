// <copyright file="AlignmentNode.cs" company="Altavec">
// Copyright (c) Altavec. All rights reserved.
// </copyright>

namespace DiffLib;

internal class AlignmentNode
{
    public AlignmentNode(DiffOperation operation, double similarity, int nodeCount, AlignmentNode? next)
    {
        this.Operation = operation;
        this.Similarity = similarity;
        this.NodeCount = nodeCount;
        this.Next = next;
    }

    public DiffOperation Operation { get; }

    public double Similarity { get; }

    public double AverageSimilarity => this.NodeCount == 0 ? 0.0 : this.Similarity / this.NodeCount;

    public int NodeCount { get; }

    public AlignmentNode? Next { get; }
}