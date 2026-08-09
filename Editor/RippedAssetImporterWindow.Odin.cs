#if ODIN_INSPECTOR && !DEBUG_NO_ODIN_INSPECTOR
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using static DaftAppleGames.Editor.RippedAssetImporter.UserInterfaceText;

namespace DaftAppleGames.Editor.RippedAssetImporter
{
    public partial class RippedAssetImporterWindow
    {
        [OnInspectorGUI]
        [PropertyOrder(-100)]
        private void DrawOdinHeader()
        {
            SirenixEditorGUI.Title(MainTitle, MainSubtitle, TextAlignment.Left, true);
            SirenixEditorGUI.InfoMessageBox(Introduction);
        }

        [TitleGroup(ReferencePathsGroup, ReferencePathsDescription)]
        [FolderPath(AbsolutePath = true)]
        [ValidateInput(nameof(IsExistingFolderPath), FolderPathDoesNotExistMessage)]
        [LabelText(ExportAssetsPathLabel)]
        [PropertyTooltip(ExportAssetsPathTooltip)]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(0)]
        private string OdinExportAssetsPath
        {
            get => exportAssetsPath;
            set => exportAssetsPath = value;
        }

        [TitleGroup(ReferencePathsGroup)]
        [FolderPath]
        [ValidateInput(nameof(IsExistingFolderPath), FolderPathDoesNotExistMessage)]
        [LabelText(GameAssemblyPathLabel)]
        [PropertyTooltip(GameAssemblyPathTooltip)]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(1)]
        private string OdinGameAssemblyPath
        {
            get => gameAssemblyPath;
            set => gameAssemblyPath = value;
        }

        [TitleGroup(ReferencePathsGroup)]
        [FolderPath]
        [ValidateInput(nameof(IsExistingFolderPath), FolderPathDoesNotExistMessage)]
        [LabelText(DependenciesDestinationLabel)]
        [PropertyTooltip(DependenciesDestinationTooltip)]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(2)]
        private string OdinDestinationPath
        {
            get => destinationPath;
            set => destinationPath = value;
        }

        [TitleGroup(AssetImportGroup, AssetImportDescription)]
        [FilePath(ParentFolder = "$" + nameof(OdinSourceAssetPickerRoot), AbsolutePath = true,
            RequireExistingPath = false)]
        [ValidateInput(nameof(IsExistingFilePath), AssetPathDoesNotExistMessage)]
        [LabelText(SourceAssetPathLabel)]
        [PropertyTooltip(SourceAssetPathTooltip)]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(10)]
        private string OdinSourceAssetPath
        {
            get => sourceAssetPath;
            set => sourceAssetPath = value;
        }

        [TitleGroup(AssetImportGroup)]
        [FolderPath]
        [ValidateInput(nameof(IsExistingFolderPath), FolderPathDoesNotExistMessage)]
        [LabelText(AssetImportDestinationLabel)]
        [PropertyTooltip(AssetImportDestinationTooltip)]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(11)]
        private string OdinAssetImportDestinationPath
        {
            get => assetImportDestinationPath;
            set => assetImportDestinationPath = value;
        }

        [TitleGroup(ImportOptionsGroup)]
        [LabelText(ForceReindexLabel)]
        [PropertyTooltip(ForceReindexTooltip)]
        [ToggleLeft]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(20)]
        private bool OdinForceAssetRipperReindex
        {
            get => forceAssetRipperReindex;
            set => forceAssetRipperReindex = value;
        }

        [TitleGroup(ImportOptionsGroup)]
        [LabelText(FixShaderExponentLabel)]
        [PropertyTooltip(FixShaderExponentTooltip)]
        [ToggleLeft]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(21)]
        private bool OdinFixShaderExponentNotation
        {
            get => fixShaderExponentNotation;
            set => fixShaderExponentNotation = value;
        }

        [TitleGroup(ImportOptionsGroup)]
        [LabelText(RepairTmpAtlasesLabel)]
        [PropertyTooltip(RepairTmpAtlasesTooltip)]
        [ToggleLeft]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(22)]
        private bool OdinRepairMissingTmpAtlases
        {
            get => repairMissingTmpAtlases;
            set => repairMissingTmpAtlases = value;
        }

        [TitleGroup(ImportOptionsGroup)]
        [LabelText(ReportOnlyLabel)]
        [PropertyTooltip(ReportOnlyTooltip)]
        [ToggleLeft]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(23)]
        private bool OdinReportOnly
        {
            get => reportOnly;
            set => reportOnly = value;
        }

        [TitleGroup(ImportOptionsGroup)]
        [LabelText(OverwriteExistingLabel)]
        [PropertyTooltip(OverwriteExistingTooltip)]
        [ToggleLeft]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(24)]
        private bool OdinOverwriteExisting
        {
            get => overwriteExisting;
            set => overwriteExisting = value;
        }

        [Button(ImportButtonLabel, ButtonSizes.Large)]
        [PropertyTooltip(ImportButtonTooltip)]
        [EnableIf(nameof(AreAllPathPropertiesPopulated))]
        [HideIf(nameof(IsImporting))]
        [PropertyOrder(30)]
        private void DrawImportButton()
        {
            BeginImport();
        }

        [Button(CancelButtonLabel, ButtonSizes.Large)]
        [PropertyTooltip(CancelButtonTooltip)]
        [ShowIf(nameof(IsImporting))]
        [PropertyOrder(31)]
        private void DrawCancelButton()
        {
            CancelImport();
        }

        [ShowIf(nameof(IsImporting))]
        [ProgressBar(0.0, 1.0)]
        [LabelText(ImportProgressLabel)]
        [PropertyTooltip(ImportProgressTooltip)]
        [ShowInInspector]
        [PropertyOrder(32)]
        private float OdinImportProgress => importProgress;

        [ShowIf(nameof(IsImporting))]
        [DisplayAsString]
        [LabelText(ImportStatusLabel)]
        [PropertyTooltip(ImportStatusTooltip)]
        [ShowInInspector]
        [PropertyOrder(33)]
        private string OdinImportStatus => importStatus;

        [TitleGroup(ImportReportGroup)]
        [OnInspectorGUI]
        [PropertyOrder(40)]
        private void DrawOdinReport()
        {
            string reportText = report ?? string.Empty;
            float reportWidth = Mathf.Max(100.0f, EditorGUIUtility.currentViewWidth - 40.0f);
            float reportHeight = Mathf.Max(
                160.0f, EditorStyles.textArea.CalcHeight(new GUIContent(reportText), reportWidth));

            reportScrollPosition = EditorGUILayout.BeginScrollView(
                reportScrollPosition, GUILayout.MinHeight(160.0f), GUILayout.ExpandHeight(true));
            EditorGUILayout.SelectableLabel(
                reportText, EditorStyles.textArea, GUILayout.Height(reportHeight));
            Rect reportRect = GUILayoutUtility.GetLastRect();
            GUI.Label(reportRect, new GUIContent(string.Empty, ImportReportTooltip));
            EditorGUILayout.EndScrollView();
        }

        private string OdinSourceAssetPickerRoot => FileSystemUtils.GetAbsolutePath(exportAssetsPath);

        private bool IsExistingFolderPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) &&
                   Directory.Exists(FileSystemUtils.GetAbsolutePath(path));
        }

        private bool IsExistingFilePath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) &&
                   FileSystemUtils.FileExists(FileSystemUtils.GetAbsolutePath(path));
        }
    }
}
#endif
