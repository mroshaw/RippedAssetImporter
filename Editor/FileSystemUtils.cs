using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace DaftAppleGames.Editor.RippedAssetImporter
{
    /// <summary>
    /// Provides project path conversion and Windows extended-length file operations for
    /// reference imports. AssetRipper preserves deeply nested game paths that can exceed the
    /// limits enforced by Unity 2019's Mono filesystem APIs, even when the host operating system
    /// supports longer paths.
    /// </summary>
    internal static class FileSystemUtils
    {
        // Directory and file operations have different effective MAX_PATH limits because Windows must reserve
        // space for a filename when creating a directory. These are the legacy limits used by Unity's Mono runtime.
        private const int LegacyMaxDirectoryPath = 247;
        private const int LegacyMaxFilePath = 259;
        private const int ErrorAlreadyExists = 183;

        private static readonly bool IsWindowsEditor = Application.platform == RuntimePlatform.WindowsEditor;

        // Unity 2019's Directory.CreateDirectory can reject extended-length paths before Windows sees them.
        // Calling the Unicode Win32 API directly lets the \\?\ prefix opt out of legacy path parsing.
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateDirectoryW(string path, IntPtr securityAttributes);

        /// <summary>
        /// Reads the GUID from a Unity metadata file when that file exists.
        /// </summary>
        public static string GetMetaGuid(string metaPath)
        {
            return FileExists(metaPath) ? ReadMetaGuid(metaPath) : string.Empty;
        }

        /// <summary>
        /// Reads the GUID declaration from a Unity metadata file.
        /// </summary>
        public static string ReadMetaGuid(string metaPath)
        {
            using (StreamReader reader = new StreamReader(ToExtendedLengthPath(metaPath)))
            {
                for (int lineIndex = 0; lineIndex < 20 && !reader.EndOfStream; lineIndex++)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line) || !line.StartsWith("guid:", StringComparison.Ordinal)) continue;

                    string guid = line.Substring("guid:".Length).Trim();
                    if (guid.Length == 32) return guid;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Determines whether a path is a descendant of the supplied root directory.
        /// </summary>
        public static bool IsPathWithinRoot(string path, string root)
        {
            string rootWithSeparator = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
                                       Path.DirectorySeparatorChar;
            return path.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the path relative to the supplied root directory.
        /// </summary>
        public static string GetRelativePath(string root, string path)
        {
            Uri rootUri = new Uri(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
                                  Path.DirectorySeparatorChar);
            Uri pathUri = new Uri(path);
            return Uri.UnescapeDataString(rootUri.MakeRelativeUri(pathUri).ToString())
                .Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Resolves a project-relative or absolute path to a normalized absolute path. An empty
        /// path resolves to the Unity project root so file and folder pickers always have a safe
        /// initial location.
        /// </summary>
        public static string GetAbsolutePath(string path)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            if (string.IsNullOrWhiteSpace(path)) return NormalizeFullPath(projectRoot);
            if (Path.IsPathRooted(path)) return NormalizeFullPath(path);

            return NormalizeFullPath(Path.Combine(projectRoot, path));
        }

        /// <summary>
        /// Converts an absolute path inside the Unity project to a project-relative path.
        /// </summary>
        public static string GetProjectRelativePath(string absolutePath)
        {
            string projectRoot = NormalizeFullPath(Directory.GetParent(Application.dataPath).FullName);
            string normalizedPath = NormalizeFullPath(absolutePath);
            if (!IsPathWithinRoot(normalizedPath, projectRoot) &&
                !string.Equals(normalizedPath, projectRoot, StringComparison.OrdinalIgnoreCase)) return string.Empty;

            return GetRelativePath(projectRoot, normalizedPath).Replace('\\', '/');
        }

        /// <summary>
        /// Returns a canonical absolute path without trailing directory separators.
        /// </summary>
        public static string NormalizeFullPath(string path)
        {
            return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// Checks for a file while supporting extended-length Windows paths.
        /// </summary>
        public static bool FileExists(string path)
        {
            return File.Exists(ToExtendedLengthPath(path));
        }

        /// <summary>
        /// Reads a text file while supporting extended-length Windows paths.
        /// </summary>
        public static string ReadAllText(string path)
        {
            return File.ReadAllText(ToExtendedLengthPath(path));
        }

        /// <summary>
        /// Writes UTF-8 text without a byte-order mark and supports extended-length Windows paths.
        /// </summary>
        public static void WriteAllText(string path, string contents)
        {
            File.WriteAllText(ToExtendedLengthPath(path), contents, new UTF8Encoding(false));
        }

        /// <summary>
        /// Copies and overwrites a file while supporting extended-length Windows paths.
        /// </summary>
        public static void CopyFile(string sourcePath, string destinationPath)
        {
            File.Copy(ToExtendedLengthPath(sourcePath), ToExtendedLengthPath(destinationPath), true);
        }

        /// <summary>
        /// Creates a directory tree while supporting extended-length Windows paths.
        /// </summary>
        public static void CreateDirectory(string path)
        {
            if (!IsWindowsEditor || path.Length <= LegacyMaxDirectoryPath)
            {
                Directory.CreateDirectory(path);
                return;
            }

            CreateLongDirectory(path);
        }

        /// <summary>
        /// Adds the appropriate Windows extended-length prefix when a path exceeds legacy limits.
        /// </summary>
        public static string ToExtendedLengthPath(string path, int legacyMaxPath = LegacyMaxFilePath)
        {
            if (!IsWindowsEditor || !Path.IsPathRooted(path) || path.Length <= legacyMaxPath ||
                path.StartsWith(@"\\?\", StringComparison.Ordinal)) return path;

            // UNC paths use \\?\UNC\server\share rather than placing \\?\ directly before the leading slashes.
            return path.StartsWith(@"\\", StringComparison.Ordinal)
                ? @"\\?\UNC\" + path.Substring(2)
                : @"\\?\" + path;
        }

        private static void CreateLongDirectory(string path)
        {
            if (Directory.Exists(ToExtendedLengthPath(path, LegacyMaxDirectoryPath))) return;

            // CreateDirectoryW creates only the final segment, unlike Directory.CreateDirectory's recursive
            // behavior, so ensure the parent chain exists before invoking it.
            string parentPath = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(parentPath) &&
                !Directory.Exists(ToExtendedLengthPath(parentPath, LegacyMaxDirectoryPath)))
                CreateDirectory(parentPath);

            if (CreateDirectoryW(ToExtendedLengthPath(path, LegacyMaxDirectoryPath), IntPtr.Zero)) return;

            int errorCode = Marshal.GetLastWin32Error();
            // Treat creation as successful if Unity or another importer created the directory after our check.
            if (errorCode != ErrorAlreadyExists)
                throw new IOException($"Could not create directory '{path}'. Windows error: {errorCode}.");
        }
    }
}
