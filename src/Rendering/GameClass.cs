﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.Tools;
using Rampastring.XNAUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSMapEditor.CCEngine;
using TSMapEditor.Models;

namespace TSMapEditor.Rendering
{
    public class GameClass : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;

        public GameClass()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            graphics.SynchronizeWithVerticalRetrace = false;
            Window.Title = "DTA Scenario Editor";
        }

        private WindowManager windowManager;

        private readonly char DSC = Path.DirectorySeparatorChar;

        protected override void Initialize()
        {
            base.Initialize();

            AssetLoader.Initialize(GraphicsDevice, Content);

            windowManager = new WindowManager(this, graphics);
            windowManager.Initialize(Content, Environment.CurrentDirectory + DSC + "Content" + DSC);

            windowManager.InitGraphicsMode(1024, 600, false);
            windowManager.SetRenderResolution(1024, 600);
            windowManager.CenterOnScreen();
            windowManager.Cursor.LoadNativeCursor(Environment.CurrentDirectory + DSC + "Content" + DSC + "cursor.cur");

            Components.Add(windowManager);

            InitTest();
        }

        private void InitTest()
        {
            IniFile rulesIni = new IniFile("F:/Pelit/DTA Beta/INI/Rules.ini");
            IniFile firestormIni = new IniFile("F:/Pelit/DTA Beta/INI/Enhance.ini");
            IniFile mapIni = new IniFile("F:/Pelit/DTA Beta/Maps/Default/back_county.map");
            Map map = new Map();
            map.LoadExisting(rulesIni, firestormIni, mapIni);

            Console.WriteLine();
            Console.WriteLine("Map loaded.");

            Theater theater = new Theater("Temperate", "INI/Tem.ini", "IsoTem.mix", "isotem.pal", ".tem");
            theater.ReadConfigINI("F:/Pelit/DTA Beta/");

            CCFileManager ccFileManager = new CCFileManager();
            ccFileManager.AddSearchDirectory("F:/Pelit/DTA Beta/MIX/");
            ccFileManager.LoadPrimaryMixFile("Cache.mix");
            ccFileManager.LoadPrimaryMixFile(theater.ContentMIXName);

            TheaterGraphics theaterGraphics = new TheaterGraphics(GraphicsDevice, theater, ccFileManager, map.Rules);

            MapView mapView = new MapView(windowManager, map, theaterGraphics);
            mapView.Width = 1024;
            mapView.Height = 600;
            windowManager.AddAndInitializeControl(mapView);
        }
    }
}