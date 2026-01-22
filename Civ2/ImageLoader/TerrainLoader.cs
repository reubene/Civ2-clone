using Civ2engine;
using Civ2engine.IO;
using Civ2engine.Terrains;
using Model;
using Model.Images;
using Model.ImageSets;
using Raylib_CSharp.Images;
using Raylib_CSharp.Transformations;
using RaylibUI;
using RaylibUtils;
using System.Numerics;
using Model.Graphics;
using Rectangle = Model.Rectangle;

namespace Civ2.ImageLoader
{
    public static class TerrainLoader
    {
        public static void LoadTerrain(Ruleset ruleset, IUserInterface active)
        {
            active.TileSets.Clear();
            for (var i = 0; i < active.ExpectedMaps; i++)
            {
                active.TileSets.Add(LoadTerrain(ruleset, i, active));
            }
        }

        private static TerrainSet LoadTerrain(Ruleset ruleset, int index, IUserInterface active)
        {
            // Initialize objects
            var terrain = new TerrainSet(64, 32);

            // Get dither tile before making it transparent
            var ditherTile = new ComputedImage(MapIndexChange(active.PicSources["dither"][0], index))
                .LoadColors(0)
                .ReplaceColor(Color.Black, Color.White)
                .ReplaceColor(new Color(255, 0, 255, 0), Color.Black)
                .ReplaceColor(0, Color.Black);

            terrain.BaseTiles = active.PicSources["base1"].Select(t => MapIndexChange((BitmapStorage)t, index)).ToArray();

            terrain.Specials =
            [
                active.PicSources["special1"].Select(s => MapIndexChange((BitmapStorage) s, index)).ToArray(),
                active.PicSources["special2"].Select(s => MapIndexChange((BitmapStorage)s, index)).ToArray()
            ];

            terrain.Blank = MapIndexChange((BitmapStorage)active.PicSources["blank"][0], index);

            // 4 small dither tiles (base mask must be B/W)
            terrain.DitherMask =
            [
                new ComputedImage(ditherTile, new Rectangle(32, 0, 32, 16)),
                new ComputedImage(ditherTile, new Rectangle(32, 16, 32, 16)),
                new ComputedImage(ditherTile, new Rectangle(0, 16, 32, 16)),
                new ComputedImage(ditherTile, new Rectangle(0, 0, 32, 16))
            ];

            terrain.DitherMaps =
            [
                BuildDitherMaps(terrain.DitherMask[0], terrain.BaseTiles, 32, 0, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[1], terrain.BaseTiles, 32, 16, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[2], terrain.BaseTiles, 0, 16, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[3], terrain.BaseTiles, 0, 0, terrain.Blank)
            ];

            terrain.River = active.PicSources["river"].Select(r => MapIndexChange((BitmapStorage)r, index)).ToArray();
            terrain.Forest = active.PicSources["forest"].Select(r => MapIndexChange((BitmapStorage)r, index)).ToArray();
            terrain.Mountains = active.PicSources["mountain"].Select(r => MapIndexChange((BitmapStorage)r, index)).ToArray();
            terrain.Hills = active.PicSources["hill"].Select(r => MapIndexChange((BitmapStorage)r, index)).ToArray();
            terrain.RiverMouth = active.PicSources["riverMouth"].Select(r => MapIndexChange((BitmapStorage)r, index)).ToArray();

            terrain.Coast = new IImageSource[8, 4];
            for (var i = 0; i < 8; i++)
            {
                terrain.Coast[i, 0] = MapIndexChange((BitmapStorage)active.PicSources["coastline"][4 * i + 0], index); // N
                terrain.Coast[i, 1] = MapIndexChange((BitmapStorage)active.PicSources["coastline"][4 * i + 1], index); // S
                terrain.Coast[i, 2] = MapIndexChange((BitmapStorage)active.PicSources["coastline"][4 * i + 2], index); // W
                terrain.Coast[i, 3] = MapIndexChange((BitmapStorage)active.PicSources["coastline"][4 * i + 3], index); // E
            }

            // Road & railroad
            terrain.ImprovementsMap = new Dictionary<int, ImprovementGraphic>();

            var roadGraphics = new ImprovementGraphic
            {
                Levels = new IImageSource[2, 9]
            };

            terrain.ImprovementsMap.Add(ImprovementTypes.Road, roadGraphics);

            for (var i = 0; i < 9; i++)
            {
                roadGraphics.Levels[0, i] = MapIndexChange((BitmapStorage)active.PicSources["road"][i], index);
                roadGraphics.Levels[1, i] = MapIndexChange((BitmapStorage)active.PicSources["railroad"][i], index);
            }

            terrain.ImprovementsMap.Add(ImprovementTypes.Irrigation, new ImprovementGraphic
            {
                Levels = new[,]
                {
                    { MapIndexChange((BitmapStorage)active.PicSources["irrigation"][0], index) },
                    { MapIndexChange((BitmapStorage)active.PicSources["farmland"][0], index) }
                }
            });

            terrain.ImprovementsMap[ImprovementTypes.Mining] = new ImprovementGraphic
                { Levels = new[,] { { MapIndexChange((BitmapStorage)active.PicSources["mine"][0], index) } } };

            terrain.ImprovementsMap[ImprovementTypes.Pollution] = new ImprovementGraphic
                { Levels = new[,] { { MapIndexChange((BitmapStorage)active.PicSources["pollution"][0], index) } } };

            //Note airbase and fortress are now loaded directly by the cities loader
            terrain.GrasslandShield = MapIndexChange((BitmapStorage)active.PicSources["shield"][0], index);

            return terrain;
        }

        private static DitherMap BuildDitherMaps(IImageSource mask, IImageSource[] baseTiles, int offsetX, int offsetY,
            IImageSource terrainBlank)
        {
            var sampleRect = new Rectangle(offsetX, offsetY, 32, 16);
            var totalTiles = baseTiles.Length + 1;
            var ditherMaps = new IImageSource[totalTiles];
            for (var i = 0; i < baseTiles.Length; i++)
            {
                ditherMaps[i] = new ComputedImage(baseTiles[i], sampleRect, mask);
            }

            ditherMaps[^1] = new ComputedImage(terrainBlank, sampleRect, mask);

            return new DitherMap { X = offsetX, Y = offsetY, Images = ditherMaps };
        }

        /// <summary>
        /// For TERRAIN3, 4, 5, etc.
        /// </summary>
        private static IImageSource MapIndexChange(IImageSource source, int mapIndex)
        {
            if (source is not BitmapStorage storage)
            {
                throw new ArgumentException("Source is not a BitmapStorage");
            }

            var file = storage.Filename;

            if (mapIndex != 0)
            {
                int.TryParse(file[^1].ToString(), out int currentIndex);
                int newIndex = mapIndex * 2 + currentIndex;
                file = $"{file.Remove(file.Length - 1, 1)}{newIndex}";
            }

            return new BitmapStorage(file, storage.Location, storage.TransparencyPixel, storage.SearchFlagLoc);
        }
    }
}   