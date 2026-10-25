using System;
using System.Collections.Generic;

namespace GoonzuGame.Skills
{
    // Represents a node in the skill/talent tree
    public class SkillTreeNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int CurrentLevel { get; set; }
        public List<string> Prerequisites { get; set; } // List of node Ids
        public List<SkillTreeNode> Children { get; set; }
        public string Class { get; set; } // Class this node belongs to
        public string SubClass { get; set; } // Subclass this node belongs to
        public string Type { get; set; } // Type (active/passive/support/etc)
        public string SubType { get; set; } // Subtype (element, weapon, etc)

        public SkillTreeNode(string id, string name, string description, int minLevel, int maxLevel)
        {
            Id = id;
            Name = name;
            Description = description;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            CurrentLevel = minLevel;
            Prerequisites = new List<string>();
            Children = new List<SkillTreeNode>();
        }
    }

    // Represents the full skill/talent tree for a class/subclass
    public class SkillTalentTree
    {
        public string Class { get; set; }
        public string SubClass { get; set; }
        public List<SkillTreeNode> Nodes { get; set; }
        public SkillTalentTree(string className, string subClassName)
        {
            Class = className;
            SubClass = subClassName;
            Nodes = new List<SkillTreeNode>();
        }
    }
}
