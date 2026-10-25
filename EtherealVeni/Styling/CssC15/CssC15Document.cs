using System;
using System.Collections.Generic;

namespace EtherealVeni.Styling.CssC15;

public sealed record CssC15Declaration(string Property, string Value);

public sealed class CssC15Rule
{
    public string Selector { get; }
    public List<CssC15Declaration> Declarations { get; } = new();
    public CssC15Rule(string selector) => Selector = selector.Trim();
}

public sealed class CssC15Document
{
    public int SchemaVersion { get; init; } = 1;
    public List<CssC15Rule> Rules { get; } = new();
}

public static class CssC15Parser
{
    public static CssC15Document Parse(string source)
    {
        if (string.IsNullOrWhiteSpace(source)) throw new ArgumentException("CSS.C1.5 source is empty.", nameof(source));
        var document = new CssC15Document();
        foreach (var block in source.Split('}', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = block.Split('{', 2);
            if (parts.Length != 2) throw new FormatException("CSS.C1.5 rule requires a selector and declaration block.");
            var rule = new CssC15Rule(parts[0]);
            foreach (var declaration in parts[1].Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var pair = declaration.Split(':', 2);
                if (pair.Length != 2) throw new FormatException("CSS.C1.5 declaration requires property:value.");
                rule.Declarations.Add(new CssC15Declaration(pair[0].Trim(), pair[1].Trim()));
            }
            document.Rules.Add(rule);
        }
        return document;
    }
}
