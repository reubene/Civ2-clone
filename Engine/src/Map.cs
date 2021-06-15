using System;
using System.Collections;
using System.Collections.Generic;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using ExtensionMethods;

namespace Civ2engine
{
    public class Map : BaseInstance
    {
        public int MapIndex { get; } = 0;
        public int XDim { get; internal set; }
        public int YDim { get; internal set; }
        public int ResourceSeed { get; internal set; }
        public int LocatorXdim { get; private set; }
        public int LocatorYdim { get; private set; }
        public bool MapRevealed { get; set; }
        public int WhichCivsMapShown { get; set; }
        public Tile[,] Tile { get; set; }
        public bool[,][] Visibility { get; set; } // Visibility of tiles for each civ
        
        public bool IsValidTileC2(int xC2, int yC2)
        {
            var x = (((xC2 + 2 * XDim) % (2 * XDim)) - yC2 % 2);
            return -1 < x && x < XDim && -1 < yC2 && yC2 < YDim;
        }
        public ITerrain TileC2(int xC2, int yC2) => Tile[(((xC2 + 2 * XDim) % (2 * XDim)) - yC2 % 2) / 2, yC2]; // Accepts tile coords in civ2-style and returns the correct Tile (you can index beyond E/W borders for drawing round world)
        public bool IsTileVisibleC2(int xC2, int yC2, int civ) => MapRevealed || Visibility[( ((xC2 + 2 * XDim) % (2 * XDim)) - yC2 % 2 ) / 2, yC2][civ];   // Returns Visibility for civ2-style coords (you can index beyond E/W borders for drawing round world)
        public bool TileHasEnemyUnit(int xC2, int yC2, UnitType unitType) => (Game.UnitsHere(xC2, yC2).Count == 1) && (Game.UnitsHere(xC2, yC2)[0].Type == unitType);

        private int _zoom;
        public int Zoom     // -7 (min) ... 8 (max), 0=std.
        {
            get { return _zoom; }
            set
            {
                _zoom = Math.Max(Math.Min(value, 8), -7);
            }
        }
        public int Xpx => 4 * (_zoom + 8);    // Length of 1 map square in X
        public int Ypx => 2 * (_zoom + 8);    // Length of 1 map square in Y
        public int[] StartingClickedXY { get; set; }    // Last tile clicked with your mouse on the map. Gives info where the map should be centered (further calculated in MapPanel).
        private int[] _activeXY;
        public int[] ActiveXY   // Coords of either active unit or view piece
        {
            get
            {
                if (!ViewPieceMode )
                {
                    _activeXY = new [] { Game.GetActiveUnit.X, Game.GetActiveUnit.Y };
                }
                return _activeXY;
            }
            set => _activeXY = value;
        }
        private bool _viewPieceMode;
        public bool ViewPieceMode 
        {
            get => Game.GetActiveUnit == null || !Game.GetActiveCiv.AnyUnitsAwaitingOrders || _viewPieceMode;
            set => _viewPieceMode = value;
        }

        /// <summary>
        /// Generate first instance of terrain tiles by importing game data.
        /// </summary>
        /// <param name="data">Game data.</param>
        /// <param name="rules">Game rules.</param>
        public void PopulateTilesFromGameData(GameData data, Rules rules)
        {
            XDim = data.MapXdim;
            YDim = data.MapYdim;
            ResourceSeed = data.MapResourceSeed;
            LocatorXdim = data.MapLocatorXdim;
            LocatorYdim = data.MapLocatorYdim;
            Visibility = data.MapVisibilityCivs;

            Tile = new Tile[XDim, YDim];
            for (int col = 0; col < XDim; col++)
            {
                for (int row = 0; row < YDim; row++)
                {
                    var terrain = data.MapTerrainType[col, row];
                    Tile[col, row] = new Tile(2 * col + (row % 2), row, rules.Terrains[MapIndex][(int) terrain], Map.ResourceSeed)
                    {
                        River = data.MapRiverPresent[col, row],
                        Resource = data.MapResourcePresent[col, row],
                        //UnitPresent = data.MapUnitPresent[col, row],  // you can find this out yourself
                        //CityPresent = data.MapCityPresent[col, row],  // you can find this out yourself
                        Irrigation = data.MapIrrigationPresent[col, row],
                        Mining = data.MapMiningPresent[col, row],
                        Road = data.MapRoadPresent[col, row],
                        Railroad = data.MapRailroadPresent[col, row],
                        Fortress = data.MapFortressPresent[col, row],
                        Pollution = data.MapPollutionPresent[col, row],
                        Farmland = data.MapFarmlandPresent[col, row],
                        Airbase = data.MapAirbasePresent[col, row],
                        Island = data.MapIslandNo[col, row]
                    };
                }
            }
        }

        public IEnumerable<int[]> CityRadius(int[] xy)
        {
            var evenOdd = xy[1] % 2;
            var coords = new [] { (xy[0] - evenOdd) / 2, xy[1] };
            var offsets = new List<int[]>
            {
                new[] {0 + evenOdd, -1},
                new[] {0 + evenOdd, 1},
                new[] {1 + evenOdd, -1},
                new[] {1, 0},
                new[] {1 + evenOdd, 1},
                new[] {1, 2},
                new[] {1, -2},
                new[] {0, -2},
                new[] {0 + evenOdd, -3},
                new[] {0, 2},
                new[] {0 + evenOdd, 3},
                new[] {-1, 2},
                new[] {-1 + evenOdd, 1},
                new[] {-1, 0},
                new[] {-1 + evenOdd, -1},
                new[] {-2 + evenOdd, -1},
                new[] {-2 + evenOdd, 1},
                new[] {-1, -2},
                new[] {-1 + evenOdd, -3},
                new[] {-1 + evenOdd, 3}
            };
            
            yield return coords;
            foreach (var offset in offsets)
            {
                var x = coords[0] + offset[0];
                var y = coords[1] + offset[1];
                if(x < 0 || x >= XDim || y < 0 || y >= YDim) continue;
                yield return new[] {x, y};
            }
        }
        
        public void SetStartingVisibilityC2(int[] unitXy, int ownerId)
        {
            foreach (var point in this.CityRadius(unitXy))
            {
                Visibility[point[0], point[1]][ownerId] = true;
            }
        }

        public IEnumerable<Tile> DirectNeighbours(Tile candidate)
        {
            var evenOdd = candidate.Y % 2;
            var coords = new [] { (candidate.X - evenOdd) / 2, candidate.Y };
            var offsets = new List<int[]>
            {
                new[] {0 + evenOdd, -1},
                new[] {0 + evenOdd, 1},
                new[] {-1 + evenOdd, 1},
                new[] {-1 + evenOdd, -1}
            };
            foreach (var offset in offsets)
            {
                var x = coords[0] + offset[0];
                var y = coords[1] + offset[1];
                if (x < 0 || x >= XDim || y < 0 || y >= YDim) continue;
                yield return Tile[x, y];
            }
        }
    }
}
