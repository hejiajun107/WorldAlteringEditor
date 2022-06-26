﻿using Microsoft.Xna.Framework.Input;
using System;
using TSMapEditor.GameMath;
using TSMapEditor.Rendering;

namespace TSMapEditor.UI
{
    public abstract class CursorAction
    {
        public CursorAction(ICursorActionTarget cursorActionTarget)
        {
            CursorActionTarget = cursorActionTarget;
        }

        /// <summary>
        /// Raised when the cursor action is exited. 
        /// Typically this happens through the user right-clicking or 
        /// through the activation of a different cursor action.
        /// </summary>
        public event EventHandler ActionExited;

        /// <summary>
        /// Raised when the action itself wants it to be disabled.
        /// </summary>
        public event EventHandler OnExitingAction;

        public void OnActionExit() => ActionExited?.Invoke(this, EventArgs.Empty);

        public void ExitAction() => OnExitingAction?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Override in derived classes to enable this cursor action to receive
        /// keyboard events through <see cref="OnKeyPressed"/>.
        /// </summary>
        public virtual bool HandlesKeyboardInput => false;

        /// <summary>
        /// Override in derived classes to enable the cell cursor to be drawn
        /// while this cursor action is active.
        /// </summary>
        public virtual bool DrawCellCursor => false;


        protected ICursorActionTarget CursorActionTarget { get; }

        /// <summary>
        /// Called when the action is activated (when it becomes the cursor action that the user is using).
        /// </summary>
        public virtual void OnActionEnter() { }

        /// <summary>
        /// Called when a keyboard key is pressed while the cursor action is active.
        /// </summary>
        /// <param name="e">The key press event from the XNAUI library.</param>
        public virtual void OnKeyPressed(Rampastring.XNAUI.Input.KeyPressEventArgs e) { }

        /// <summary>
        /// Called prior to drawing the map.
        /// </summary>
        /// <param name="cellCoords">The coords of the cell under the cursor.</param>
        public virtual void PreMapDraw(Point2D cellCoords) { }

        /// <summary>
        /// Called after drawing the map.
        /// Override in derived classes to clear preview data related to the action.
        /// </summary>
        /// <param name="cellCoords">The coords of the cell under the cursor.</param>
        public virtual void PostMapDraw(Point2D cellCoords) { }

        /// <summary>
        /// Called when the mouse is moved on the map with the left mouse button down while this action being active.
        /// </summary>
        /// <param name="cellCoords">The coords of the cell under the cursor.</param>
        public virtual void LeftDown(Point2D cellCoords) { }

        /// <summary>
        /// Called when the left mouse button is clicked (pressed and released) on the map with this action being active.
        /// </summary>
        /// <param name="cellCoords">The coords of the cell under the cursor.</param>
        public virtual void LeftClick(Point2D cellCoords) { }

        /// <summary>
        /// Called after drawing the map.
        /// Override in derived classes to draw on top of the map texture.
        /// </summary>
        /// <param name="cellCoords">The coords of the cell under the cursor.</param>
        /// <param name="cameraTopLeftPoint">The top-left point of the user's screen.</param>
        public virtual void DrawPreview(Point2D cellCoords, Point2D cameraTopLeftPoint) { }
    }
}