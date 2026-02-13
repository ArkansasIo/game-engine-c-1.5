namespace GameEngine
{
    /// <summary>
    /// Central manager for enabling/disabling and tracking game features.
    /// </summary>
    public class FeatureManager
    {
        public void EnableFeature(string featureName)
        {
            System.Console.WriteLine($"Feature enabled: {featureName}");
        }
        public void DisableFeature(string featureName)
        {
            System.Console.WriteLine($"Feature disabled: {featureName}");
        }
        public bool IsFeatureEnabled(string featureName)
        {
            System.Console.WriteLine($"Check if feature enabled: {featureName}");
            return true;
        }
    }
}
