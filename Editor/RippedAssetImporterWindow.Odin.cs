#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.Editor.RippedAssetImporter
{
    public partial class RippedAssetImporterWindow
    {
        private const string ReferencePathsGroup = "Reference Paths";
        private const string AssetImportGroup = "Asset Import";
        private const string ImportOptionsGroup = "Import Options";
        private const string ImportReportGroup = "Import Report";

        [TitleGroup(ReferencePathsGroup, "Paths normally configured once for the current game project.")]
        [InfoBox("Copies the selected asset and its recursive dependencies while preserving AssetRipper GUIDs. " +
                 "Exported scripts are remapped to MonoScript types in the imported game assemblies.")]
        [FolderPath(AbsolutePath = true, RequireExistingPath = true)]
        [LabelText("Export Assets Root")]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(0)]
        private string OdinExportAssetsPath
        {
            get => exportAssetsPath;
            set => exportAssetsPath = value;
        }

        [TitleGroup(ReferencePathsGroup)]
        [FolderPath(RequireExistingPath = true)]
        [LabelText("Game Assemblies Root")]
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
        [LabelText("Dependencies Destination")]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(2)]
        private string OdinDestinationPath
        {
            get => destinationPath;
            set => destinationPath = value;
        }

        [TitleGroup(AssetImportGroup, "Select the asset to import and where its root copy should be placed.")]
        [FilePath(AbsolutePath = true, RequireExistingPath = true)]
        [LabelText("Source Asset")]
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
        [LabelText("Asset Import Destination")]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(11)]
        private string OdinAssetImportDestinationPath
        {
            get => assetImportDestinationPath;
            set => assetImportDestinationPath = value;
        }

        [TitleGroup(ImportOptionsGroup)]
        [LabelText("Force AssetRipper Re-index")]
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
        [LabelText("Fix Shader E Notation")]
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
        [LabelText("Repair Missing TMP Atlases")]
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
        [LabelText("Report Only")]
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
        [LabelText("Overwrite Existing")]
        [ToggleLeft]
        [DisableIf(nameof(IsImporting))]
        [ShowInInspector]
        [PropertyOrder(24)]
        private bool OdinOverwriteExisting
        {
            get => overwriteExisting;
            set => overwriteExisting = value;
        }

        [Button("Import Asset and Dependencies", ButtonSizes.Large)]
        [EnableIf(nameof(CanBeginImport))]
        [HideIf(nameof(IsImporting))]
        [PropertyOrder(30)]
        private void DrawImportButton()
        {
            BeginImport();
        }

        [Button("Cancel Import", ButtonSizes.Large)]
        [ShowIf(nameof(IsImporting))]
        [PropertyOrder(31)]
        private void DrawCancelButton()
        {
            CancelImport();
        }

        [ShowIf(nameof(IsImporting))]
        [ProgressBar(0.0, 1.0)]
        [LabelText("Import Progress")]
        [ShowInInspector]
        [PropertyOrder(32)]
        private float OdinImportProgress => importProgress;

        [ShowIf(nameof(IsImporting))]
        [DisplayAsString]
        [LabelText("Status")]
        [ShowInInspector]
        [PropertyOrder(33)]
        private string OdinImportStatus => importStatus;

        [TitleGroup(ImportReportGroup)]
        [MultiLineProperty(12)]
        [ReadOnly]
        [HideLabel]
        [ShowInInspector]
        [PropertyOrder(40)]
        private string OdinReport => report;

        private bool CanBeginImport => !isImporting && !string.IsNullOrWhiteSpace(sourceAssetPath);
    }
}
#endif
