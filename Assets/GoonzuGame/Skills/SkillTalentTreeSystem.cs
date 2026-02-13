using System;
using System.Collections.Generic;
using GoonzuGame.Classes;

namespace GoonzuGame.Skills
{
    public static class SkillTalentTreeSystem
    {
        // Example: Generate trees for all classes, subclasses, types, subtypes
        public static List<SkillTalentTree> GenerateAllTrees(
            List<ClassDef> classes,
            List<SubClassDef> subClasses,
            List<SubTypeDef> subTypes,
            List<string> types)
        {
            var trees = new List<SkillTalentTree>();
            foreach (var classDef in classes)
            {
                foreach (var subClassName in classDef.SubClasses)
                {
                    var subClass = subClasses.Find(s => s.Name == subClassName);
                    if (subClass == null) continue;
                    foreach (var type in types)
                    {
                        foreach (var subTypeName in subClass.SubTypes)
                        {
                            var subType = subTypes.Find(st => st.Name == subTypeName);
                            if (subType == null) continue;
                            var tree = SkillTalentTreeGenerator.GenerateTree(
                                classDef.Name,
                                subClass.Name,
                                type,
                                subType.Name
                            );
                            trees.Add(tree);
                        }
                    }
                }
            }
            return trees;
        }
    }
}
