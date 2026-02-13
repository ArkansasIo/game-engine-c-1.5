# PoE 2-Style Skill & Talent Tree System

This system provides a scalable skill/talent tree for all classes, subclasses, types, and subtypes, supporting levels 1-150.

## Key Files
- `SkillTalentTree.cs`: Data structures for skill/talent nodes and trees.
- `SkillTalentTreeGenerator.cs`: Generates a tree for a given class/subclass/type/subtype.
- `SkillTalentTreeSystem.cs`: Generates trees for all classes, subclasses, types, and subtypes.

## Usage Example

```
csharp
using GoonzuGame.Classes;
using GoonzuGame.Skills;

// Prepare your class, subclass, type, and subtype lists
List<ClassDef> classes = ...;
List<SubClassDef> subClasses = ...;
List<SubTypeDef> subTypes = ...;
List<string> types = new List<string> { "Active", "Passive", "Support" };

// Generate all trees
var allTrees = SkillTalentTreeSystem.GenerateAllTrees(classes, subClasses, subTypes, types);

// Each tree covers 1-150 levels for its branch
```

## Extending
- Add more node logic, effects, or requirements in `SkillTreeNode`.
- Add UI/visualization by traversing the `SkillTalentTree` structure.
- Integrate with player progression and unlock logic as needed.

---

For further customization, see the code comments in each file.
