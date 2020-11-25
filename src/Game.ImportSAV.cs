﻿using System;
using civ2.Enums;
using civ2.Units;
using civ2.Terrains;
using System.IO;

namespace civ2
{
    public partial class Game
    {
        private void ImportSAV(string SAVpath)
        {
            string bin;
            int intVal1, intVal2, intVal3, intVal4;

            //Read every byte
            byte[] bytes = File.ReadAllBytes(SAVpath);

            #region Start of saved game file
            //=========================
            //START OF SAVED GAME FILE
            //=========================
            // Determine game version
            if (bytes[10] == 39)        Version = GameVersionType.CiC;      // Conflicts (27 hex)
            else if (bytes[10] == 40)   Version = GameVersionType.FW;       // FW (28 hex)
            else if (bytes[10] == 44)   Version = GameVersionType.MGE;      // MGE (2C hex)
            else if (bytes[10] == 49)   Version = GameVersionType.ToT10;    // ToT1.0 (31 hex)
            else if (bytes[10] == 50)   Version = GameVersionType.ToT11;    // ToT1.1 (32 hex)
            else                        Version = GameVersionType.CiC;      // lower than Conflicts

            // Options
            // TODO: determine if randomizing villages/resources, randomizing player starting locations, select comp. opponents, accelerated sturtup options are selected from SAV file
            bool bloodlust                          = GetBit(bytes[12], 0);     // Bloodlust on/off            
            bool simplifiedCombat                   = GetBit(bytes[12], 3);     // Simplified combat on/off
            bool flatEarth                          = GetBit(bytes[13], 0);     // Flat/round earth
            bool dontRestartIfEliminated            = GetBit(bytes[13], 7);     // Don't restart if eliminated
            bool moveUnitsWithoutMouse              = GetBit(bytes[14], 0);     // Move units without mouse
            bool enterClosestCityScreen             = GetBit(bytes[14], 1);     // Enter closes city screen     
            bool grid                               = GetBit(bytes[14], 2);     // Grid on/off
            bool soundEffects                       = GetBit(bytes[14], 3);     // Sound effects on/off
            bool music                              = GetBit(bytes[14], 4);     // Music on/off
            bool cheatMenu                          = GetBit(bytes[15], 0);     // Cheat menu on/off
            bool alwaysWaitAtEndOfTurn              = GetBit(bytes[15], 1);     // Always wait at end of turn on/off
            bool autosaveEachTurn                   = GetBit(bytes[15], 2);     // Autosave each turn on/off
            bool showEnemyMoves                     = GetBit(bytes[15], 3);     // Show enemy moves on/off
            bool noPauseAfterEnemyMoves             = GetBit(bytes[15], 4);     // No pause after enemy moves on/off
            bool fastPieceSlide                     = GetBit(bytes[15], 5);     // Fast piece slide on/off
            bool instantAdvice                      = GetBit(bytes[15], 6);     // Instant advice on/off
            bool tutorialHelp                       = GetBit(bytes[15], 7);     // Tutorial help on/off
            bool animatedHeralds                    = GetBit(bytes[16], 2);     // Animated heralds on/off
            bool highCouncil                        = GetBit(bytes[16], 3);     // High council on/off
            bool civilopediaForAdvances             = GetBit(bytes[16], 4);     // Civilopedia for advances on/off
            bool throneRoomGraphics                 = GetBit(bytes[16], 5);     // Throne room graphics on/off
            bool diplomacyScreenGraphics            = GetBit(bytes[16], 6);     // Diplomacy screen graphics on/off
            bool wonderMovies                       = GetBit(bytes[16], 7);     // Wonder movies on/off
            bool cheatPenaltyWarning                = GetBit(bytes[20], 3);     // Cheat penalty/warning on/off
            bool announceWeLoveKingDay              = GetBit(bytes[22], 0);     // Announce we love king day on/off
            bool warnWhenFoodDangerouslyLow         = GetBit(bytes[22], 1);     // Warn when food dangerously low on/off
            bool announceCitiesInDisorder           = GetBit(bytes[22], 2);     // Announce cities in disorder on/off
            bool announceOrderRestored              = GetBit(bytes[22], 3);     // Announce order restored in cities on/off
            bool showNonCombatUnitsBuilt            = GetBit(bytes[22], 4);     // Show non combat units build on/off
            bool showInvalidBuildInstructions       = GetBit(bytes[22], 5);     // Show invalid build instructions on/off
            bool warnWhenCityGrowthHalted           = GetBit(bytes[22], 6);     // Warn when city growth halted on/off
            bool showCityImprovementsBuilt          = GetBit(bytes[22], 7);     // Show city improvements built on/off
            bool zoomToCityNotDefaultAction         = GetBit(bytes[23], 5);     // Zoom to city not default action on/off
            bool warnWhenPollutionOccurs            = GetBit(bytes[23], 6);     // Warn when pollution occurs on/off
            bool warnChangProductWillCostShields    = GetBit(bytes[23], 7);     // Warn when changing production will cost shileds on/off
            _options.SetOptions(simplifiedCombat, flatEarth, bloodlust, dontRestartIfEliminated, soundEffects, music, cheatMenu, alwaysWaitAtEndOfTurn, 
                                        autosaveEachTurn, showEnemyMoves, noPauseAfterEnemyMoves, fastPieceSlide, instantAdvice, tutorialHelp, enterClosestCityScreen, 
                                        moveUnitsWithoutMouse, throneRoomGraphics, diplomacyScreenGraphics, animatedHeralds, civilopediaForAdvances, highCouncil, 
                                        wonderMovies, warnWhenCityGrowthHalted, showCityImprovementsBuilt, showNonCombatUnitsBuilt, showInvalidBuildInstructions, 
                                        announceCitiesInDisorder, announceOrderRestored, announceWeLoveKingDay, warnWhenFoodDangerouslyLow, warnWhenPollutionOccurs,
                                        warnChangProductWillCostShields, zoomToCityNotDefaultAction, cheatPenaltyWarning, grid);

            // Number of turns passed
            Data.TurnNumber = int.Parse(string.Concat(bytes[29].ToString("X"), bytes[28].ToString("X")), System.Globalization.NumberStyles.HexNumber);    //convert hex value 2 & 1 (in that order) together to int

            // Number of turns passed for game year calculation
            Data.TurnNumberForGameYear = int.Parse(string.Concat(bytes[31].ToString("X"), bytes[30].ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Which unit is selected at start of game (return -1 if no unit is selected (FFFFhex=65535dec))
            int _selectedIndex = int.Parse(string.Concat(bytes[35].ToString("X"), bytes[34].ToString("X")), System.Globalization.NumberStyles.HexNumber);
            Data.SelectedUnitIndex = (_selectedIndex == 65535) ? -1 : _selectedIndex;
            
            // Which human player is used
            Data.HumanPlayer = bytes[39];

            // Players map which is used
            Data.PlayersMapUsed = bytes[40];

            // Players civ number used
            Data.PlayersCivilizationNumberUsed = bytes[41];

            // Map revealed
            Data.MapRevealed = (bytes[43] == 1) ? true : false;

            // Difficulty level
            Data.DifficultyLevel = bytes[44];

            // Barbarian activity
            Data.BarbarianActivity = bytes[45];

            // Civs in play
            Data.CivsInPlay = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            string conv = Convert.ToString(bytes[46], 2).PadLeft(8, '0');
            for (int i = 0; i < 8; i++)
                if (conv[i] == '1')
                    Data.CivsInPlay[i] = 1;

            // Civs with human player playing (multiplayer)
            string humanPlayerPlayed = Convert.ToString(bytes[47], 2).PadLeft(8, '0');

            // Amount of pollution
            Data.PollutionAmount = bytes[50];

            // Global temp rising times occured
            Data.GlobalTempRiseOccured = bytes[51];
            
            //Number of turns of peace
            Data.NoOfTurnsOfPeace = bytes[56]; 

            // Number of units
            Data.NumberOfUnits = int.Parse(string.Concat(bytes[59].ToString("X"), bytes[58].ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Number of cities
            Data.NumberOfCities = int.Parse(string.Concat(bytes[61].ToString("X"), bytes[60].ToString("X")), System.Globalization.NumberStyles.HexNumber);

            #endregion
            #region Wonders
            //=========================
            //WONDERS
            //=========================
            int[] wonderCity = new int[28];         // city which has wonder
            bool[] wonderBuilt = new bool[28];      // has the wonder been built
            bool[] wonderDestroyed = new bool[28];  // has the wonder been destroyed
            for (int i = 0; i < 28; i++)
            {
                // City number with the wonder
                intVal1 = bytes[266 + 2 * i];
                intVal2 = bytes[266 + 2 * i + 1];
                wonderCity[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Determine if wonder is built/destroyed
                if (wonderCity[i] == 65535) //FFFF(hex)
                {
                    wonderBuilt[i] = false;
                }
                else if (wonderCity[i] == 65279)    //FEFF(hex)
                {
                    wonderDestroyed[i] = true;
                }
                else
                {
                    wonderBuilt[i] = true;
                    wonderDestroyed[i] = false;
                }
            }
            #endregion
            #region Civs
            //=========================
            //CIVS
            //=========================
            char[] asciich = new char[23];
            int[] civCityStyle = new int[8];
            string[] civLeaderName = new string[8];
            string[] civTribeName = new string[8];
            string[] civAdjective = new string[8];
            for (int i = 0; i < 7; i++) //for 7 civs, but NOT for barbarians (barbarians have i=0, so begin count at 1)
            {
                //City style
                civCityStyle[i + 1] = bytes[584 + 242 * i];

                //Leader names (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(bytes[584 + 2 + 242 * i + j]);
                civLeaderName[i + 1] = new string(asciich);
                civLeaderName[i + 1] = civLeaderName[i + 1].Replace("\0", string.Empty);  //remove null characters

                //Tribe name (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(bytes[584 + 2 + 23 + 242 * i + j]);
                civTribeName[i + 1] = new string(asciich);
                civTribeName[i + 1] = civTribeName[i + 1].Replace("\0", string.Empty);

                //Adjective (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(bytes[584 + 2 + 23 + 23 + 242 * i + j]);
                civAdjective[i + 1] = new string(asciich);
                civAdjective[i + 1] = civAdjective[i + 1].Replace("\0", string.Empty);

                //Leader titles (Anarchy, Despotism, ...)
                // .... TO-DO ....

            }
            //Manually add data for barbarians
            civCityStyle[0] = 0;
            civLeaderName[0] = "NULL";
            civTribeName[0] = "Barbarians";
            civAdjective[0] = "Barbarian";
            #endregion
            #region Tech & money
            //=========================
            //TECH & MONEY
            //=========================
            int[] rulerGender = new int[8];
            int[] civMoney = new int[8];
            int[] tribeNumber = new int[8];
            int[] civResearchProgress = new int[8];
            int[] civResearchingTech = new int[8];
            int[] civSciRate = new int[8];
            int[] civTaxRate = new int[8];
            int[] civGovernment = new int[8];
            int[] civReputation = new int[8];
            int[] civTechs = new int[89];
            //starting offset = 8E6(hex) = 2278(10), each block has 1427(10) bytes
            for (int i = 0; i < 8; i++) //for each civ
            {
                //Gender (0=male, 2=female)
                rulerGender[i] = bytes[2278 + 1428 * i + 1]; //2nd byte in tribe block

                //Money
                intVal1 = bytes[2278 + 1428 * i + 2];    //3rd byte in tribe block
                intVal2 = bytes[2278 + 1428 * i + 3];    //4th byte in tribe block
                civMoney[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Tribe number as per @Leaders table in RULES.TXT
                tribeNumber[i] = bytes[2278 + 1428 * i + 6];    //7th byte in tribe block

                //Research progress
                intVal1 = bytes[2278 + 1428 * i + 8];    //9th byte in tribe block
                intVal2 = bytes[2278 + 1428 * i + 9];    //10th byte in tribe block
                civResearchProgress[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Tech currently being researched
                civResearchingTech[i] = bytes[2278 + 1428 * i + 10]; //11th byte in tribe block (FF(hex) = no goal)

                //Science rate (%/10)
                civSciRate[i] = bytes[2278 + 1428 * i + 19]; //20th byte in tribe block

                //Tax rate (%/10)
                civTaxRate[i] = bytes[2278 + 1428 * i + 20]; //21st byte in tribe block

                //Government
                civGovernment[i] = bytes[2278 + 1428 * i + 21]; //22nd byte in tribe block (0=anarchy, ...)

                //Reputation
                civReputation[i] = bytes[2278 + 1428 * i + 30]; //31st byte in tribe block

                //Treaties
                // ..... TO-DO .....

                //Attitudes
                // ..... TO-DO .....

                //Technologies
                string civTechs1 = Convert.ToString(bytes[2278 + 1428 * i + 88], 2).PadLeft(8, '0');    //89th byte
                string civTechs2 = Convert.ToString(bytes[2278 + 1428 * i + 89], 2).PadLeft(8, '0');
                string civTechs3 = Convert.ToString(bytes[2278 + 1428 * i + 90], 2).PadLeft(8, '0');
                string civTechs4 = Convert.ToString(bytes[2278 + 1428 * i + 91], 2).PadLeft(8, '0');
                string civTechs5 = Convert.ToString(bytes[2278 + 1428 * i + 92], 2).PadLeft(8, '0');
                string civTechs6 = Convert.ToString(bytes[2278 + 1428 * i + 93], 2).PadLeft(8, '0');
                string civTechs7 = Convert.ToString(bytes[2278 + 1428 * i + 94], 2).PadLeft(8, '0');
                string civTechs8 = Convert.ToString(bytes[2278 + 1428 * i + 95], 2).PadLeft(8, '0');
                string civTechs9 = Convert.ToString(bytes[2278 + 1428 * i + 96], 2).PadLeft(8, '0');
                string civTechs10 = Convert.ToString(bytes[2278 + 1428 * i + 97], 2).PadLeft(8, '0');
                string civTechs11 = Convert.ToString(bytes[2278 + 1428 * i + 98], 2).PadLeft(8, '0');
                string civTechs12 = Convert.ToString(bytes[2278 + 1428 * i + 99], 2).PadLeft(8, '0');
                string civTechs13 = Convert.ToString(bytes[2278 + 1428 * i + 100], 2).PadLeft(8, '0');   //101st byte
                civTechs13 = civTechs13.Remove(civTechs13.Length - 4); //remove last 4 bits, they are not important
                //Put all techs into one large string, where bit0=1st tech, bit1=2nd tech, ..., bit99=100th tech
                //First reverse bit order in all strings
                civTechs1 = Reverse(civTechs1);
                civTechs2 = Reverse(civTechs2);
                civTechs3 = Reverse(civTechs3);
                civTechs4 = Reverse(civTechs4);
                civTechs5 = Reverse(civTechs5);
                civTechs6 = Reverse(civTechs6);
                civTechs7 = Reverse(civTechs7);
                civTechs8 = Reverse(civTechs8);
                civTechs9 = Reverse(civTechs9);
                civTechs10 = Reverse(civTechs10);
                civTechs11 = Reverse(civTechs11);
                civTechs12 = Reverse(civTechs12);
                civTechs13 = Reverse(civTechs13);
                //Merge all strings into a large string
                string civTechs_ = String.Concat(civTechs1, civTechs2, civTechs3, civTechs4, civTechs5, civTechs6, civTechs7, civTechs8, civTechs9, civTechs10, civTechs11, civTechs12, civTechs13);
                //True = tech researched, false = not researched
                for (int no = 0; no < 89; no++)
                    civTechs[no] = (civTechs_[no] == '1') ? 1 : 0;

                CreateCiv(i, Data.HumanPlayer, civCityStyle[i], civLeaderName[i], civTribeName[i], civAdjective[i], rulerGender[i], civMoney[i], 
                          tribeNumber[i], civResearchProgress[i], civResearchingTech[i], civSciRate[i], civTaxRate[i], civGovernment[i], 
                          civReputation[i], civTechs);
            }
            #endregion
            #region Map
            //=========================
            //MAP
            //=========================
            // Map header ofset
            int ofset;
            if (bytes[10] > 39) ofset = 13702; // FW and later (offset=3586hex)
            else ofset = 13432;             // Conflicts (offset=3478hex)

            // Map X dimension
            intVal1 = bytes[ofset + 0];
            intVal2 = bytes[ofset + 1];
            Data.MapXdim = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber) / 2; //map 150x120 is really 75x120

            // Map Y dimension
            intVal1 = bytes[ofset + 2];
            intVal2 = bytes[ofset + 3];
            Data.MapYdim = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Map area:
            intVal1 = bytes[ofset + 4];
            intVal2 = bytes[ofset + 5];
            Data.MapArea = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //// Flat Earth flag (info already given before!!)
            //intVal1 = bytes[ofset + 6];
            //intVal2 = bytes[ofset + 7];
            //flatEarth = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Map seed
            intVal1 = bytes[ofset + 8];
            intVal2 = bytes[ofset + 9];
            Data.MapSeed = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Locator map X dimension
            intVal1 = bytes[ofset + 10];
            intVal2 = bytes[ofset + 11];
            Data.MapLocatorXdim = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);  //TODO: what does this do?

            // Locator map Y dimension
            int intValue11 = bytes[ofset + 12];
            int intValue12 = bytes[ofset + 13];
            Data.MapLocatorYdim = int.Parse(string.Concat(intValue12.ToString("X"), intValue11.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Initialize Terrain array now that you know its size
            TerrainTile = new ITerrain[Data.MapXdim, Data.MapYdim];   //TODO: where to put this?

            // block 1 - terrain improvements (for individual civs)
            int ofsetB1 = ofset + 14; //offset for block 2 values
            //...........
            // block 2 - terrain type
            int ofsetB2 = ofsetB1 + 7 * Data.MapArea; //offset for block 2 values
            for (int i = 0; i < Data.MapArea; i++)
            {
                int x = i % Data.MapXdim;
                int y = i / Data.MapXdim;

                // Terrain type
                TerrainType type = TerrainType.Desert;  //only initial
                bool river = false;
                int terrain_type = bytes[ofsetB2 + i * 6 + 0];
                if (terrain_type == 0) { type = TerrainType.Desert; river = false; }   //0dec=0hex
                if (terrain_type == 128) { type = TerrainType.Desert; river = true; }   //128dec=80hex
                if (terrain_type == 1) { type = TerrainType.Plains; river = false; }   //1dec=1hex
                if (terrain_type == 129) { type = TerrainType.Plains; river = true; }   //129dec=81hex
                if (terrain_type == 2) { type = TerrainType.Grassland; river = false; }   //2dec=2hex
                if (terrain_type == 130) { type = TerrainType.Grassland; river = true; }   //130dec=82hex
                if (terrain_type == 3) { type = TerrainType.Forest; river = false; }   //3dec=3hex
                if (terrain_type == 131) { type = TerrainType.Forest; river = true; }   //131dec=83hex
                if (terrain_type == 4) { type = TerrainType.Hills; river = false; }   //4dec=4hex
                if (terrain_type == 132) { type = TerrainType.Hills; river = true; }   //132dec=84hex
                if (terrain_type == 5) { type = TerrainType.Mountains; river = false; }   //5dec=5hex
                if (terrain_type == 133) { type = TerrainType.Mountains; river = true; }   //133dec=85hex
                if (terrain_type == 6) { type = TerrainType.Tundra; river = false; }   //6dec=6hex
                if (terrain_type == 134) { type = TerrainType.Tundra; river = true; }   //134dec=86hex
                if (terrain_type == 7) { type = TerrainType.Glacier; river = false; }   //7dec=7hex
                if (terrain_type == 135) { type = TerrainType.Glacier; river = true; }   //135dec=87hex
                if (terrain_type == 8) { type = TerrainType.Swamp; river = false; }   //8dec=8hex
                if (terrain_type == 136) { type = TerrainType.Swamp; river = true; }   //136dec=88hex
                if (terrain_type == 9) { type = TerrainType.Jungle; river = false; }   //9dec=9hex
                if (terrain_type == 137) { type = TerrainType.Jungle; river = true; }   //137dec=89hex
                if (terrain_type == 10) { type = TerrainType.Ocean; river = false; }   //10dec=Ahex
                if (terrain_type == 74) { type = TerrainType.Ocean; river = false; }   //74dec=4Ahex
                //determine if resources are present
                bool resource = false;
                //!!! NOT WORKING PROPERLY !!!
                //bin = Convert.ToString(dataArray[ofsetB2 + i * 6 + 0], 2).PadLeft(8, '0');
                //if (bin[1] == '1') { resource = true; }

                // Tile improvements (for all civs! In block 1 it's for indivudual civs)
                //int tile_improv = bytes[ofsetB2 + i * 6 + 1];
                //bin = Convert.ToString(tile_improv, 2).PadLeft(8, '0');

                bool unit_present   = GetBit(bytes[ofsetB2 + i * 6 + 1], 7);
                bool city_present   = GetBit(bytes[ofsetB2 + i * 6 + 1], 6);
                bool irrigation     = GetBit(bytes[ofsetB2 + i * 6 + 1], 5);
                bool mining         = GetBit(bytes[ofsetB2 + i * 6 + 1], 4);
                bool road           = GetBit(bytes[ofsetB2 + i * 6 + 1], 3);
                bool railroad       = GetBit(bytes[ofsetB2 + i * 6 + 1], 2) && GetBit(bytes[ofsetB2 + i * 6 + 1], 3);
                bool fortress       = GetBit(bytes[ofsetB2 + i * 6 + 1], 1);
                bool pollution      = GetBit(bytes[ofsetB2 + i * 6 + 1], 0);
                bool farmland       = GetBit(bytes[ofsetB2 + i * 6 + 1], 4) && GetBit(bytes[ofsetB2 + i * 6 + 1], 5);
                bool airbase        = GetBit(bytes[ofsetB2 + i * 6 + 1], 1) && GetBit(bytes[ofsetB2 + i * 6 + 1], 6);

                int intValueB23 = bytes[ofsetB2 + i * 6 + 2];       //City radius (TO-DO)
                
                int terrain_island = bytes[ofsetB2 + i * 6 + 3];    //Island counter
                
                // Visibility of squares for all civs (0...red (barbarian), 1...white, 2...green, etc.)
                int intValueB25 = bytes[ofsetB2 + i * 6 + 4];
                bool[] visibility = new bool[8];
                bin = Convert.ToString(intValueB25, 2).PadLeft(8, '0');
                for (int civ = 0; civ < 8; civ++) 
                    visibility[civ] = (bin[7 - civ] == '1') ? true : false;

                int intValueB26 = bytes[ofsetB2 + i * 6 + 5];       //?

                //string hexValue = intValueB26.ToString("X");

                // SAV file doesn't tell where special resources are, so you have to set this yourself
                int specialtype = ReturnSpecial(x, y, type, Data.MapXdim, Data.MapYdim);

                CreateTerrain(x, y, type, specialtype, resource, river, terrain_island, unit_present, city_present, irrigation, mining, road, railroad, fortress, pollution, farmland, airbase, visibility, bin);

            }
            //block 3 - locator map
            int ofsetB3 = ofsetB2 + 6 * Data.MapArea; //offset for block 2 values
                                                      //...............
            #endregion
            #region Units
            //=========================
            //UNIT INFO
            //=========================
            int ofsetU = ofsetB3 + 2 * Data.MapLocatorXdim * Data.MapLocatorYdim + 1024;

            // Determine byte length of units
            int multipl;
            if (bytes[10] <= 40)        multipl = 26;   // FW or CiC
            else if (bytes[10] == 44)   multipl = 32;   // MGE
            else                        multipl = 40;   // ToT

            for (int i = 0; i < Data.NumberOfUnits; i++)
            {
                // Unit X location
                intVal1 = bytes[ofsetU + multipl * i + 0];
                intVal2 = bytes[ofsetU + multipl * i + 1];
                int unitXlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                bool unit_dead = GetBit(bytes[ofsetU + multipl * i + 1], 0);    // Unit is inactive (dead) if the value of X-Y is negative (1st bit = 1)

                // Unit Y location
                intVal1 = bytes[ofsetU + multipl * i + 2];
                intVal2 = bytes[ofsetU + multipl * i + 3];
                int unitYlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
                                
                bool unitFirstMove = GetBit(bytes[ofsetU + multipl * i + 4], 1);    // If this is the unit's first move

                bool unitGreyStarShield = GetBit(bytes[ofsetU + multipl * i + 5], 0);   // Grey star to the shield
                bool unitVeteranStatus = GetBit(bytes[ofsetU + multipl * i + 5], 2);    // Veteran status
                int unitType = bytes[ofsetU + multipl * i + 6];                         // Unit type
                int unitCiv = bytes[ofsetU + multipl * i + 7];                          // Unit civ, 00 = barbarians                
                int unitMovePointsLost = bytes[ofsetU + multipl * i + 8];               // Unit move points expended                
                int unitHitpointsLost = bytes[ofsetU + multipl * i + 10];               // Unit hitpoints lost
                int unitLastMove = bytes[ofsetU + multipl * i + 11];                    // Unit previous move (00=up-right, 01=right, ..., 07=up, FF=no movement)                
                int unitCaravanCommodity = bytes[ofsetU + multipl * i + 13];            // Unit caravan commodity                
                int unitOrders = bytes[ofsetU + multipl * i + 15];                      // Unit orders                
                int unitHomeCity = bytes[ofsetU + multipl * i + 16];                    // Unit home city

                // Unit go-to X
                intVal1 = bytes[ofsetU + multipl * i + 18];
                intVal2 = bytes[ofsetU + multipl * i + 19];
                int unitGoToX = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Unit go-to Y
                intVal1 = bytes[ofsetU + multipl * i + 20];
                intVal2 = bytes[ofsetU + multipl * i + 21];
                int unitGoToY = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Unit link to other units on top of it
                intVal1 = bytes[ofsetU + multipl * i + 22];
                intVal2 = bytes[ofsetU + multipl * i + 23];
                int unitLinkOtherUnitsOnTop = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Unit link to other units under it
                intVal1 = bytes[ofsetU + multipl * i + 24];
                intVal2 = bytes[ofsetU + multipl * i + 25];
                int unitLinkOtherUnitsUnder = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                CreateUnit((UnitType)unitType, unitXlocation, unitYlocation, unit_dead, unitFirstMove, unitGreyStarShield, unitVeteranStatus, unitCiv, unitMovePointsLost, 
                           unitHitpointsLost, unitLastMove, unitCaravanCommodity, (OrderType)unitOrders, unitHomeCity, unitGoToX, unitGoToY, unitLinkOtherUnitsOnTop, unitLinkOtherUnitsUnder);            }
            #endregion
            #region Cities
            //=========================
            //CITIES
            //=========================
            int ofsetC = ofsetU + multipl * Data.NumberOfUnits;

            if (bytes[10] <= 40)        multipl = 84;   // FW or CiC
            else if (bytes[10] == 44)   multipl = 88;   // MGE
            else                        multipl = 92;   // ToT

            char[] asciichar = new char[15];
            for (int i = 0; i < Data.NumberOfCities; i++)
            {
                // City X location
                intVal1 = bytes[ofsetC + multipl * i + 0];
                intVal2 = bytes[ofsetC + multipl * i + 1];
                int cityXlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // City Y location
                intVal1 = bytes[ofsetC + multipl * i + 2];
                intVal2 = bytes[ofsetC + multipl * i + 3];
                int cityYlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                bool cityCanBuildCoastal        = GetBit(bytes[ofsetC + multipl * i + 4], 0);    // Can build coastal improvements
                bool cityAutobuildMilitaryRule  = GetBit(bytes[ofsetC + multipl * i + 4], 3);    // Auto build under military rule
                bool cityStolenTech             = GetBit(bytes[ofsetC + multipl * i + 4], 4);    // Stolen tech
                bool cityImprovementSold        = GetBit(bytes[ofsetC + multipl * i + 4], 5);    // Improvement sold
                bool cityWeLoveKingDay          = GetBit(bytes[ofsetC + multipl * i + 4], 6);    // We love king day
                bool cityCivilDisorder          = GetBit(bytes[ofsetC + multipl * i + 4], 7);    // Civil disorder
                bool cityCanBuildShips          = GetBit(bytes[ofsetC + multipl * i + 6], 2);    // Can build ships
                bool cityObjectivex3            = GetBit(bytes[ofsetC + multipl * i + 7], 3);    // Objective x3
                bool cityObjectivex1            = GetBit(bytes[ofsetC + multipl * i + 7], 5);    // Objective x1

                int cityOwner = bytes[ofsetC + multipl * i + 8];        // Owner
                int citySize = bytes[ofsetC + multipl * i + 9];         // Size
                int cityWhoBuiltIt = bytes[ofsetC + multipl * i + 10];  // Who built it

                // Production squares
                //???????????????????

                // Specialists
                //??????????????????

                // Food in storage
                intVal1 = bytes[ofsetC + multipl * i + 26];
                intVal2 = bytes[ofsetC + multipl * i + 27];
                int cityFoodInStorage = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Shield progress
                intVal1 = bytes[ofsetC + multipl * i + 28];
                intVal2 = bytes[ofsetC + multipl * i + 29];
                int cityShieldsProgress = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Net trade
                intVal1 = bytes[ofsetC + multipl * i + 30];
                intVal2 = bytes[ofsetC + multipl * i + 31];
                int cityNetTrade = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Name        
                for (int j = 0; j < 15; j++) 
                    asciichar[j] = Convert.ToChar(bytes[ofsetC + multipl * i + j + 32]);
                string cityName = new string(asciichar);
                cityName = cityName.Replace("\0", string.Empty);

                // Distribution of workers on map in city view
                string cityWorkDistr1 = Convert.ToString(bytes[ofsetC + multipl * i + 48], 2).PadLeft(8, '0');  // inner circle (starting from N, going in counter-clokwise direction)                
                string cityWorkDistr2 = Convert.ToString(bytes[ofsetC + multipl * i + 49], 2).PadLeft(8, '0');  // on 8 of the outer circle    
                string cityWorkDistr3 = Convert.ToString(bytes[ofsetC + multipl * i + 50], 2).PadLeft(5, '0');  // on 4 of the outer circle
                string _cityDistributionWorkers = string.Format("{0}{1}{2}", cityWorkDistr3, cityWorkDistr2, cityWorkDistr1);
                int[] cityDistributionWorkers = new int[21];
                for (int distNo = 0; distNo < 21; distNo++)
                    cityDistributionWorkers[distNo] = (_cityDistributionWorkers[distNo] == '1') ? 1 : 0;

                int cityNoOfSpecialistsx4 = bytes[ofsetC + multipl * i + 51];   // Number of specialists x4

                // Improvements
                string cityImprovements1 = Convert.ToString(bytes[ofsetC + multipl * i + 52], 2).PadLeft(8, '0');   // bit6=palace (1st improvement), bit7=not important
                cityImprovements1 = cityImprovements1.Remove(cityImprovements1.Length - 1);                         // remove last bit, it is not important
                string cityImprovements2 = Convert.ToString(bytes[ofsetC + multipl * i + 53], 2).PadLeft(8, '0');
                string cityImprovements3 = Convert.ToString(bytes[ofsetC + multipl * i + 54], 2).PadLeft(8, '0');
                string cityImprovements4 = Convert.ToString(bytes[ofsetC + multipl * i + 55], 2).PadLeft(8, '0');
                string cityImprovements5 = Convert.ToString(bytes[ofsetC + multipl * i + 56], 2).PadLeft(8, '0');   // bit0-bit4=not important, bit5=port facility (last improvement)
                // Put all improvements into one large string, where bit0=palace, bit1=barracks, ..., bit33=port facility
                // First reverse bit order in all strings
                cityImprovements1 = Reverse(cityImprovements1);
                cityImprovements2 = Reverse(cityImprovements2);
                cityImprovements3 = Reverse(cityImprovements3);
                cityImprovements4 = Reverse(cityImprovements4);
                cityImprovements5 = Reverse(cityImprovements5);
                cityImprovements5 = cityImprovements5.Remove(cityImprovements5.Length - 5); // remove last 5 bits, they are not important
                // Merge all strings into a large string
                string cityImprovements_ = string.Format("{0}{1}{2}{3}{4}", cityImprovements1, cityImprovements2, cityImprovements3, cityImprovements4, cityImprovements5);
                // Convert string array to bool array
                bool[] cityImprovements = new bool[34];
                for (int impNo = 0; impNo < 34; impNo++)
                    cityImprovements[impNo] = (cityImprovements_[impNo] == '1') ? true : false;

                // Item in production
                // 0(dec)/0(hex) ... 61(dec)/3D(hex) are units, improvements are inversed (FF(hex)=1st, FE(hex)=2nd, ...)
                // convert this notation of improvements, so that 62(dec) is 1st improvement, 63(dec) is 2nd, ...
                int cityItemInProduction = bytes[ofsetC + multipl * i + 57];
                if (cityItemInProduction > 70)  //if it is improvement
                    cityItemInProduction = 255 - cityItemInProduction + 62; // 62 because 0...61 are units
                                
                int cityActiveTradeRoutes = bytes[ofsetC + multipl * i + 58];   // No of active trade routes

                // 1st, 2nd, 3rd trade commodities supplied
                int[] cityCommoditySupplied = new int[] { bytes[ofsetC + multipl * i + 59], bytes[ofsetC + multipl * i + 60], bytes[ofsetC + multipl * i + 61] };

                // 1st, 2nd, 3rd trade commodities demanded
                int[] cityCommodityDemanded = new int[] { bytes[ofsetC + multipl * i + 62], bytes[ofsetC + multipl * i + 63], bytes[ofsetC + multipl * i + 64] };

                // 1st, 2nd, 3rd trade commodities in route
                int[] cityCommodityInRoute = new int[] { bytes[ofsetC + multipl * i + 65], bytes[ofsetC + multipl * i + 66], bytes[ofsetC + multipl * i + 67] };

                // 1st, 2nd, 3rd trade route partner city number
                int[] cityTradeRoutePartnerCity = new int[] { bytes[ofsetC + multipl * i + 68], bytes[ofsetC + multipl * i + 69], bytes[ofsetC + multipl * i + 70] };

                // Science
                intVal1 = bytes[ofsetC + multipl * i + 74];
                intVal2 = bytes[ofsetC + multipl * i + 75];
                int cityScience = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Tax
                intVal1 = bytes[ofsetC + multipl * i + 76];
                intVal2 = bytes[ofsetC + multipl * i + 77];
                int cityTax = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // No of trade icons
                intVal1 = bytes[ofsetC + multipl * i + 78];
                intVal2 = bytes[ofsetC + multipl * i + 79];
                int cityNoOfTradeIcons = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
                                
                int cityFoodProduction = bytes[ofsetC + multipl * i + 80];  // Total food production
                                
                int cityShieldProduction = bytes[ofsetC + multipl * i + 81];    // Total shield production
                                
                int cityHappyCitizens = bytes[ofsetC + multipl * i + 82];   // No of happy citizens
                                
                int cityUnhappyCitizens = bytes[ofsetC + multipl * i + 83]; // No of unhappy citizens

                // Sequence number of the city
                //...

                // Check if wonder is in city (28 possible wonders)
                bool[] cityWonders = new bool[28];
                for (int wndr = 0; wndr < 28; wndr++)
                    cityWonders[wndr] = (wonderCity[wndr] == i) ? true : false;

                CreateCity(cityXlocation, cityYlocation, cityCanBuildCoastal, cityAutobuildMilitaryRule, cityStolenTech, cityImprovementSold, cityWeLoveKingDay, cityCivilDisorder, 
                           cityCanBuildShips, cityObjectivex3, cityObjectivex1, cityOwner, citySize, cityWhoBuiltIt, cityFoodInStorage, cityShieldsProgress, cityNetTrade, cityName,
                           cityDistributionWorkers, cityNoOfSpecialistsx4, cityImprovements, cityItemInProduction, cityActiveTradeRoutes, cityCommoditySupplied, cityCommodityDemanded,
                           cityCommodityInRoute, cityTradeRoutePartnerCity, cityScience, cityTax, cityNoOfTradeIcons, cityFoodProduction, cityShieldProduction, cityHappyCitizens, 
                           cityUnhappyCitizens, cityWonders);
            }
            #endregion
            #region Other
            //=========================
            //OTHER
            //=========================
            int ofsetO = ofsetC + multipl * Data.NumberOfCities;

            // Active cursor XY position
            intVal1 = bytes[ofsetO + 63];
            intVal2 = bytes[ofsetO + 64];
            intVal3 = bytes[ofsetO + 65];
            intVal4 = bytes[ofsetO + 66];
            Data.ActiveXY = new int[] { int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber), int.Parse(string.Concat(intVal4.ToString("X"), intVal3.ToString("X")), System.Globalization.NumberStyles.HexNumber) };

            // Clicked tile X position
            intVal1 = bytes[ofsetO + 1425];
            intVal2 = bytes[ofsetO + 1426];
            intVal3 = bytes[ofsetO + 1427];
            intVal4 = bytes[ofsetO + 1428];
            Data.ClickedXY = new int[] { int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber), int.Parse(string.Concat(intVal4.ToString("X"), intVal3.ToString("X")), System.Globalization.NumberStyles.HexNumber) };

            #endregion

        }

        private static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        private static string Reverse(string s)   //Reverse a string
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static int ReturnSpecial(int col, int row, TerrainType type, int mapXdim, int mapYdim)
        {
            int special = 0;

            //Special Resources (only grassland)
            //Grassland shield is present in pattern 1100110011... in 1st line, in 3rd line shifted by 1 to right (01100110011...), in 5th line shifted by 1 to right (001100110011...) etc.
            //In 2nd line 00110011..., in 4th line shifted right by 1 (1001100...), in 6th line shifted by 1 to right (11001100...) etc.
            //For grassland special = 0 (no shield), special = 1 (shield).
            if (type == TerrainType.Grassland)
            {
                if (row % 2 == 0) //odd lines
                    special = ((col + 4 - (row % 8) / 2) % 4 == 0 || (col + 4 - (row % 8) / 2) % 4 == 1) ? 1 : 0;
                else    //even lines
                    special = ((col + 4 - (row % 8) / 2) % 4 == 2 || (col + 4 - (row % 8) / 2) % 4 == 3) ? 1 : 0;

                //if (Game.TerrainTile[col, row].Special == 1) { graphics.DrawImage(Images.Shield, 0, 0); }
            }

            //Special Resources (not grassland)
            //(not yet 100% sure how this works)
            //No matter which terrain tile it is (except grassland). 2 special resources R1 & R2 (e.g. palm & oil for desert). R1 is (in x-direction) always followed by R2, then R1, R2, R1, ... First 2 (j=1,3) are special as they do not belong to other blocks described below. Next block has 7 y-coordinates (j=8,10,...20), next block has 6 (j=25,27,...35), next block 7 (j=40,42,...52), next block 6 (j=57,59,...67), ... Blocks are always 5 tiles appart in y-direction. In x-direction for j=1 the resources are 3/5/3/5 etc. tiles appart. For j=3 they are 8 tiles appart in x-direction. For the next block they are 8-8-8-(3/5/3/5)-8-8-8 tiles appart in x-direction. For the next block they are 8-(3/5/3/5)-8-8-(3/5/3/5)-8 tiles appart. Then these 4 blocks start repeating again. Starting points: For j=1 it is (0,1), for j=3 it is (6,3). The starting (x) points for the next block are x=3,6,4,2,5,3,6. For next block they are x=2,0,3,1,4,2. For the next block they are x=7,2,0,3,1,7,2. For next block they are x=6,1,7,5,3,6. These 4 patterns then start repeating again. So the next block has again pattern 3,6,4,2,5,3,6, the next block has x=2,0,3,1,4,2, etc.
            //For these tiles special=0 (no special, e.g. only desert), special=1 (special #1, e.g. oasis for desert), special=2 (special #2, e.g. oil for desert)
            int[] startx_B1 = new int[] { 3, 6, 4, 2, 5, 3, 6 };  //starting x-points for 4 blocks
            int[] startx_B2 = new int[] { 2, 0, 3, 1, 4, 2 };
            int[] startx_B3 = new int[] { 7, 2, 0, 3, 1, 7, 2 };
            int[] startx_B4 = new int[] { 6, 1, 7, 5, 3, 6 };
            if (type != TerrainType.Grassland)
            {
                special = 0;    //for start we presume this 
                bool found = false;

                if (row == 1) //prva posebna tocka
                {
                    int novi_i = 0; //zacetna tocka pri j=1 (0,1)
                    while (novi_i < mapXdim)  //keep jumping in x-direction till map end
                    {
                        if (novi_i < mapXdim && col == novi_i) { special = 2; break; }   //tocke (3,1), (11,1), (19,1), ...
                        novi_i += 3;
                        if (novi_i < mapXdim && col == novi_i) { special = 1; break; }   //tocke (8,1), (16,1), (24,1), ...
                        novi_i += 5;
                    }

                }
                else if (row == 3)    //druga posebna tocka
                {
                    int novi_i = 6; //zacetna tocka pri j=3 je (6,3)
                    while (novi_i < mapXdim)
                    {
                        if (novi_i < mapXdim && col == novi_i) { special = 1; break; }
                        novi_i += 8;
                        if (novi_i < mapXdim && col == novi_i) { special = 2; break; }
                        novi_i += 8;
                    }

                }
                else
                {
                    int novi_j = 3;
                    while (novi_j < mapYdim)  //skakanje za 4 bloke naprej
                    {
                        if (found) break;

                        //BLOCK 1
                        int counter = 0;
                        novi_j += 5;   //jump to block beginning
                        while (novi_j < mapYdim && counter < 7)  //7 jumps in y-direction
                        {
                            if (found) break;

                            if (row == novi_j)    //correct y-loc found, now start looking for x
                            {
                                int novi_i = startx_B1[counter];
                                //set which resources will be and jumps
                                int res1, res2;
                                int skok_x1, skok_x2;
                                if (counter == 3)
                                {
                                    skok_x1 = 5;
                                    skok_x2 = 3;
                                    res1 = 2;
                                    res2 = 1;
                                }
                                else if (counter == 0 || counter == 1 || counter == 4)
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 2;
                                    res2 = 2;
                                }
                                else
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 1;
                                    res2 = 1;
                                }

                                while (novi_i < mapXdim)
                                {
                                    if (novi_i < mapXdim && col == novi_i) { special = res1; found = true; break; }
                                    novi_i += skok_x1;
                                    if (novi_i < mapXdim && col == novi_i) { special = res2; found = true; break; }
                                    novi_i += skok_x2;

                                    if (found) break;
                                }
                                break;   //terminate search
                            }
                            novi_j += 2;
                            counter += 1;
                        }
                        if (found) break;

                        //BLOCK 2
                        counter = 0;
                        novi_j += 5;   //jump to block beginning
                        while (novi_j < mapYdim && counter < 6)  //6 jumps in y-direction
                        {
                            if (found) break;

                            if (row == novi_j)    //correct y-loc found, now start looking for x
                            {
                                int novi_i = startx_B2[counter];
                                //set which resources will be and jumps
                                int res1, res2;
                                int skok_x1, skok_x2;
                                if (counter == 1)   //1st jump
                                {
                                    skok_x1 = 5;
                                    skok_x2 = 3;
                                    res1 = 1;
                                    res2 = 2;
                                }
                                else if (counter == 4)  //4th jump
                                {
                                    skok_x1 = 3;
                                    skok_x2 = 5;
                                    res1 = 2;
                                    res2 = 1;
                                }
                                else if (counter == 0 || counter == 3)
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 2;
                                    res2 = 2;
                                }
                                else
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 1;
                                    res2 = 1;
                                }

                                while (novi_i < mapXdim)
                                {
                                    if (novi_i < mapXdim && col == novi_i) { special = res1; found = true; break; }
                                    novi_i += skok_x1;
                                    if (novi_i < mapXdim && col == novi_i) { special = res2; found = true; break; }
                                    novi_i += skok_x2;

                                    if (found) break;
                                }
                                break;   //terminate search
                            }
                            novi_j += 2;
                            counter += 1;
                        }
                        if (found) break;

                        //BLOCK 3
                        counter = 0;
                        novi_j += 5;   //jump to block beginning
                        while (novi_j < mapYdim && counter < 7)  //7 jumps in y-direction
                        {
                            if (found) break;

                            if (row == novi_j)    //correct y-loc found, now start looking for x
                            {
                                int novi_i = startx_B3[counter];
                                //set which resources will be and jumps
                                int res1, res2;
                                int skok_x1, skok_x2;
                                if (counter == 3)   //3rd jump
                                {
                                    skok_x1 = 3;
                                    skok_x2 = 5;
                                    res1 = 1;
                                    res2 = 2;
                                }
                                else if (counter == 0 || counter == 1 || counter == 4)
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 2;
                                    res2 = 2;
                                }
                                else
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 1;
                                    res2 = 1;
                                }

                                while (novi_i < mapXdim)
                                {
                                    if (novi_i < mapXdim && col == novi_i) { special = res1; found = true; break; }
                                    novi_i += skok_x1;
                                    if (novi_i < mapXdim && col == novi_i) { special = res2; found = true; break; }
                                    novi_i += skok_x2;

                                    if (found) break;
                                }
                                break;   //terminate search
                            }
                            novi_j += 2;
                            counter += 1;
                        }
                        if (found) break;

                        //BLOCK 4
                        counter = 0;
                        novi_j += 5;   //jump to block beginning
                        while (novi_j < mapYdim && counter < 6)  //6 jumps in y-direction
                        {
                            if (found) break;

                            if (row == novi_j)    //correct y-loc found, now start looking for x
                            {
                                int novi_i = startx_B4[counter];
                                //set which resources will be and jumps
                                int res1, res2;
                                int skok_x1, skok_x2;
                                if (counter == 1 || counter == 4)   //1st & 3rd jump
                                {
                                    skok_x1 = 3;
                                    skok_x2 = 5;
                                    res1 = 2;
                                    res2 = 1;
                                }
                                else if (counter == 0 || counter == 3)
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 2;
                                    res2 = 2;
                                }
                                else
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 1;
                                    res2 = 1;
                                }

                                while (novi_i < mapXdim)
                                {
                                    if (novi_i < mapXdim && col == novi_i) { special = res1; found = true; break; }
                                    novi_i += skok_x1;
                                    if (novi_i < mapXdim && col == novi_i) { special = res2; found = true; break; }
                                    novi_i += skok_x2;

                                    if (found) break;
                                }
                                break;   //terminate search
                            }
                            novi_j += 2;
                            counter += 1;
                        }
                        if (found) break;

                    }
                }

            }

            return special;
        }

    }
}
