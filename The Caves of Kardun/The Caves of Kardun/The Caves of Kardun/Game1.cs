#region File Description
//----------------------------------------------------------------------------
// Game1.cs
//
// Copyright (C) . All rights reserved.
//----------------------------------------------------------------------------
#endregion End of File Description

#region Using Statements
// System
using System;
using System.Linq;
using System.Collections.Generic;

// Microsoft XNA Framework
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using The_Caves_of_Kardun.TileEngine;
#endregion End of Using Statements

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        #region Fields

        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        private int[,] mapData;
        private List<Room> rooms = new List<Room>();
        private Point mapDimensions;
        private Point tileSize;

        private Texture2D floorTexture;
        private Texture2D wallTexture;

        #endregion

        #region Properties
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public Game1()
        {
            this.graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferWidth = 1280;
            graphicsDeviceManager.PreferredBackBufferHeight = 720;
            graphicsDeviceManager.ApplyChanges();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.Content.RootDirectory = "Content";
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            mapDimensions = new Point(80, 45);
            mapData = new int[mapDimensions.X, mapDimensions.Y];
            tileSize = new Point(16, 16);

            MakeLevel(null, 50);

            base.Initialize();
        }

        private void MakeLevel(int? randomSeed, int tryCreateAmountOfRooms)
        {
            this.rooms.Clear();
            this.mapData = new int[mapDimensions.X, mapDimensions.Y];

            Random random;

            if (randomSeed.HasValue)
                random = new Random(randomSeed.Value);
            else
                random = new Random();

            for (int i = 0; i < tryCreateAmountOfRooms; i++)
            {
                int width = random.Next(8, 16);
                int height = random.Next(8, 16);

                Room room = new Room(
                    random.Next(0, mapDimensions.X - width),
                    random.Next(0, mapDimensions.Y - height),
                    width, height);

                if (!RoomCollides(room))
                    rooms.Add(room);
            }

            foreach (Room room in this.rooms)
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
        }

        private bool RoomCollides(Room room)
        {
            foreach (Room r in this.rooms)
                if (room.CollidesWith(r))
                    return true;

            return false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.floorTexture = Content.Load<Texture2D>("Tiles/floor");
            this.wallTexture = Content.Load<Texture2D>("Tiles/wall");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                MakeLevel(null, 50);

            base.Update(gameTime);
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            for (int x = 0; x < mapDimensions.X; x++)
            {
                for (int y = 0; y < mapDimensions.Y; y++)
                {
                    if (mapData[x, y] == 1)
                        spriteBatch.Draw(floorTexture, new Vector2(x * tileSize.X, y * tileSize.Y), Color.White);
                    else if (mapData[x, y] == 2)
                        spriteBatch.Draw(wallTexture, new Vector2(x * tileSize.X, y * tileSize.Y), Color.White);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}