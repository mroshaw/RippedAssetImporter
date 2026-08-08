#if ODIN_INSPECTOR && !DEBUG_NO_ODIN_INSPECTOR
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using static DaftAppleGames.Editor.RippedAssetImporter.UserInterfaceText;

namespace DaftAppleGames.Editor.RippedAssetImporter
{
    public partial class RippedAssetImporterWindow
    {
        [Title(MainTitle, MainSubtitle)]
        [TitleGroup(ReferencePathsGroup, ReferencePathsDescription)]
        [InfoBox(Introduction)]
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
        [MultiLineProperty(12)]
        [ReadOnly]
        [HideLabel]
        [PropertyTooltip(ImportReportTooltip)]
        [ShowInInspector]
        [PropertyOrder(40)]
        private string OdinReport => report;

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
