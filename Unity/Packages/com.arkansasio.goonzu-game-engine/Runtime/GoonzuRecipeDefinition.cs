using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

[Serializable]
public struct GoonzuRecipeIngredient
{
    public GoonzuItemDefinition item;
    [Min(1)] public int quantity;
}

[CreateAssetMenu(menuName = "Goonzu/C 1.5 Recipe Definition", fileName = "GoonzuRecipe")]
public sealed class GoonzuRecipeDefinition : ScriptableObject
{
    [SerializeField] private string recipeId = string.Empty;
    [SerializeField] private string displayName = string.Empty;
    [SerializeField] private GoonzuRecipeIngredient[] ingredients = Array.Empty<GoonzuRecipeIngredient>();
    [SerializeField] private GoonzuItemDefinition result;
    [SerializeField, Min(1)] private int resultQuantity = 1;
    [SerializeField, Min(0)] private int requiredLevel;
    [SerializeField, Min(0f)] private float craftDurationSeconds;

    public string RecipeId => recipeId;
    public string DisplayName => displayName;
    public IReadOnlyList<GoonzuRecipeIngredient> Ingredients => ingredients;
    public GoonzuItemDefinition Result => result;
    public int ResultQuantity => Mathf.Max(1, resultQuantity);
    public int RequiredLevel => Mathf.Max(0, requiredLevel);
    public float CraftDurationSeconds => Mathf.Max(0f, craftDurationSeconds);

    public bool IsValid(out string error)
    {
        if (string.IsNullOrWhiteSpace(recipeId)) { error = "Recipe ID is required."; return false; }
        if (string.IsNullOrWhiteSpace(displayName)) { error = $"Recipe '{recipeId}' is missing a display name."; return false; }
        if (result == null || !result.IsValid(out _)) { error = $"Recipe '{recipeId}' has an invalid result."; return false; }
        foreach (var ingredient in ingredients)
        {
            if (ingredient.item == null || !ingredient.item.IsValid(out _) || ingredient.quantity <= 0)
            {
                error = $"Recipe '{recipeId}' has an invalid ingredient.";
                return false;
            }
        }
        error = string.Empty;
        return true;
    }
}
