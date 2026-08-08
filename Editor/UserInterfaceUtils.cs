#if !ODIN_INSPECTOR
using UnityEditor;
using UnityEngine;

namespace DaftAppleGames.Editor.RippedAssetImporter
{
    internal static class UserInterfaceUtils
    {
        private const float LabelWidth = 215.0f;
        private const float BrowseButtonWidth = 72.0f;

        /// <summary>
        /// Draws the complete importer interface for the supplied editor window.
        /// </summary>
        public static void Draw(RippedAssetImporterWindow window)
        {
            EditorGUIUtility.labelWidth = LabelWidth;
            DrawIntroduction();

            using (new EditorGUI.DisabledScope(window.IsImporting))
            {
                DrawSectionHeading("Reference Paths");
                string exportAssetsPath = window.ExportAssetsPath;
                DrawPathField("Export Assets Root", ref exportAssetsPath, true);
                window.ExportAssetsPath = exportAssetsPath;

                string gameAssemblyPath = window.GameAssemblyPath;
                DrawPathField("Game Assemblies Root", ref gameAssemblyPath, false);
                window.GameAssemblyPath = gameAssemblyPath;

                string destinationPath = window.DestinationPath;
                DrawPathField("Dependencies Destination", ref destinationPath, false);
                window.DestinationPath = destinationPath;

                DrawSectionHeading("Asset Import");
                DrawSourceAssetField(window);

                string assetImportDestinationPath = window.AssetImportDestinationPath;
                DrawPathField("Asset Import Destination", ref assetImportDestinationPath, false);
                window.AssetImportDestinationPath = assetImportDestinationPath;

                DrawSectionHeading("Import Options");
                window.ForceAssetRipperReindex = EditorGUILayout.Toggle(
                    "Force AssetRipper Re-index", window.ForceAssetRipperReindex);
                window.FixShaderENotation = EditorGUILayout.Toggle(
                    "Fix shader E notation", window.FixShaderENotation);
                window.RepairMissingTmpAtlases = EditorGUILayout.Toggle(
                    "Repair missing TMP atlases", window.RepairMissingTmpAtlases);
                window.ReportOnly = EditorGUILayout.Toggle("Report only", window.ReportOnly);
                window.OverwriteExisting = EditorGUILayout.Toggle("Overwrite Existing", window.OverwriteExisting);
            }

            DrawImportControls(window);
            DrawReport(window);
        }

        private static void DrawIntroduction()
        {
            EditorGUILayout.LabelField("AssetRipper Reference Asset Importer", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Copies the selected asset and its recursive dependencies while preserving AssetRipper GUIDs. " +
                "Exported scripts are remapped to MonoScript types in ThunderKit's imported game DLLs.",
                MessageType.Info);
        }

        private static void DrawSectionHeading(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        private static void DrawImportControls(RippedAssetImporterWindow window)
        {
            EditorGUILayout.Space();
            if (window.IsImporting)
            {
                Rect progressRect = EditorGUILayout.GetControlRect(false, 22.0f);
                EditorGUI.ProgressBar(progressRect, window.ProgressValue, window.ImportStatus);
                if (GUILayout.Button("Cancel Import", GUILayout.Height(28.0f))) window.CancelImport();
                return;
            }

            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(window.SourceAssetPath)))
            {
                if (GUILayout.Button("Import Asset and Dependencies", GUILayout.Height(32.0f))) window.BeginImport();
            }
        }

        private static void DrawReport(RippedAssetImporterWindow window)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Import Report", EditorStyles.boldLabel);
            window.ReportScrollPosition = EditorGUILayout.BeginScrollView(window.ReportScrollPosition);
            EditorGUILayout.TextArea(window.Report, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private static void DrawSourceAssetField(RippedAssetImporterWindow window)
        {
            EditorGUILayout.BeginHorizontal();
            window.SourceAssetPath = EditorGUILayout.TextField("Source Asset", window.SourceAssetPath);
            if (GUILayout.Button("Browse", GUILayout.Width(BrowseButtonWidth)))
            {
                string initialPath = GetFilePickerDirectory(
                    window.SourceAssetPath, window.ExportAssetsPath);
                string selectedPath = EditorUtility.OpenFilePanel(
                    "Select AssetRipper asset", initialPath, string.Empty);
                if (!string.IsNullOrEmpty(selectedPath)) window.SourceAssetPath = selectedPath;
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawPathField(string label, ref string path, bool allowAbsolutePath)
        {
            EditorGUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(label, path);
            if (GUILayout.Button("Browse", GUILayout.Width(BrowseButtonWidth)))
            {
                string initialPath = FileSystemUtils.GetAbsolutePath(path);
                string selectedPath = EditorUtility.OpenFolderPanel(label, initialPath, string.Empty);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    string projectRelativePath = FileSystemUtils.GetProjectRelativePath(selectedPath);
                    path = allowAbsolutePath || string.IsNullOrEmpty(projectRelativePath)
                        ? selectedPath
                        : projectRelativePath;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private static string GetFilePickerDirectory(string filePath, string fallbackDirectoryPath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                string absoluteFilePath = FileSystemUtils.GetAbsolutePath(filePath);
                string directoryPath = System.IO.Path.GetDirectoryName(absoluteFilePath);
                if (!string.IsNullOrEmpty(directoryPath)) return directoryPath;
            }

            return FileSystemUtils.GetAbsolutePath(fallbackDirectoryPath);
        }
    }
}
#endif
