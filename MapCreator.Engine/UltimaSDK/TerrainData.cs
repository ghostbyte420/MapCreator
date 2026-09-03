using MapCreator.Engine.Compiler;

namespace MapCreator.Engine.UltimaSDK
{
    /// <summary>
    /// Maps terrain IDs to texture IDs for rendering
    /// </summary>
    public static class TerrainData
    {
        private static ClsTerrainTable _terrainTable;

        /// <summary>
        /// Initialize the terrain table (call this once at startup)
        /// </summary>
        public static void Initialize()
        {
            if (_terrainTable == null)
            {
                _terrainTable = new ClsTerrainTable();
                _terrainTable.Load();
            }
        }

        /// <summary>
        /// Converts a terrain ID to its corresponding TileID for art file lookup
        /// </summary>
        public static int LandTileToTextureId(int terrainId)
        {
            // Ensure the terrain table is loaded
            if (_terrainTable == null)
            {
                Initialize();
            }

            // Look up the terrain by ID using the TerrianGroup method
            ClsTerrain terrain = _terrainTable.TerrianGroup(terrainId);

            if (terrain != null)
            {
                // Return the TileID (this is what the art files use)
                return terrain.TileID;
            }

            // Fallback: return the terrain ID if lookup fails
            return terrainId;
        }
    }
}