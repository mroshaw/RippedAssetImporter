namespace DaftAppleGames.Editor.RippedAssetImporter
{
    internal static class UserInterfaceText
    {
        internal const string MenuPath = "Tools/Import Ripped Asset";
        internal const string WindowTitle = "Ripped Asset Importer";
        internal const string MainTitle = "Ripped Asset Importer";
        internal const string MainSubtitle = "Import an asset and its recursive dependency closure";
        internal const string Introduction =
            "Copies the selected asset and its recursive dependencies while preserving AssetRipper GUIDs. " +
            "Exported scripts are remapped to MonoScript types in the imported game assemblies.";
        internal const string InitialReport = "Select an AssetRipper asset to begin.";

        internal const string ReferencePathsGroup = "Reference Paths";
        internal const string ReferencePathsDescription =
            "Paths normally configured once for the current game project.";
        internal const string AssetImportGroup = "Asset Import";
        internal const string AssetImportDescription =
            "Select the asset to import and where its root copy should be placed.";
        internal const string ImportOptionsGroup = "Import Options";
        internal const string ImportReportGroup = "Import Report";

        internal const string ExportAssetsPathLabel = "Ripped Assets Root";
        internal const string GameAssemblyPathLabel = "Game Assemblies Root";
        internal const string DependenciesDestinationLabel = "Dependencies Destination";
        internal const string SourceAssetPathLabel = "Source Asset";
        internal const string AssetImportDestinationLabel = "Asset Import Destination";
        internal const string ForceReindexLabel = "Force AssetRipper Re-index";
        internal const string FixShaderExponentLabel = "Fix Shader E Notation";
        internal const string RepairTmpAtlasesLabel = "Repair Missing TMP Atlases";
        internal const string ReportOnlyLabel = "Report Only";
        internal const string OverwriteExistingLabel = "Overwrite Existing";
        internal const string ImportButtonLabel = "Import Asset and Dependencies";
        internal const string CancelButtonLabel = "Cancel Import";
        internal const string ImportProgressLabel = "Import Progress";
        internal const string ImportStatusLabel = "Status";
        internal const string BrowseButtonLabel = "Browse";
        internal const string SourceAssetDialogTitle = "Select AssetRipper asset";
        internal const string FolderPathDoesNotExistMessage = "Please select a valid folder.";
        internal const string AssetPathDoesNotExistMessage = "Please select a valid asset file.";
        
        internal const string ExportAssetsPathTooltip =
            "Root Assets folder of the Unity project exported by AssetRipper.";
        internal const string GameAssemblyPathTooltip =
            "Project folder containing the imported game DLLs used to resolve MonoScript references.";
        internal const string DependenciesDestinationTooltip =
            "Project folder where dependencies discovered beneath the AssetRipper export root are imported.";
        internal const string SourceAssetPathTooltip =
            "AssetRipper asset to import along with its recursive dependency closure.";
        internal const string AssetImportDestinationTooltip =
            "Project folder where the selected source asset itself is imported.";
        internal const string ForceReindexTooltip =
            "Rebuild the cached AssetRipper GUID index before discovering dependencies.";
        internal const string FixShaderExponentTooltip =
            "Convert scientific-notation values that Unity's shader importer may reject.";
        internal const string RepairTmpAtlasesTooltip =
            "Restore missing blank atlas sub-assets required by imported dynamic TextMeshPro fonts.";
        internal const string ReportOnlyTooltip =
            "Analyse and report the planned import without writing or modifying assets.";
        internal const string OverwriteExistingTooltip =
            "Replace files that already exist at their calculated import destinations.";
        internal const string ImportButtonTooltip =
            "Import the selected asset and all dependencies reachable through serialized GUID references.";
        internal const string CancelButtonTooltip =
            "Request cancellation of the current import; files already written are not removed.";
        internal const string ImportProgressTooltip = "Progress through the current import operation.";
        internal const string ImportStatusTooltip = "Current phase of the import operation.";
        internal const string ImportReportTooltip =
            "Detailed results, repairs, skipped assets, and unresolved references from the latest import.";
    }
}
