#region File Description
//////////////////////////////////////////////////////////////////////////
// Level                                                         //
//                                                                      //
// Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion End of Using Statements

namespace The_Caves_of_Kardun.TileEngine
{
    public class Level
    {
        #region Fields

        private int? randomSeed;
        private int amountOfRooms;
        private int maxFails;

        private int[,] mapData;
        private List<Room> rooms = new List<Room>();
        private Point mapDimensions;

        private Texture2D doorTexture;
        private Texture2D floorTexture;
        private Texture2D wallTexture;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the map data.
        /// </summary>
        public int[,] MapData
        {
            get { return this.mapData; }
        }

        public List<Room> Rooms 
        { 
            get { return this.rooms; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="mapDimensions"></param>
        /// <param name="randomSeed"></param>
        /// <param name="amountOfRooms"></param>
        /// <param name="maxFails"></param>
        public Level(Point mapDimensions, int? randomSeed, int amountOfRooms, int maxFails)
        {
            this.mapDimensions = mapDimensions;
            this.mapData = new int[mapDimensions.X, mapDimensions.Y];

            this.randomSeed = randomSeed;
            this.amountOfRooms = amountOfRooms;
            this.maxFails = maxFails;

            MakeLevel(randomSeed, amountOfRooms, maxFails);
        }

        #endregion

        #region Public Methods

        public void LoadContent(ContentManager content)
        {
            this.floorTexture = content.Load<Texture2D>("Tiles/floor");
            this.wallTexture = content.Load<Texture2D>("Tiles/wall");
            this.doorTexture = content.Load<Texture2D>("Tiles/door");
        }

        /// <summary>
        /// Recreates the level.
        /// </summary>
        public void RecreateLevel()
        {
            MakeLevel(this.randomSeed, this.amountOfRooms, this.maxFails);
        }

        /// <summary>
        /// Draws the level.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraPosition"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, Point min, Point max)
        {
            min.X = Math.Max(min.X, 0);
            min.Y = Math.Max(min.Y, 0);

            max.X = Math.Min(max.X, mapDimensions.X);
            max.Y = Math.Min(max.Y, mapDimensions.Y);

            spriteBatch.Begin();
            for (int x = min.X; x < max.X; x++)
            {
                for (int y = min.Y; y < max.Y; y++)
                {
                    if (mapData[x, y] == 1)
                        spriteBatch.Draw(floorTexture,
                            new Rectangle(
                                (int)(x * TheCavesOfKardun.TileWidth - cameraPosition.X),
                                (int)(y * TheCavesOfKardun.TileHeight - cameraPosition.Y),
                                TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight),
                            Color.White);
                    else if (mapData[x, y] == 2)
                        spriteBatch.Draw(wallTexture,
                            new Rectangle(
                                (int)(x * TheCavesOfKardun.TileWidth - cameraPosition.X),
                                (int)(y * TheCavesOfKardun.TileHeight - cameraPosition.Y),
                                TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight),
                            Color.White);
                    else if (mapData[x, y] == 3)
                        spriteBatch.Draw(doorTexture,
                            new Rectangle(
                                (int)(x * TheCavesOfKardun.TileWidth - cameraPosition.X),
                                (int)(y * TheCavesOfKardun.TileHeight - cameraPosition.Y),
                                TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight),
                            Color.White);
                }
            }
            spriteBatch.End();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Creates the entire level.
        /// </summary>
        /// <param name="randomSeed"></param>
        /// <param name="amountOfRooms"></param>
        /// <param name="maxFails"></param>
        private void MakeLevel(int? randomSeed, int amountOfRooms, int maxFails)
        {
            this.rooms.Clear();
            this.mapData = new int[mapDimensions.X, mapDimensions.Y];

            Random random;

            if (randomSeed.HasValue)
                random = new Random(randomSeed.Value);
            else
                random = new Random();

            while (this.rooms.Count < amountOfRooms)
            {
                int width = random.Next(8, 14);
                int height = random.Next(8, 14);

                Room room = new Room(
                    random.Next(0, mapDimensions.X - width),
                    random.Next(0, mapDimensions.Y - height),
                    width, height);

                if (!RoomCollides(room))
                    rooms.Add(room);
                else
                {
                    maxFails--;
                    if (maxFails <= 0)
                        break;
                }
            }

            foreach (Room room in this.rooms)
                MakeRoom(room);

            for (int i = 0; i < this.rooms.Count - 1; i++)
            {
                MakeCorridor(this.rooms[i], this.rooms[i + 1]);
            }

            MakeWalls();
        }

        /// <summary>
        /// Creates a room.
        /// </summary>
        /// <param name="room"></param>
        private void MakeRoom(Room room)
        {
            for (int x = 0; x < room.Width; x++)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    if (x == 0 || x == room.Width - 1 || y == 0 || y == room.Height - 1)
                        mapData[room.Left + x, room.Top + y] = 2;
                    else
                        mapData[room.Left + x, room.Top + y] = 1;
                }
            }
        }

        /// <summary>
        /// Creates a corridor
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        private void MakeCorridor(Room r1, Room r2)
        {
            int x = r1.Center.X;
            int y = r1.Center.Y;

            if (x == 0 && y == 0)
                return;

            int x2 = r2.Center.X;
            int y2 = r2.Center.Y;

            if (x2 == 0 && y2 == 0)
                return;

            while (x != x2)
            {
                this.mapData[x, y] = 1;

                x += (x < x2) ? 1 : -1;
            }

            while (y != y2)
            {
                this.mapData[x, y] = 1;

                y += (y < y2) ? 1 : -1;
            }

            r1.IsConnect = true;
            r2.IsConnect = true;
        }

        /// <summary>
        /// Creates walls
        /// </summary>
        private void MakeWalls()
        {
            for (int x = 0; x < this.mapDimensions.X; x++)
                for (int y = 0; y < this.mapDimensions.Y; y++)
                    if (this.mapData[x, y] == 0 && HasAdjacentFloor(x, y))
                        this.mapData[x, y] = 2;
        }

        /// <summary>
        /// Check if we should se a wall.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool HasAdjacentFloor(int x, int y)
        {
            if (x > 0 && this.mapData[x - 1, y] == 1)
                return true;
            if (x < this.mapDimensions.X - 1 && this.mapData[x + 1, y] == 1)
                return true;

            if (y > 0 && this.mapData[x, y - 1] == 1)
                return true;
            if (y < this.mapDimensions.Y - 1 && this.mapData[x, y + 1] == 1)
                return true;

            if (x > 0 && y > 0 && this.mapData[x - 1, y - 1] == 1)
                return true;
            if (x < mapDimensions.X - 1 && y > 0 && this.mapData[x + 1, y - 1] == 1)
                return true;

            if (x > 0 && y < mapDimensions.Y - 1 && this.mapData[x - 1, y + 1] == 1)
                return true;
            if (x < mapDimensions.X - 1 && y < mapDimensions.Y - 1 && this.mapData[x + 1, y + 1] == 1)
                return true;

            return false;
        }

        /// <summary>
        /// Does the room collide with another room?
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private bool RoomCollides(Room room)
        {
            foreach (Room r in this.rooms)
                if (room.CollidesWith(r))
                    return true;

            return false;
        }

        #endregion

        #region Nested Types
        #endregion
    }
}