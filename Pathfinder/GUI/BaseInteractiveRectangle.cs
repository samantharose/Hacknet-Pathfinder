﻿using System;
using Hacknet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gui = Hacknet.Gui;

namespace Pathfinder.GUI
{
    public abstract class BaseInteraction<T>
    {
        protected BaseInteraction(T x, T y, T width, T height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public virtual bool IsActive => !GuiData.blockingInput && Contains(GuiData.getMousePoint());
        public virtual bool IsReleased => IsActive && (GuiData.mouseLeftUp() || GuiData.mouse.LeftButton == ButtonState.Released);
        public virtual bool IsHeldDown => IsActive && (GuiData.isMouseLeftDown()
                                               && !Gui.Button.DisableIfAnotherIsActive) && !IsReleased;
        public virtual bool JustReleased { get; protected set; }
        public virtual bool WasHeld { get; protected set; }
        public T X { get; set; }
        public T Y { get; set; }
        public T Width { get; set; }
        public T Height { get; set; }
        public abstract bool HandleInteraction();

        public bool Contains(Point p) => Contains(X, Y, Width, Height, p);

        public static bool Contains(T x, T y, T width, T height, Point p)
        {
            return Convert.ToDouble(x) <= p.X
                          && p.X < Convert.ToDouble(x) + Convert.ToDouble(width)
                          && Convert.ToDouble(y) <= p.Y
                          && p.Y < Convert.ToDouble(y) + Convert.ToDouble(height);
        }
    }

    public abstract class BaseInteractiveRectangle<T> : BaseInteraction<T>
    {
        protected BaseInteractiveRectangle(T x, T y, T width, T height) : base(x, y, width, height) {}

        public override bool HandleInteraction()
        {
            if (WasHeld && IsReleased)
            {
                JustReleased = true;
                WasHeld = false;
            }
            else
                JustReleased = false;
            WasHeld |= IsHeldDown;
            return JustReleased;
        }

        public virtual bool Draw()
        {
            DoDraw();
            return HandleInteraction();
        }

        public abstract void DoDraw();
    }
}