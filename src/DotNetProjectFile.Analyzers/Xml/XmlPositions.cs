﻿using Microsoft.CodeAnalysis.Text;
using System.Xml;

namespace DotNetProjectFile.Xml;

public sealed record XmlPositions
{
    public LinePositionSpan FullSpan => new(StartElement.Start, EndElement.End);

    public LinePositionSpan StartElement { get; init; }

    public LinePositionSpan EndElement { get; init; }

    public bool IsSelfClosing => StartElement == EndElement;

    public static XmlPositions New(XElement element)
    {
        LinePosition? start = null;
        LinePosition? next = null;
        LinePosition? end = null;

        using var reader = element.CreateReader();

        var info = (IXmlLineInfo)reader;

        reader.MoveToContent();

        var depth = reader.Depth;

        do
        {
            if (reader.NodeType == XmlNodeType.Element && depth == reader.Depth)
            {
                start ??= info.LinePosition().Expand(-1);
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
                end ??= info.LinePosition().Expand(-2);
                next ??= end.Value.Expand(-1);
            }
            else if (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.None)
            {
                next ??= info.LinePosition().Expand(-1);
            }
        }
        while (reader.Read());

        var final = element.NextNode?.LinePosition()
            ?? info.LinePosition().Expand(element.Name.LocalName.Length + 1);

        return new()
        {
            StartElement = new(start.GetValueOrDefault(), next ?? final),
            EndElement = new(end.GetValueOrDefault(), final),
        };
    }
}
