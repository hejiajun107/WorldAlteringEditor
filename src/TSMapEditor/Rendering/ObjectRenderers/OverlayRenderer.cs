﻿using Microsoft.Xna.Framework;
using TSMapEditor.GameMath;
using TSMapEditor.Models;

namespace TSMapEditor.Rendering.ObjectRenderers
{
    public sealed class OverlayRenderer : ObjectRenderer<Overlay>
    {
        public OverlayRenderer(RenderDependencies renderDependencies) : base(renderDependencies)
        {
        }

        protected override Color ReplacementColor => new Color(255, 0, 255);

        protected override CommonDrawParams GetDrawParams(Overlay gameObject)
        {
            return new CommonDrawParams()
            {
                IniName = gameObject.OverlayType.ININame,
                ShapeImage = TheaterGraphics.OverlayTextures[gameObject.OverlayType.Index]
            };
        }

        protected override float GetDepthFromPosition(Overlay gameObject, Rectangle drawingBounds)
        {
            // Calculate position-related depth from the southernmost edge of the cell of the southernmost texture coordinate of the object.
            var cellPixelCoords = CellMath.CellTopLeftPointFromCellCoords(gameObject.Position, Map);
            int dy = drawingBounds.Bottom - cellPixelCoords.Y;
            int wholeCells = dy / Constants.CellSizeY;
            int fraction = dy % Constants.CellSizeY;
            int cellY = cellPixelCoords.Y + (wholeCells + 1) * Constants.CellSizeY;

            if (fraction > (Constants.CellSizeY * 3) / 2 &&
                (drawingBounds.X < cellPixelCoords.X || drawingBounds.Right > cellPixelCoords.X + Constants.CellSizeX))
            {
                // This object leaks into the neighbouring cells - to another "isometric row"
                cellY += Constants.CellSizeY / 2;
            }

            // Use height from the cell where the object has been placed.
            var heightLookupCell = Map.GetTile(gameObject.Position);
            int height = 0;
            if (heightLookupCell != null)
            {
                height = heightLookupCell.Level;
            }

            return ((cellY + (height * Constants.CellHeight)) / (float)Map.HeightInPixelsWithCellHeight) * Constants.DownwardsDepthRenderSpace +
                (height * Constants.DepthRenderStep);
        }

        protected override float GetDepthAddition(Overlay gameObject)
        {
            if (gameObject.OverlayType.HighBridgeDirection == BridgeDirection.None)
            {
                // Draw overlays above smudges
                return Constants.DepthEpsilon * ObjectDepthAdjustments.Overlay;
            }

            const int bridgeHeight = 4;

            var tile = Map.GetTile(gameObject.Position);
            return (Constants.DepthEpsilon * ObjectDepthAdjustments.Overlay) + ((tile.Level + bridgeHeight) * Constants.CellHeight / (float)Map.HeightInPixelsWithCellHeight);
        }

        protected override void Render(Overlay gameObject, Point2D drawPoint, in CommonDrawParams drawParams)
        {
            Color remapColor = Color.White;
            if (gameObject.OverlayType.TiberiumType != null)
                remapColor = gameObject.OverlayType.TiberiumType.XNAColor;

            if (!RenderDependencies.EditorState.Is2DMode && gameObject.OverlayType.HighBridgeDirection != BridgeDirection.None)
            {
                if (gameObject.OverlayType.HighBridgeDirection == BridgeDirection.EastWest)
                {
                    drawPoint.Y -= Constants.CellHeight + 1;
                }
                else
                {
                    drawPoint.Y -= Constants.CellHeight * 2 + 1;
                }
            }

            bool affectedByLighting = drawParams.ShapeImage.SubjectToLighting;
            bool affectedByAmbient = !gameObject.OverlayType.Tiberium && !affectedByLighting;

            DrawShadow(gameObject);
            DrawShapeImage(gameObject, drawParams.ShapeImage, gameObject.FrameIndex, Color.White,
                true, remapColor, affectedByLighting, affectedByAmbient, drawPoint);
        }
    }
}
