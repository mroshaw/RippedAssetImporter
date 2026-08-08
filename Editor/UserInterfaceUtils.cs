#if !ODIN_INSPECTOR || DEBUG_NO_ODIN_INSPECTOR
using UnityEditor;
using UnityEngine;
using static DaftAppleGames.Editor.RippedAssetImporter.UserInterfaceText;

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
                DrawSectionHeading(ReferencePathsGroup);
                string exportAssetsPath = window.ExportAssetsPath;
                DrawPathField(ExportAssetsPathLabel, ExportAssetsPathTooltip, ref exportAssetsPath, true);
                window.ExportAssetsPath = exportAssetsPath;

                string gameAssemblyPath = window.GameAssemblyPath;
                DrawPathField(GameAssemblyPathLabel, GameAssemblyPathTooltip, ref gameAssemblyPath, false);
                window.GameAssemblyPath = gameAssemblyPath;

                string destinationPath = window.DestinationPath;
                DrawPathField(
                    DependenciesDestinationLabel, DependenciesDestinationTooltip, ref destinationPath, false);
                window.DestinationPath = destinationPath;

                DrawSectionHeading(AssetImportGroup);
                DrawSourceAssetField(window);

                string assetImportDestinationPath = window.AssetImportDestinationPath;
                DrawPathField(AssetImportDestinationLabel, AssetImportDestinationTooltip,
                    ref assetImportDestinationPath, false);
                window.AssetImportDestinationPath = assetImportDestinationPath;

                DrawSectionHeading(ImportOptionsGroup);
                window.ForceAssetRipperReindex = EditorGUILayout.Toggle(
                    new GUIContent(ForceReindexLabel, ForceReindexTooltip),
                    window.ForceAssetRipperReindex);
                window.FixShaderENotation = EditorGUILayout.Toggle(
                    new GUIContent(FixShaderExponentLabel, FixShaderExponentTooltip), window.FixShaderENotation);
                window.RepairMissingTmpAtlases = EditorGUILayout.Toggle(
                    new GUIContent(RepairTmpAtlasesLabel, RepairTmpAtlasesTooltip),
                    window.RepairMissingTmpAtlases);
                window.ReportOnly = EditorGUILayout.Toggle(
                    new GUIContent(ReportOnlyLabel, ReportOnlyTooltip), window.ReportOnly);
                window.OverwriteExisting = EditorGUILayout.Toggle(
                    new GUIContent(OverwriteExistingLabel, OverwriteExistingTooltip), window.OverwriteExisting);
            }

            DrawImportControls(window);
            DrawReport(window);
        }

        private static void DrawIntroduction()
        {
            EditorGUILayout.LabelField(MainTitle, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(Introduction, MessageType.Info);
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
                GUI.Label(progressRect, new GUIContent(
                    string.Empty, ImportProgressTooltip + " " + ImportStatusTooltip));
                if (GUILayout.Button(new GUIContent(CancelButtonLabel, CancelButtonTooltip), GUILayout.Height(28.0f)))
                {
                    window.CancelImport();
                }
                return;
            }

            using (new EditorGUI.DisabledScope(!window.CanBeginImport))
            {
                if (GUILayout.Button(
                        new GUIContent(ImportButtonLabel, ImportButtonTooltip),
                        GUILayout.Height(32.0f)))
                {
                    window.BeginImport();
                }
            }
        }

        private static void DrawReport(RippedAssetImporterWindow window)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                new GUIContent(ImportReportGroup, ImportReportTooltip), EditorStyles.boldLabel);
            window.ReportScrollPosition = EditorGUILayout.BeginScrollView(window.ReportScrollPosition);
            EditorGUILayout.TextArea(window.Report, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private static void DrawSourceAssetField(RippedAssetImporterWindow window)
        {
            EditorGUILayout.BeginHorizontal();
            window.SourceAssetPath = EditorGUILayout.TextField(
                new GUIContent(SourceAssetPathLabel, SourceAssetPathTooltip), window.SourceAssetPath);
            if (GUILayout.Button(
                    new GUIContent(BrowseButtonLabel, SourceAssetPathTooltip), GUILayout.Width(BrowseButtonWidth)))
            {
                string initialPath = GetFilePickerDirectory(
                    window.SourceAssetPath, window.ExportAssetsPath);
                string selectedPath = EditorUtility.OpenFilePanel(
                    SourceAssetDialogTitle, initialPath, string.Empty);
                if (!string.IsNullOrEmpty(selectedPath)) window.SourceAssetPath = selectedPath;
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawPathField(string label, string tooltip, ref string path, bool allowAbsolutePath)
        {
            EditorGUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(new GUIContent(label, tooltip), path);
            if (GUILayout.Button(new GUIContent(BrowseButtonLabel, tooltip), GUILayout.Width(BrowseButtonWidth)))
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
