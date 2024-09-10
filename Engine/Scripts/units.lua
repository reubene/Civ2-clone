---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by revans.
--- DateTime: 24/07/2024 8:09am
---
--
--0 Settlers,     Exp, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, nil, 000000000000000
--1 Engineers,    nil, 0,  2.,0,  0a,2d,  2h,1f,  4,0,  5, Exp, 000000000000000
--2 Warriors,     Feu, 0,  1.,0,  1a,1d,  1h,1f,  1,0,  1, nil, 000000000000000
--3 Phalanx,      Feu, 0,  1.,0,  1a,2d,  1h,1f,  2,0,  1, Bro, 000000000000000
--4 Archers,      Gun, 0,  1.,0,  3a,2d,  1h,1f,  3,0,  1, War, 000000000000000
--5 Legion,       Gun, 0,  1.,0,  4a,2d,  1h,1f,  4,0,  1, Iro, 000000000000000
--6 Pikemen,      Gun, 0,  1.,0,  1a,2d,  1h,1f,  2,0,  1, Feu, 000010000000000
--7 Musketeers,   Csc, 0,  1.,0,  3a,3d,  2h,1f,  3,0,  1, Gun, 000000000000000
--8 Fanatics,     nil, 0,  1.,0,  4a,4d,  2h,1f,  2,0,  1, Fun, 000100000000000
--9 Partisans,    nil, 0,  1.,0,  4a,4d,  2h,1f,  5,0,  1, Gue, 000001000000010
civ.getUnitType(9).Effects.Add(civ.core.Effects.Partisan, 1)
--10 Alpine Troops,nil, 0,  1.,0,  5a,5d,  2h,1f,  5,0,  1, Tac, 000001000000000
--Riflemen,     nil, 0,  1.,0,  5a,4d,  2h,1f,  4,0,  1, Csc, 000000000000000
--Marines,      nil, 0,  1.,0,  8a,5d,  2h,1f,  6,0,  0, Amp, 000000000000100
--Paratroopers, nil, 0,  1.,0,  6a,4d,  2h,1f,  6,0,  1, CA,  000000100000000
--Mech. Inf.,   nil, 0,  3.,0,  6a,6d,  3h,1f,  5,0,  1, Lab, 000000000000000
--15 Horsemen,     Chi, 0,  2.,0,  2a,1d,  1h,1f,  2,0,  0, Hor, 000000000000000
--Chariot,      PT,  0,  2.,0,  3a,1d,  1h,1f,  3,0,  0, Whe, 000000000000000
--Elephant,     MT,  0,  2.,0,  4a,1d,  1h,1f,  4,0,  0, PT,  000000000000000
--Crusaders,    Ldr, 0,  2.,0,  5a,1d,  1h,1f,  4,0,  0, MT,  000000000000000
--Knights,      Ldr, 0,  2.,0,  4a,2d,  1h,1f,  4,0,  0, Chi, 000000000000000
--20 Dragoons,     Tac, 0,  2.,0,  5a,2d,  2h,1f,  5,0,  0, Ldr, 000000000000000
--Cavalry,      Mob, 0,  2.,0,  8a,3d,  2h,1f,  6,0,  0, Tac, 000000000000000
--Armor,        nil, 0,  3.,0, 10a,5d,  3h,1f,  8,0,  0, Mob, 000000000000000
--Catapult,     Met, 0,  1.,0,  6a,1d,  1h,1f,  4,0,  0, Mat, 000000000000000
--Cannon,       Too, 0,  1.,0,  8a,1d,  2h,1f,  4,0,  0, Met, 000000000000000
--25 Artillery,    Rob, 0,  1.,0, 10a,1d,  2h,2f,  5,0,  0, Too, 000000000000000
--Howitzer,     nil, 0,  2.,0, 12a,2d,  3h,2f,  7,0,  0, Rob, 000000001000000
--Fighter,      Sth, 1, 10.,1,  4a,3d,  2h,2f,  6,0,  3, Fli, 000000000010001
--Bomber,       Sth, 1,  8.,2, 12a,1d,  2h,2f, 12,0,  0, AFl, 000000000000001
--Helicopter,   nil, 1,  6.,0, 10a,3d,  2h,2f, 10,0,  0, CA,  100000000000001
--30 Stlth Ftr.,   nil, 1, 14.,1,  8a,4d,  2h,2f,  8,0,  3, Sth, 000000000010001
--Stlth Bmbr.,  nil, 1, 12.,2, 14a,5d,  2h,2f, 16,0,  0, Sth, 000000000000001
--Trireme,      Nav, 2,  3.,0,  1a,1d,  1h,1f,  4,2,  4, Map, 000000000100000
--Caravel,      Mag, 2,  3.,0,  2a,1d,  1h,1f,  4,3,  4, Nav, 000000000000000
--Galleon,      Ind, 2,  4.,0,  0a,2d,  2h,1f,  4,4,  4, Mag, 000000000000000
--35 Frigate,      E1,  2,  4.,0,  4a,2d,  2h,1f,  5,2,  2, Mag, 000000000000000
--Ironclad,     E1,  2,  4.,0,  4a,4d,  3h,1f,  6,0,  2, SE,  000000000000000
--Destroyer,    nil, 2,  6.,0,  4a,4d,  3h,1f,  6,0,  2, E1,  100000000000001
--Cruiser,      Roc, 2,  5.,0,  6a,6d,  3h,2f,  8,0,  2, Stl, 100000000000001
--AEGIS Cruiser,nil, 2,  5.,0,  8a,8d,  3h,2f, 10,0,  2, Roc, 110000000000001
--40 Battleship,   nil, 2,  4.,0, 12a,12d, 4h,2f, 16,0,  2, Aut, 000000000000001
--Submarine,    nil, 2,  3.,0, 10a,2d,  3h,2f,  6,0,  2, Cmb, 000000000001001
--Carrier,      nil, 2,  5.,0,  1a,9d,  4h,2f, 16,0,  2, AFl, 000000010000001
--Transport,    nil, 2,  5.,0,  0a,3d,  3h,1f,  5,8,  4, Ind, 000000000000000
--Cruise Msl.,  nil, 1, 12.,1, 18a,0d,  1h,3f,  6,0,  0, Roc, 001000000000000
civ.getUnitType(44).Effects.Add(civ.core.Effects.SdiVulnerable, 1)
--45 Nuclear Msl., nil, 1, 16.,1, 99a,0d,  1h,1f, 16,0,  0, Roc, 001000000000000
--Diplomat,     Esp, 0,  2.,0,  0a,0d,  1h,1f,  3,0,  6, Wri, 000000000000010
--Spy,          nil, 0,  3.,0,  0a,0d,  1h,1f,  3,0,  6, Esp, 000000000000011
--Caravan,      Cor, 0,  1.,0,  0a,1d,  1h,1f,  5,0,  7, Tra, 000000000000010
--Freight,      nil, 0,  2.,0,  0a,1d,  1h,1f,  5,0,  7, Cor, 000000000000010
--50 Explorer,     Gue, 0,  1.,0,  0a,1d,  1h,1f,  3,0,  0, Sea, 000001000000010
--Extra Land,   nil, 0,  1.,0,  1a,1d,  1h,1f,  5,0,  0, no,  000000000000000
--Extra Ship,   nil, 2,  4.,0,  4a,2d,  2h,1f,  5,1,  2, no,  000000000000000
--Extra Air,    nil, 1,  8.,4,  8a,8d,  2h,2f, 10,0,  0, no,  000000000000000
--Test Unit 1,  nil, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, no,  000000000000000
--55 Test Unit 2,  nil, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, no,  000000000000000
--Test Unit 3,  nil, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, no,  000000000000000
--Test Unit 4,  nil, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, no,  000000000000000
--Test Unit 5,  nil, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, no,  000000000000000
--Test Unit 6,  nil, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, no,  000000000000000
--60 Test Unit 7,  nil, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, no,  000000000000000
--Test Unit 8,  nil, 0,  1.,0,  0a,1d,  2h,1f,  4,0,  5, no,  000000000000000