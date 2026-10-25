using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace AIAssetTools
{
    /// <summary>
    /// Main asset pipeline for AI-generated GoonZu assets
    /// </summary>
    public class AssetPipeline : EditorWindow
    {
        [MenuItem("Tools/AI Asset Tools/Asset Pipeline")]
        static void ShowWindow()
        {
            GetWindow<AssetPipeline>("AI Asset Pipeline");
        }

        private enum PipelineStep
        {
            Setup,
            Generate,
            Import,
            Optimize,
            Complete
        }

        private PipelineStep currentStep = PipelineStep.Setup;
        private bool autoAdvance = true;
        private Vector2 scrollPos;

        // Pipeline settings
        private bool createFolders = true;
        private bool generatePlaceholders = true;
        private bool batchGenerate = false;
        private bool batchImport = true;
        private bool createAtlases = false;
        private bool validateAssets = true;

        void OnGUI()
        {
            GUILayout.Label("AI Asset Pipeline", EditorStyles.boldLabel);
            GUILayout.Label("Automated workflow for GoonZu asset creation");

            // Current step indicator
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Current Step:", currentStep.ToString());
            DrawProgressBar();

            // Auto advance toggle
            autoAdvance = EditorGUILayout.Toggle("Auto Advance Steps", autoAdvance);

            EditorGUILayout.Space();
            GUILayout.Label("Pipeline Configuration:", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Step toggles
            createFolders = EditorGUILayout.Toggle("1. Create Asset Folders", createFolders);
            generatePlaceholders = EditorGUILayout.Toggle("2. Generate Placeholders", generatePlaceholders);
            batchGenerate = EditorGUILayout.Toggle("3. Batch Generate Assets", batchGenerate);
            batchImport = EditorGUILayout.Toggle("4. Batch Import Assets", batchImport);
            createAtlases = EditorGUILayout.Toggle("5. Create Sprite Atlases", createAtlases);
            validateAssets = EditorGUILayout.Toggle("6. Validate Assets", validateAssets);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            // Control buttons
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Run Full Pipeline"))
            {
                RunFullPipeline();
            }

            if (GUILayout.Button("Run Current Step"))
            {
                RunCurrentStep();
            }

            if (GUILayout.Button("Next Step"))
            {
                NextStep();
            }

            if (GUILayout.Button("Reset Pipeline"))
            {
                ResetPipeline();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Quick actions
            GUILayout.Label("Quick Actions:", EditorStyles.boldLabel);

            if (GUILayout.Button("Setup Project"))
            {
                SetupProject();
            }

            if (GUILayout.Button("Generate Core Assets"))
            {
                GenerateCoreAssets();
            }

            if (GUILayout.Button("Import & Optimize"))
            {
                ImportAndOptimize();
            }
        }

        void DrawProgressBar()
        {
            float progress = (float)currentStep / (float)PipelineStep.Complete;
            Rect rect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(rect, progress, $"Step {currentStep.ToString()}");
        }

        void RunFullPipeline()
        {
            Debug.Log("Starting full asset pipeline...");

            if (createFolders) CreateAssetFolders();
            if (generatePlaceholders) GeneratePlaceholders();
            if (batchGenerate) BatchGenerate();
            if (batchImport) BatchImport();
            if (createAtlases) CreateAtlases();
            if (validateAssets) ValidateAssets();

            currentStep = PipelineStep.Complete;
            Debug.Log("Asset pipeline complete!");
        }

        void RunCurrentStep()
        {
            switch (currentStep)
            {
                case PipelineStep.Setup:
                    if (createFolders) CreateAssetFolders();
                    if (generatePlaceholders) GeneratePlaceholders();
                    break;
                case PipelineStep.Generate:
                    if (batchGenerate) BatchGenerate();
                    break;
                case PipelineStep.Import:
                    if (batchImport) BatchImport();
                    break;
                case PipelineStep.Optimize:
                    if (createAtlases) CreateAtlases();
                    if (validateAssets) ValidateAssets();
                    break;
            }

            if (autoAdvance) NextStep();
        }

        void NextStep()
        {
            if (currentStep < PipelineStep.Complete)
            {
                currentStep++;
            }
        }

        void ResetPipeline()
        {
            currentStep = PipelineStep.Setup;
            Debug.Log("Pipeline reset to setup");
        }

        void SetupProject()
        {
            CreateAssetFolders();
            GeneratePlaceholders();
            currentStep = PipelineStep.Generate;
            Debug.Log("Project setup complete");
        }

        void GenerateCoreAssets()
        {
            // Generate essential assets for a minimal working set
            RecraftAIManager recraftManager = FindObjectOfType<RecraftAIManager>();
            if (recraftManager == null)
            {
                GameObject managerObj = new GameObject("RecraftAIManager");
                recraftManager = managerObj.AddComponent<RecraftAIManager>();
            }

            // Generate core character and UI assets
            recraftManager.GenerateAllCharacters();
            recraftManager.GenerateIconSet("ui_basic");

            currentStep = PipelineStep.Import;
            Debug.Log("Core asset generation started");
        }

        void ImportAndOptimize()
        {
            BatchImport();
            CreateAtlases();
            ValidateAssets();
            currentStep = PipelineStep.Complete;
            Debug.Log("Import and optimization complete");
        }

        // Pipeline step implementations
        void CreateAssetFolders()
        {
            AssetOrganizer.CreateAssetFolders();
        }

        void GeneratePlaceholders()
        {
            // Use the placeholder generator
            AssetPlaceholderGenerator.GenerateAllPlaceholders();
        }

        void BatchGenerate()
        {
            Debug.Log("Batch generation would happen here");
            // This would integrate with AI APIs
        }

        void BatchImport()
        {
            // Import all PNGs in the GoonzuGame folder
            string[] pngFiles = Directory.GetFiles("Assets/GoonzuGame", "*.png", SearchOption.AllDirectories);

            foreach (string pngFile in pngFiles)
            {
                AssetDatabase.ImportAsset(pngFile, ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.Refresh();
            Debug.Log($"Imported {pngFiles.Length} assets");
        }

        void CreateAtlases()
        {
            // Create sprite atlases for performance
            Debug.Log("Creating sprite atlases...");
            // Implementation would go here
        }

        void ValidateAssets()
        {
            string[] pngFiles = Directory.GetFiles("Assets/GoonzuGame", "*.png", SearchOption.AllDirectories);
            int validCount = 0;

            foreach (string pngFile in pngFiles)
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(pngFile);
                if (texture != null)
                    validCount++;
            }

            Debug.Log($"Validated {validCount}/{pngFiles.Length} assets");
        }
    }
}