#region File Description
//////////////////////////////////////////////////////////////////////////
// Room                                                         //
//                                                                      //
// Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System;
#endregion End of Using Statements

namespace The_Caves_of_Kardun.TileEngine
{
    public sealed class Room
    {
        #region Fields

        Random random;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the distance of this room to the spawn room.
        /// </summary>
        public double DistanceToSpawn { get; set; }

        /// <summary>
        /// Gets the left.
        /// </summary>
        public int Left { get; private set; }
        
        /// <summary>
        /// Gets the top.
        /// </summary>
        public int Top { get; private set; }

        /// <summary>
        /// Gets the width in tiles.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height in tiles.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        public int Right
        {
            get { return Left + Width - 1; }
        }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        public int Bottom
        {
            get { return Top + Height - 1; }
        }

        /// <summary>
        /// Gets the center of the room.
        /// </summary>
        public Point Center
        {
            get
            {
                return new Point(Left + Width/2, Top + Height/2);
            }
        }

        /// <summary>
        /// Gets a random floor tile.
        /// </summary>
        public Point RandomFloorTile
        {
            get
            {
                Point point = new Point();
                point.X = random.Next((this.Left + 1), (this.Right - 1));
                point.Y = random.Next((this.Top + 1), (this.Bottom - 1));

                return point;
            }
        }

        public int[,] MapData { get; set; }

        public bool IsConnect { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new room.
        /// </summary>
        /// <param name="left">Tiles between the left corner and to the start of the room.</param>
        /// <param name="top">Tiles between the top corner and to the start of the room.</param>
        /// <param name="width">The width of the room.</param>
        /// <param name="height">The height of the room.</param>
        public Room(int left, int top, int width, int height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;

            this.DistanceToSpawn = 0;
            this.MapData = new int[width, height];
            this.random = new Random();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if two rooms collides with each other.
        /// </summary>
        /// <param name="other">The other room.</param>
        /// <returns>Returns true if the room collides; otherwise false.</returns>
        public bool CollidesWith(Room other)
        {
            if (Left > other.Right + 1)
                return false;

            if (Top > other.Bottom + 1)
                return false;

            if (Right < other.Left - 1)
                return false;

            if (Bottom < other.Top - 1)
                return false;

            return true;
        }

        #endregion
    }
}