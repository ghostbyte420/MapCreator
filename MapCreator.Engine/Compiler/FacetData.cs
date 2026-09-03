using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace MapCreator.Engine.Compiler
{
    /// <summary>
    /// Reads the map-compiler data files that are EMBEDDED inside this assembly
    /// (MapCreator.Engine.dll). Every embedded file lives under the logical
    /// prefix "FacetData\". A file placed under "FacetData\Custom\..." OVERRIDES
    /// the matching file of the same relative path that lives outside of Custom.
    ///
    /// Example:
    ///   Original : FacetData\TerrainTypes\Grass.xml
    ///   Override : FacetData\Custom\TerrainTypes\Grass.xml   (wins if present)
    /// </summary>
    public static class FacetData
    {
        private const string Root = "FacetData\\";
        private const string CustomRoot = "FacetData\\Custom\\";

        // The assembly that holds the embedded files (this Engine DLL).
        private static readonly Assembly Asm = typeof(FacetData).Assembly;

        // Every embedded resource name, read once and cached for speed.
        private static readonly string[] AllNames = Asm.GetManifestResourceNames();

        /// <summary>
        /// Turns a caller path into our internal form: backslashes, no leading slash.
        /// e.g. "TerrainTypes/Grass.xml" -> "TerrainTypes\Grass.xml".
        /// </summary>
        private static string Normalize(string relative)
        {
            if (string.IsNullOrEmpty(relative))
                return string.Empty;
            return relative.Replace('/', '\\').TrimStart('\\');
        }

        /// <summary>
        /// Picks the exact embedded name to use for a relative path.
        /// Priority order:
        /// 1. Exact Custom path match
        /// 2. Filename-only Custom match (any subfolder)
        /// 3. Original path match
        /// Returns null if no match exists.
        /// </summary>
        public static string? ResolveName(string relative)
        {
            string rel = Normalize(relative);
            string custom = CustomRoot + rel;
            string original = Root + rel;

            // Step 1: Try EXACT Custom path match first
            if (AllNames.Any(n => n.Equals(custom, StringComparison.OrdinalIgnoreCase)))
                return custom;

            // Step 2: Search Custom folder recursively by FILENAME ONLY
            // This allows flexible organization like Custom/MyProject/Common.xml
            string filename = Path.GetFileName(rel);
            if (!string.IsNullOrEmpty(filename))
            {
                var customMatch = AllNames
                    .Where(n => n.StartsWith(CustomRoot, StringComparison.OrdinalIgnoreCase))
                    .Where(n => Path.GetFileName(n).Equals(filename, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)  // Alphabetical priority
                    .FirstOrDefault();

                if (customMatch != null)
                    return customMatch;
            }

            // Step 3: ONLY NOW check original path
            if (AllNames.Any(n => n.Equals(original, StringComparison.OrdinalIgnoreCase)))
                return original;

            return null;
        }

        /// <summary>
        /// Opens a read stream for the given relative path (Custom overrides Original).
        /// </summary>
        public static Stream OpenRead(string relative)
        {
            string? name = ResolveName(relative);
            if (name == null)
                throw new FileNotFoundException("Embedded facet data not found: " + relative);

            Stream? s = Asm.GetManifestResourceStream(name);
            if (s == null)
                throw new FileNotFoundException("Embedded facet data stream missing: " + name);
            return s;
        }

        /// <summary>
        /// Loads the given relative path into an XmlDocument (Custom overrides Original).
        /// </summary>
        public static XmlDocument LoadXml(string relative)
        {
            XmlDocument doc = new XmlDocument();
            using (Stream s = OpenRead(relative))
            {
                doc.Load(s);
            }
            return doc;
        }

        /// <summary>
        /// Lists the relative paths of every *.xml file under the given logical folder.
        /// If recurse is true, subfolders are included. Custom and Original entries
        /// with the same relative path are merged into ONE entry (OpenRead/LoadXml
        /// will then automatically pick the Custom copy).
        /// </summary>
        public static List<string> EnumerateXml(string logicalFolder, bool recurse)
        {
            string folder = Normalize(logicalFolder);
            if (folder.Length > 0 && !folder.EndsWith("\\"))
                folder += "\\";

            var result = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string name in AllNames)
            {
                if (!name.StartsWith(Root, StringComparison.OrdinalIgnoreCase))
                    continue;

                // Path relative to "FacetData\".
                string rel = name.Substring(Root.Length);

                // Treat a Custom file as if it were the normal file (strip "Custom\").
                string logical = rel;
                if (logical.StartsWith("Custom\\", StringComparison.OrdinalIgnoreCase))
                    logical = logical.Substring("Custom\\".Length);

                if (!logical.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (folder.Length > 0 &&
                    !logical.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!recurse)
                {
                    // Skip files that live in a deeper subfolder.
                    string remainder = logical.Substring(folder.Length);
                    if (remainder.Contains('\\'))
                        continue;
                }

                result.Add(logical);
            }

            return result.ToList();
        }
    }
}