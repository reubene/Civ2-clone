﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Imagery
{
    class Draw
    {
        //Draw entire game map
        public Bitmap DrawMap()
        {
            Bitmap map = new Bitmap(Game.Data.MapXdim * 64, Game.Data.MapYdim * 32);    //define a bitmap for drawing map

            Squares square = new Squares();
                       
            using (Graphics graphics = Graphics.FromImage(map))
            {
                for (int col = 0; col < Game.Data.MapXdim; col++)
                {
                    for (int row = 0; row < Game.Data.MapYdim; row++)
                    {
                        graphics.DrawImage(square.Terrain(col, row), 64 * col + 32 * (row % 2) + 1, 16 * row + 1);
                    }
                }
            }
            
            return map;
        }

        //Draw city
        public Bitmap DrawCity(City city, bool citySizeWindow)
        {
            Bitmap map = new Bitmap(64, 48);    //define a bitmap for drawing map

            int cityStyle = Game.Civs[city.Owner].CityStyle;
            int sizeStyle = 0;
            //Determine city bitmap
            //For everything not modern or industrial => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
            //If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
            if (cityStyle < 4)
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                {
                    if (city.Size <= 3) { sizeStyle = 1; }
                    else if (city.Size > 3 && city.Size <= 5) { sizeStyle = 2; }
                    else { sizeStyle = 3; }

                }
                else
                {
                    if (city.Size <= 3) { sizeStyle = 0; }
                    else if (city.Size > 3 && city.Size <= 5) { sizeStyle = 1; }
                    else if (city.Size > 5 && city.Size <= 7) { sizeStyle = 2; }
                    else { sizeStyle = 3; }
                }
            }
            //If city is industrial => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
            //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
            else if (cityStyle == 4)
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                {
                    if (city.Size <= 4) { sizeStyle = 1; }
                    else if (city.Size > 4 && city.Size <= 7) { sizeStyle = 2; }
                    else { sizeStyle = 3; }

                }
                else
                {
                    if (city.Size <= 4) { sizeStyle = 0; }
                    else if (city.Size > 4 && city.Size <= 7) { sizeStyle = 1; }
                    else if (city.Size > 7 && city.Size <= 10) { sizeStyle = 2; }
                    else { sizeStyle = 3; }
                }
            }
            //If city is modern => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
            //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
            else
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                {
                    if (city.Size <= 4) { sizeStyle = 1; }
                    else if (city.Size > 4 && city.Size <= 10) { sizeStyle = 2; }
                    else { sizeStyle = 3; }

                }
                else
                {
                    if (city.Size <= 4) { sizeStyle = 0; }
                    else if (city.Size > 4 && city.Size <= 10) { sizeStyle = 1; }
                    else if (city.Size > 10 && city.Size <= 18) { sizeStyle = 2; }
                    else { sizeStyle = 3; }
                }
            }

            using (Graphics graphics = Graphics.FromImage(map))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (!Array.Exists(city.Improvements, element => element.Type == ImprovementType.CityWalls))  //no city walls
                {
                    graphics.DrawImage(Images.City[cityStyle, sizeStyle], 0, 0);
                    if (citySizeWindow) //Draw city size window
                    {
                        graphics.DrawRectangle(new Pen(Color.Black), Images.citySizeWindowLoc[cityStyle, sizeStyle, 0] - 1, Images.citySizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13);  //rectangle
                        graphics.FillRectangle(new SolidBrush(Images.CivColors[city.Owner]), Images.citySizeWindowLoc[cityStyle, sizeStyle, 0], Images.citySizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), Images.citySizeWindowLoc[cityStyle, sizeStyle, 0] + 4, Images.citySizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text
                    }
                    graphics.DrawImage(Images.CityFlag[city.Owner], Images.cityFlagLoc[cityStyle, sizeStyle, 0] - 3, Images.cityFlagLoc[cityStyle, sizeStyle, 1] - 17);     //Draw city flag
                }
                else
                {
                    graphics.DrawImage(Images.CityWall[cityStyle, sizeStyle], 0, 0);         
                    if (citySizeWindow)
                    {
                        graphics.DrawRectangle(new Pen(Color.Black), Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] - 1, Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13); //Draw city (+Wall) size window
                        graphics.FillRectangle(new SolidBrush(Images.CivColors[city.Owner]), Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0], Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] + 4, Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text    
                    }                
                    graphics.DrawImage(Images.CityFlag[city.Owner], Images.cityWallFlagLoc[cityStyle, sizeStyle, 0] - 3, Images.cityWallFlagLoc[cityStyle, sizeStyle, 1] - 17);    //Draw city flag
                }

                sf.Dispose();
            }

            return map;
        }

        //Draw terrain in city form
        public Bitmap DrawCityFormMap(City city)
        {
            Bitmap map = new Bitmap(4 * 64, 4 * 32);    //define a bitmap for drawing map

            Squares square = new Squares();

            int x = 2 * city.X + (city.Y % 2);  //first convert coordinates to civ2-style
            int y = city.Y;
            Bitmap image;
            using (Graphics graphics = Graphics.FromImage(map))
            {
                for (int col = 0; col < 4; col++)
                {
                    for (int row = 0; row < 7; row++)
                    {
                        int x_ = x - 3 + 2 * col + (y % 2);
                        int y_ = y - 3 + row;
                        if (!((row == 0 & col == 0) || (row == 0 & col == 3) || (row == 6 & col == 0) || (row == 6 & col == 3) || (col == 3 & (row == 1 || row == 3 || row == 5))))
                        {
                            if (x_ >= 0 && x_ < 2 * Game.Data.MapXdim && y_ >= 0 && y_ < Game.Data.MapYdim)
                            {
                                image = square.Terrain((x_ - (y_ % 2)) / 2, y_);
                            }
                            else
                            {
                                image = Images.Blank; 
                            }
                            graphics.DrawImage(image, 64 * col + 32 * (row % 2), 16 * row);
                        }
                    }
                }                
                graphics.DrawImage(DrawCity(city, false), 64 * 1 + 32 * (3 % 2) + 1, 16 * 2 + 1);
            }

            return map;
        }
    }
}
