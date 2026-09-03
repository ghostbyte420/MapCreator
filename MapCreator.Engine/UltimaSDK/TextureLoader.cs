using System.Drawing;

namespace MapCreator.Engine.UltimaSDK
{
    /// <summary>
    /// Wrapper for loading terrain textures using the existing Art system
    /// </summary>
    public static class TextureLoader
    {
        /// <summary>
        /// Gets a 44x44 land texture by texture ID using the existing Art.GetLand method
        /// </summary>
        public static Bitmap GetLandTexture(int textureId)
        {
            // The Art.GetLand method already handles:
            // - Diamond pattern decoding
            // - Caching
            // - File index lookup
            // - Proper color format (16bppArgb1555)
            return Art.GetLand(textureId);
        }
    }
}