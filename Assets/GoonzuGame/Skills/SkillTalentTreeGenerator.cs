using System;
using System.Collections.Generic;

namespace GoonzuGame.Skills
{
    public static class SkillTalentTreeGenerator
    {
        // Example: Generate a skill/talent tree for a class/subclass/type/subtype, levels 1-150
        public static SkillTalentTree GenerateTree(string className, string subClassName, string type, string subType)
        {
            var tree = new SkillTalentTree(className, subClassName);
            // For demo: 10 major nodes, each with 15 levels (1-150)
            for (int i = 1; i <= 10; i++)
            {
                var node = new SkillTreeNode(
                    id: $"{className}_{subClassName}_{type}_{subType}_Node{i}",
                    name: $"{type} Talent {i}",
                    description: $"{type} Talent Node {i} for {className}/{subClassName}/{subType}",
                    minLevel: 1,
                    maxLevel: 15
                )
                {
                    Class = className,
                    SubClass = subClassName,
                    Type = type,
                    SubType = subType
                };
                // Add prerequisites for all except the first node
                if (i > 1)
                    node.Prerequisites.Add($"{className}_{subClassName}_{type}_{subType}_Node{i-1}");
                tree.Nodes.Add(node);
            }
            return tree;
        }
    }
}
