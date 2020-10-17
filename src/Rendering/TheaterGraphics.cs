﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TSMapEditor.CCEngine;
using TSMapEditor.Models;

namespace TSMapEditor.Rendering
{
    public class TileImage
    {
        public TileImage(int tileSetId, int tileIndex, MGTMPImage[] tMPImages)
        {
            TileSetId = tileSetId;
            TileIndex = tileIndex;
            TMPImages = tMPImages;
        }

        public int TileSetId { get; set; }
        public int TileIndex { get; set; }
        public MGTMPImage[] TMPImages { get; set; }
    }

    public class TerrainImage
    {
        public TerrainImage(GraphicsDevice graphicsDevice, ShpFile shp, byte[] shpFileData, Palette palette)
        {
            Frames = new PositionedTexture[shp.FrameCount];
            for (int i = 0; i < shp.FrameCount; i++)
            {
                var frameInfo = shp.GetShpFrameInfo(i);
                var frameData = shp.GetUncompressedFrameData(i, shpFileData);
                if (frameData == null)
                    continue;

                var texture = new Texture2D(graphicsDevice, frameInfo.Width, frameInfo.Height, false, SurfaceFormat.Color);
                Color[] colorArray = frameData.Select(b => b == 0 ? Color.Transparent : palette.Data[b].ToXnaColor()).ToArray();
                texture.SetData<Color>(colorArray);
                Frames[i] = new PositionedTexture(shp.Width, shp.Height, frameInfo.XOffset, frameInfo.YOffset, texture);
            }
        }

        public PositionedTexture[] Frames { get; set; }
    }

    public class PositionedTexture
    {
        public int ShapeWidth;
        public int ShapeHeight;
        public int OffsetX;
        public int OffsetY;
        public Texture2D Texture;

        public PositionedTexture(int shapeWidth, int shapeHeight, int offsetX, int offsetY, Texture2D texture)
        {
            ShapeWidth = shapeWidth;
            ShapeHeight = shapeHeight;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Texture = texture;
        }
    }

    /// <summary>
    /// A wrapper for a theater that loads and stores tile graphics.
    /// </summary>
    public class TheaterGraphics
    {
        public TheaterGraphics(GraphicsDevice graphicsDevice, Theater theater, CCFileManager fileManager, Rules rules)
        {
            Theater = theater;
            byte[] paletteData = fileManager.LoadFile(theater.PaletteName);
            palette = new Palette(paletteData);
            this.fileManager = fileManager;

            ReadTileTextures(graphicsDevice);
            ReadTerrainObjectTextures(graphicsDevice, rules.TerrainTypes);
        }

        private void ReadTileTextures(GraphicsDevice graphicsDevice)
        {
            for (int tsId = 0; tsId < Theater.TileSets.Count; tsId++)
            {
                TileSet tileSet = Theater.TileSets[tsId];

                Console.WriteLine("Loading " + tileSet.SetName);

                for (int i = 0; i < tileSet.TilesInSet; i++)
                {
                    Console.WriteLine("#" + i);

                    // Handle graphics variation (clear00.tem, clear00a.tem, clear00b.tem etc.)
                    for (int v = 0; v < 'g' - 'a'; v++)
                    {
                        string baseName = tileSet.FileName + (i + 1).ToString("D2", CultureInfo.InvariantCulture);

                        if (v > 0)
                        {
                            baseName = baseName + ((char)('a' + (v - 1)));
                        }

                        byte[] data = fileManager.LoadFile(baseName + Theater.FileExtension);

                        if (data == null)
                        {
                            if (v == 0)
                            {
                                graphicsList.Add(new TileImage(tsId, i, new MGTMPImage[0]));
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }

                        var tmpFile = new TmpFile();
                        tmpFile.ParseFromBuffer(data);

                        var tmpImages = new List<MGTMPImage>();
                        for (int img = 0; img < tmpFile.ImageCount; img++)
                        {
                            tmpImages.Add(new MGTMPImage(graphicsDevice, tmpFile.GetImage(img), palette, tsId));
                        }
                        graphicsList.Add(new TileImage(tsId, i, tmpImages.ToArray()));
                        break;
                    }
                }
            }
        }

        public void ReadTerrainObjectTextures(GraphicsDevice graphicsDevice, List<TerrainType> terrainTypes)
        {
            byte[] theaterPaletteData = fileManager.LoadFile(Theater.PaletteName);
            if (theaterPaletteData == null)
                throw new KeyNotFoundException(Theater.PaletteName + " not found from loaded MIX files!");
            var theaterPalette = new Palette(theaterPaletteData);

            byte[] unitPaletteData = fileManager.LoadFile(Theater.UnitPaletteName);
            if (unitPaletteData == null)
                throw new KeyNotFoundException(Theater.UnitPaletteName + " not found from loaded MIX files!");
            var unitPalette = new Palette(unitPaletteData);

            TerrainObjectTextures = new TerrainImage[terrainTypes.Count];
            for (int i = 0; i < terrainTypes.Count; i++)
            {
                string shpFileName = terrainTypes[i].Image != null ? terrainTypes[i].Image : terrainTypes[i].ININame;
                if (terrainTypes[i].Theater)
                    shpFileName += Theater.FileExtension;
                else
                    shpFileName += ".SHP";

                byte[] data = fileManager.LoadFile(shpFileName);
                if (data == null)
                    continue;

                var shpFile = new ShpFile();
                shpFile.ParseFromBuffer(data);
                TerrainObjectTextures[i] = new TerrainImage(graphicsDevice, shpFile, data, 
                    terrainTypes[i].SpawnsTiberium ? unitPalette : theaterPalette);
            }
        }

        public Theater Theater { get; }

        private CCFileManager fileManager;

        private Palette palette;

        private List<TileImage> graphicsList = new List<TileImage>();

        public int TileCount => graphicsList.Count;

        public TileImage GetTileGraphics(int id) => graphicsList[id];


        public TerrainImage[] TerrainObjectTextures { get; set; }
    }
}