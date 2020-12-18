﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Timers;
using ExtensionMethods;
using civ2.Events;
using civ2.Bitmaps;
using civ2.Units;
using civ2.Enums;

namespace civ2.Forms
{
    public partial class MapPanel : Civ2panel
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        private Main Main;
        private static List<Bitmap> AnimationFrames;
        private int MapGridVar { get; set; }        // Style of map grid presentation
        private System.Windows.Forms.Timer AnimationTimer;   // Timer for blinking (unit or viewing piece), moving unit, etc.
        private AnimationType AnimType { get; set; }
        private int AnimationCount { get; set; }

        private int[] PanelMap_offset, MapPanel_offset, CentrXY, ActiveOffset, CentrOffset, ClickedXY;
        private Rectangle mapRect1, mapRect2;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        public MapPanel(Main parent, int _width, int _height) : base(_width, _height, "", false)
        {
            this.Main = parent;

            this.Paint += new PaintEventHandler(MapPanel_Paint);

            Game.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            Game.OnUnitEvent += UnitEventHappened;
            Game.OnPlayerEvent += PlayerEventHappened;
            MinimapPanel.OnMapEvent += MapEventHappened;
            StatusPanel.OnMapEvent += MapEventHappened;
            Main.OnMapEvent += MapEventHappened;
            Main.OnCheckIfCityCanBeViewed += CheckIfCityCanBeViewed;

            NoSelectButton ZoomINButton = new NoSelectButton
            {
                Location = new Point(11, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                //BackgroundImage = ModifyImage.ResizeImage(Images.ZoomIN, 23, 23)
                BackgroundImage = Images.ZoomIN
            };
            ZoomINButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomINButton);
            ZoomINButton.Click += ZoomINclicked;

            NoSelectButton ZoomOUTButton = new NoSelectButton
            {
                Location = new Point(36, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                //BackgroundImage = ModifyImage.ResizeImage(Images.ZoomOUT, 23, 23)
                BackgroundImage = Images.ZoomOUT
            };
            ZoomOUTButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomOUTButton);
            ZoomOUTButton.Click += ZoomOUTclicked;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = null;
            DrawPanel.BackColor = Color.Black;
            DrawPanel.Paint += DrawPanel_Paint;
            DrawPanel.MouseClick += DrawPanel_MouseClick;

            // Center the map view and draw map
            MapGridVar = 0;
            AnimType = AnimationType.UpdateMap;
            ReturnCoordsAtMapViewChange(Game.StartingClickedXY);

            if (Main.ViewPieceMode)
                AnimType = AnimationType.ViewPiece;
            else
                AnimType = AnimationType.UnitWaiting;

            // Timer for waiting unit/ viewing piece
            AnimationTimer = new System.Windows.Forms.Timer();
            AnimationTimer.Tick += new EventHandler(Animation_Tick);
            StartAnimation(AnimType);
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            // Title
            StringFormat sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString($"{Game.PlayerCiv.Adjective} Map", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString($"{Game.PlayerCiv.Adjective} Map", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            e.Dispose();
            sf.Dispose();
        }

        // Draw map here
        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Calculate these once just for less computations
            int[] startingSqXYpx = new int[] { 0, 0 };

            // Draw map (for round world draw it from 2 parts)
            e.Graphics.DrawImage(Map.ActiveCivMap, PanelMap_offsetpx[0], PanelMap_offsetpx[1], mapRect1, GraphicsUnit.Pixel);
            e.Graphics.DrawImage(Map.ActiveCivMap, PanelMap_offsetpx[0] + mapRect1.Width, PanelMap_offsetpx[1], mapRect2, GraphicsUnit.Pixel);

            // Draw animation
            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        e.Graphics.DrawImage(AnimationFrames[AnimationCount % 2], ActiveOffsetPx[0], ActiveOffsetPx[1] - Game.Ypx);
                        break;
                    }
                case AnimationType.ViewPiece:
                    {
                        if (AnimationCount % 2 == 0) 
                            e.Graphics.DrawImage(Images.ViewPiece, ActiveOffsetPx[0], ActiveOffsetPx[1]);
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        IUnit unit = Game.ActiveUnit;
                        e.Graphics.DrawImage(AnimationFrames[Game.ActiveUnit.MovementCounter], unit.LastXYpx[0] - startingSqXYpx[0] - 2 * Game.Xpx, unit.LastXYpx[1] - startingSqXYpx[1] - 2 * Game.Ypx);
                        break;
                    } 
            }

            e.Dispose();
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int[] coords = Ext.PxToCoords(e.Location.X, e.Location.Y, Game.Zoom);
            ClickedXY = new int[] { (MapPanel_offset[0] + coords[0]) % (2 * Map.Xdim), MapPanel_offset[1] + coords[1] };  // Coordinates of clicked square

            if (e.Button == MouseButtons.Left)
            {
                if (Game.GetCities.Any(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]))    // City clicked
                {
                    if (Main.ViewPieceMode) Game.ActiveXY = ClickedXY;
                    //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]));
                    //cityForm.Show();
                }
                else if (Game.GetUnits.Any(unit => unit.X == ClickedXY[0] && unit.Y == ClickedXY[1]))    // Unit clicked
                {
                    int clickedUnitIndex = Game.GetUnits.FindIndex(a => a.X == ClickedXY[0] && a.Y == ClickedXY[1]);
                    if (!Game.GetUnits[clickedUnitIndex].TurnEnded)
                    {
                        Game.ActiveUnit = Game.GetUnits[clickedUnitIndex];
                        Main.ViewPieceMode = false;
                        OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
                        StartAnimation(AnimationType.UnitWaiting);
                    }
                    else
                    {
                        //TODO: determine what happens if unit has ended turn...
                    }
                    MapViewChange(ClickedXY);
                }
                else    // Something else clicked
                {
                    if (Main.ViewPieceMode) Game.ActiveXY = ClickedXY;
                    MapViewChange(ClickedXY);
                }
            }
            else    // Right click
            {
                Main.ViewPieceMode = true;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
                Game.ActiveXY = ClickedXY;
                MapViewChange(ClickedXY);
                StartAnimation(AnimationType.ViewPiece);
            }
        }

        private void MapViewChange(int[] newCenterCoords)
        {
            ReturnCoordsAtMapViewChange(newCenterCoords);
        }

        public int ToggleMapGrid()
        {
            MapGridVar++;
            if (MapGridVar > 3) MapGridVar = 0;
            //Options.Grid = (MapGridVar != 0) ? true : false;
            Refresh();
            return MapGridVar;
        }

        #region ZoomInOut events
        public void ZoomOUTclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom--;
            Map.SetNewActiveMapPic();
            DrawPanel.Refresh(); 
        }
        public void ZoomINclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom++;
            ReturnCoordsAtMapViewChange(CentrXY);
            Map.SetNewActiveMapPic();
            StartAnimation(AnimType);
            DrawPanel.Refresh(); 
        }
        public void MaxZoomINclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom = 16;
            Map.SetNewActiveMapPic();
            DrawPanel.Refresh(); 
        }
        public void MaxZoomOUTclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom = 1;
            Map.SetNewActiveMapPic(); 
            DrawPanel.Refresh(); 
        }
        public void StandardZOOMclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom = 8;
            Map.SetNewActiveMapPic(); 
            DrawPanel.Refresh(); 
        }
        public void MediumZoomOUTclicked(Object sender, EventArgs e) 
        {
            Game.Zoom = 5;
            Map.SetNewActiveMapPic();
            DrawPanel.Refresh();
        }
        #endregion



        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        MapViewChange(e.CenterSqXY);
                        break;
                    }
                case MapEventType.SwitchViewMovePiece:
                    {
                        if (Main.ViewPieceMode) StartAnimation(AnimationType.ViewPiece);
                        else StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
                case MapEventType.ViewPieceMoved:
                    {
                        StartAnimation(AnimationType.ViewPiece);
                        break;
                    }
                case MapEventType.ToggleBetweenCurrentEntireMapView:
                    {
                        DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        Update();
                        break;
                    }
                default: break;
            }
        }

        private void PlayerEventHappened(object sender, PlayerEventArgs e)
        {
            switch (e.EventType)
            {
                case PlayerEventType.NewTurn:
                    {
                        if (Game.ActiveUnit != null) Main.ViewPieceMode = false;
                        AnimationTimer.Stop();
                        AnimationCount = 0;
                        AnimationTimer.Start();
                        break;
                    }
            }
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                // Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                    {
                        AnimationCount = Game.ActiveUnit.MovementCounter;
                        if (AnimationCount == 0) StartAnimation(AnimationType.UnitMoving);
                        //DrawAnimation();
                        break;
                    }
                case UnitEventType.StatusUpdate:
                    {
                        StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
                case UnitEventType.NewUnitActivated:
                    {
                        ReturnCoordsAtMapViewChange(Game.ActiveXY);
                        StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
            }
        }

        private void InitiateWaitAtTurnEnd(object sender, WaitAtTurnEndEventArgs e)
        {
            Main.ViewPieceMode = true;
            AnimationTimer.Stop();
            AnimationCount = 0;
            AnimationTimer.Start();
        }

        // If ENTER pressed when view piece above city --> enter city view
        private void CheckIfCityCanBeViewed(object sender, CheckIfCityCanBeViewedEventArgs e)
        {
            if (Main.ViewPieceMode && Game.GetCities.Any(city => city.X == Game.ActiveXY[0] && city.Y == Game.ActiveXY[1]))
            {
                //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]));
                //cityForm.Show();
            }
        }

        #region Animation
        private void StartAnimation(AnimationType anim)
        {
            switch (anim)
            {
                case AnimationType.UpdateMap:
                    AnimationTimer.Stop();
                    AnimationCount = 0;
                    DrawPanel.Invalidate();
                    break;
                case AnimationType.UnitWaiting:
                    //AnimType = AnimationType.UnitWaiting;
                    AnimationTimer.Stop();
                    AnimationFrames = GetAnimationFrames.UnitWaiting(Main.ViewPieceMode);
                    AnimationCount = 0;
                    AnimationTimer.Interval = 200;    // ms                    
                    AnimationTimer.Start();
                    break;
                case AnimationType.UnitMoving:
                    //AnimType = AnimationType.UnitMoving;
                    AnimationFrames = GetAnimationFrames.UnitMoving(Main.ViewPieceMode);
                    break;
                case AnimationType.ViewPiece:
                    //AnimType = AnimationType.ViewPieces;
                    AnimationTimer.Stop();
                    AnimationCount = 0;
                    AnimationTimer.Interval = 200;    // ms                    
                    AnimationTimer.Start();
                    break;
            }
        }

        private void Animation_Tick(object sender, EventArgs e)
        {
            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (AnimationCount == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle(ActiveOffsetPx[0], ActiveOffsetPx[1] - Game.Ypx, 2 * Game.Xpx, 3 * Game.Ypx));
                        break;
                    }
                case AnimationType.ViewPiece:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (AnimationCount == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle(ActiveOffsetPx[0], ActiveOffsetPx[1], 2 * Game.Xpx, 2 * Game.Ypx));
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        DrawPanel.Invalidate(new Rectangle((Game.ActiveXY[0] - MapPanel_offset[0]) * 32 - 64, (Game.ActiveXY[1] - MapPanel_offset[1]) * 16 - 48, 3 * 64, 3 * 32 + 16));
                        Update();
                        if (AnimationCount == 7)  // Unit has completed movement
                        {
                            // First update world map with new visible tiles
                            Game.UpdateWorldMapAfterUnitHasMoved();

                            // Update the original world map image with image of new location of unit & redraw whole map
                            IUnit unit = Game.Instance.ActiveUnit;
                            // Game.CivsMap[Game.Instance.ActiveCiv.Id] = ModifyImage.MergedBitmaps(Game.CivsMap[Game.Instance.ActiveCiv.Id], AnimationFrames[TimerCounter], 32 * unit.LastXY[0] - 64, 16 * unit.LastXY[1] - 48);
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                            Update();

                            // Then stop animation
                            StartAnimation(AnimationType.UpdateMap);

                            // Check if unit moved outside map view -> map view needs to be updated
                            if (UnitMovedOutsideMapView)
                            {
                                ReturnCoordsAtMapViewChange(Game.ActiveXY);
                                DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                                Update();
                            }
                        }
                        break;
                    }

            }

            AnimationCount++;
        }
        #endregion

        private bool UnitMovedOutsideMapView
        {
            get
            {
                if (Game.ActiveXY[0] >= 2 * Map.Xdim ||
                Game.ActiveXY[0] < 0 ||
                Game.ActiveXY[1] >= Map.Ydim ||
                Game.ActiveXY[1] < 0)
                    return true;
                else
                    return false;
            }
        }

        // Function which sets various variables for drawing map on grid
        // CentrXY ... Central point of Draw Panel (XY map coords)
        // CentrOffset ... Central point of Draw Panel (Panel Offset)
        // ActiveXY ... Active square (XY map coords)
        // ActiveOffset ... Active square (Panel Offset)
        // PanelMap_offset ... Offset of NW point of panel from maps NW point (=0 if map is larger than panel)
        // MapPanel_offset ... Offset of map NW point from panel NW point, in squares (=0 if panel is larger than map in any direction)
        // mapRect1, mapRect2 ... rectangles for drawing 1st & 2nd part of map
        private void ReturnCoordsAtMapViewChange(int[] proposedCentralCoords)
        {
            CentrXY = proposedCentralCoords;
            ActiveOffset = new int[] { 0, 0 };
            PanelMap_offset = new int[] { 0, 0 };
            MapPanel_offset = new int[] { 0, 0 };
            mapRect1 = new Rectangle(0, 0, 0, 0);
            mapRect2 = new Rectangle(0, 0, 0, 0);

            int mapWidth = Game.Xpx * (2 * Map.Xdim + 1);
            int mapHeight = Game.Ypx * (Map.Ydim + 1);

            // No of squares of panel and map
            int[] PanelSq = new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (2 * Game.Xpx)), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (2 * Game.Ypx)) };
            int[] MapSq = new int[] { 2 * Map.Xdim + 1, Map.Ydim + 1 };

            CentrOffset = new int[] { PanelSq[0] / 2, PanelSq[1] / 2 };

            // First determine the Y-central coordinate
            if (PanelSq[1] > MapSq[1])    // Panel is larger than map in Y, center the map in panel center
            {
                PanelMap_offset[1] = (PanelSq[1] - MapSq[1]) / 2;
                CentrXY[1] = MapSq[1] / 2;
                ActiveOffset[1] = PanelMap_offset[1] + Game.ActiveXY[1];
                mapRect1.Height = mapHeight;
            }
            else    // Map wider than panel (in Y)
            {
                if (CentrXY[1] < CentrOffset[1])    // Limit Drawing Panel so it's not going beyond the map in north
                {
                    CentrXY[1] = CentrOffset[1];
                    ActiveOffset[1] = Game.ActiveXY[1];
                }
                else if (CentrXY[1] > MapSq[1] - CentrOffset[1])  // Limit Drawing Panel so it's not going below the map in south
                {
                    MapPanel_offset[1] = MapSq[1] - PanelSq[1];
                    CentrXY[1] = MapPanel_offset[1] + CentrOffset[1];
                    ActiveOffset[1] = Game.ActiveXY[1] - MapPanel_offset[1];
                }
                else    // Drawing panel within map (Y-axis)
                {
                    MapPanel_offset[1] = CentrXY[1] - CentrOffset[1];
                    ActiveOffset[1] = Game.ActiveXY[1] - MapPanel_offset[1];
                }
                mapRect1.Height = DrawPanel.Height;
            }
            mapRect1.Y = Game.Ypx * MapPanel_offset[1];

            // Then determine X-coordinate
            if (PanelSq[0] > MapSq[0])    // Panel is larger than map in X, center the map in panel center
            {
                PanelMap_offset[0] = (PanelSq[0] - MapSq[0]) / 2;
                CentrXY[0] = MapSq[0] / 2;
                ActiveOffset[0] = PanelMap_offset[0] + Game.ActiveXY[0];
                mapRect1.X = 0;
                mapRect1.Width = mapWidth;

                // If the tile at these coords doesn't exist, shift X to the right
                if (CentrXY[0] % 2 != 0 && CentrXY[1] % 2 == 0) CentrXY[0]++;
                if (CentrXY[0] % 2 == 0 && CentrXY[1] % 2 != 0) CentrXY[0]++;
            }
            else    // Map wider than panel
            {
                if (Game.Options.FlatEarth)
                {
                    if (CentrXY[0] < CentrOffset[1])  // Limit Drawing Panel so it's not going beyond the map in west
                    {
                        CentrXY[0] = CentrOffset[0];
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentrXY[0] % 2 != 0 && CentrXY[1] % 2 == 0) CentrXY[0]++;
                        if (CentrXY[0] % 2 == 0 && CentrXY[1] % 2 != 0) CentrXY[0]++;
                        ActiveOffset[0] = Game.ActiveXY[0];
                    }
                    else if (CentrXY[0] > MapSq[0] - CentrOffset[0])  // Limit Drawing Panel so it's not going below the map in east
                    {
                        CentrXY[0] = MapSq[0] - CentrOffset[0];
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentrXY[0] % 2 != 0 && CentrXY[1] % 2 == 0) CentrXY[0]++;
                        if (CentrXY[0] % 2 == 0 && CentrXY[1] % 2 != 0) CentrXY[0]++;

                        MapPanel_offset[0] = CentrXY[0] - CentrOffset[0];
                        ActiveOffset[0] = Game.ActiveXY[0] - MapPanel_offset[0];
                    }
                    else    // Drawing panel within map (X-axis)
                    {
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentrXY[0] % 2 != 0 && CentrXY[1] % 2 == 0) CentrXY[0]++;
                        if (CentrXY[0] % 2 == 0 && CentrXY[1] % 2 != 0) CentrXY[0]++;

                        MapPanel_offset[0] = CentrXY[0] - CentrOffset[0];
                        ActiveOffset[0] = Game.ActiveXY[0] - MapPanel_offset[0];
                    }
                    mapRect1.Width = DrawPanel.Width;
                    mapRect1.X = Game.Xpx * MapPanel_offset[0];
                }
                else    // Round world
                {
                    if (CentrXY[0] - CentrOffset[0] < 0)  // Panel reaches west of X=0 axis
                    {
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentrXY[0] % 2 != 0 && CentrXY[1] % 2 == 0) CentrXY[0]++;
                        if (CentrXY[0] % 2 == 0 && CentrXY[1] % 2 != 0) CentrXY[0]++;

                        MapPanel_offset[0] = MapSq[0] + (CentrXY[0] - CentrOffset[0]);
                        if (Game.ActiveXY[0] > MapPanel_offset[0])  // Active square on the left side of 2-part map
                            ActiveOffset[0] = Game.ActiveXY[0] - MapPanel_offset[0];
                        else      // Active square on the right side of 2-part map
                            ActiveOffset[0] = CentrOffset[0] - CentrXY[0] + Game.ActiveXY[0];

                        mapRect1.X = Game.Xpx * MapPanel_offset[0];
                        mapRect1.Width = mapWidth - mapRect1.X;
                        mapRect2.X = Game.Xpx;
                        mapRect2.Width = DrawPanel.Width - mapRect1.Width;
                    }
                    else if (CentrXY[0] + CentrOffset[0] > MapSq[0])  // Panel beyond map eastern edge
                    {
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentrXY[0] % 2 != 0 && CentrXY[1] % 2 == 0) CentrXY[0]++;
                        if (CentrXY[0] % 2 == 0 && CentrXY[1] % 2 != 0) CentrXY[0]++;

                        MapPanel_offset[0] = CentrXY[0] - CentrOffset[0];
                        if (Game.ActiveXY[0] > MapPanel_offset[0])  // Active square on the left side of 2-part map
                            ActiveOffset[0] = Game.ActiveXY[0] - MapPanel_offset[0];
                        else   // Active square on the right side of 2-part map
                            ActiveOffset[0] = MapSq[0] - MapPanel_offset[0] + Game.ActiveXY[0];

                        mapRect1.X = Game.Xpx * MapPanel_offset[0];
                        mapRect1.Width = mapWidth - mapRect1.X;
                        mapRect2.X = Game.Xpx;
                        mapRect2.Width = DrawPanel.Width - mapRect1.Width;
                    }
                    else
                    {
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentrXY[0] % 2 != 0 && CentrXY[1] % 2 == 0) CentrXY[0]++;
                        if (CentrXY[0] % 2 == 0 && CentrXY[1] % 2 != 0) CentrXY[0]++;

                        MapPanel_offset[0] = CentrXY[0] - CentrOffset[0];
                        ActiveOffset[0] = MapPanel_offset[0] + Game.ActiveXY[0];

                        mapRect1.Width = DrawPanel.Width;
                        mapRect1.X = Game.Xpx * MapPanel_offset[0];
                    }
                    mapRect2.Y = mapRect1.Y;
                    mapRect2.Height = mapRect1.Height;
                }
            }

            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, CentrXY, CentrOffset, ActiveOffset, PanelMap_offset, MapPanel_offset, mapRect1, mapRect2));
            DrawPanel.Invalidate();
        }

        private int[] PanelMap_offsetpx => new int[] { Game.Xpx * PanelMap_offset[0], Game.Ypx * PanelMap_offset[1] };
        private int[] MapPanel_offsetpx => new int[] { Game.Xpx * MapPanel_offset[0], Game.Ypx * MapPanel_offset[1] };
        private int[] ActiveOffsetPx => new int[] { Game.Xpx * ActiveOffset[0], Game.Ypx * ActiveOffset[1] };

    }
}
