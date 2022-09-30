using System;
using System.Collections.Generic;
using System.IO;

namespace GameEngine.Font
{
    public static class FontLibrary
    {
        #region storage

        private static Dictionary<int /*id*/, Font> Cache { get; set; } = new Dictionary<int, Font>();

        #endregion

        #region routines

        public static Font Get(int id)
            => Cache[id];

        public static bool TryGet(int id, out Font font)
            => Cache.TryGetValue(id, out font);

        public static bool TryGet(string name, out Font font)
            => Cache.TryGetValue(name.GetHashCode(), out font);

        private static Font GetOrCreate(string name, Func<Font> ctorFont)
        {
            int id = name.GetHashCode();
            if (!Cache.TryGetValue(id, out Font textureResource))
                Cache.Add(id, textureResource = ctorFont());

            return textureResource;
        }

        public static Font GetOrCreateFromFile(string filePath)
        {
            return GetOrCreate(filePath, () => new Font(filePath, filePath.GetHashCode()));
        }

        public static Font GetOrCreateFromBytes(string name, byte[] bytes)
        {
            return GetOrCreate(name, () => new Font(bytes, name.GetHashCode()));
        }

        public static void Delete(Font font)
        {
            Cache.Remove(font.Id);
            font.Dispose();
        }

        public static void DisposeAll()
        {
            foreach (Font font in Cache.Values)
                font.Dispose();
            Cache = default;
        }

        #endregion
    }
}