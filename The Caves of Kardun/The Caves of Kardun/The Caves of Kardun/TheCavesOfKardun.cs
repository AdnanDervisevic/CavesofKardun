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
    public class TheCavesOfKardun : Game
    {
        /*
         * 
         * FOG OF WAR
         * ----------
         * Rita bara det som spelaren har sett
         * Ändra Alpha på varje tile beroende på hur nära spelaren har varit
         * 
         * */

        #region Consts

        public static int TileWidth = 96;
        public static int TileHeight = 96;

        #endregion

        #region Fields


        private Random random;
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        private Texture2D hoverTexture;

        private Vector2 cameraPosition;
        private Player player;
        private Level level;

        #endregion

        #region Properties
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TheCavesOfKardun()
        {
            this.graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferWidth = 1280;
            graphicsDeviceManager.PreferredBackBufferHeight = 720;
            //graphicsDeviceManager.IsFullScreen = true;
            graphicsDeviceManager.ApplyChanges();
            IsMouseVisible = true;
            this.random = new Random();
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

            this.level = new Level(new Point(75, 75), null, 20, 100);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.level.LoadContent(Content);

            this.hoverTexture = Content.Load<Texture2D>("Hover");

            int roomIndex = random.Next(0, this.level.Rooms.Count - 1);

            this.player = new Player(Content.Load<Texture2D>("Tiles/player"), new Vector2(
                this.level.Rooms[roomIndex].Center.X * TheCavesOfKardun.TileWidth,
                this.level.Rooms[roomIndex].Center.Y * TheCavesOfKardun.TileHeight), 500);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Converts a pixel to the tile coordinates.
        /// </summary>
        /// <param name="position">The pixel position.</param>
        /// <returns>The tile cell coordinates.</returns>
        public static Point ConvertPositionToCell(Vector2 position)
        {
            return new Point(
                (int)(position.X) / TheCavesOfKardun.TileWidth,
                (int)(position.Y) / TheCavesOfKardun.TileHeight);
        }

        #endregion

        #region Public Methods

        KeyboardState prevState;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Enter) && prevState.IsKeyUp(Keys.Enter))
            {
                this.level.RecreateLevel();

                int roomIndex = random.Next(0, this.level.Rooms.Count - 1);
                this.player.Position = new Vector2(
                    this.level.Rooms[roomIndex].Center.X * TheCavesOfKardun.TileWidth,
                    this.level.Rooms[roomIndex].Center.Y * TheCavesOfKardun.TileHeight);
            }

            if (state.IsKeyDown(Keys.Escape) && prevState.IsKeyUp(Keys.Escape))
                Exit();

            UpdateMovement(gameTime);

            if (this.player.Motion != Vector2.Zero || this.player.Attacks)
                UpdateAI();

            this.cameraPosition.X = this.player.Center.X - GraphicsDevice.Viewport.Width/2;
            this.cameraPosition.Y = this.player.Center.Y - GraphicsDevice.Viewport.Height / 2;
         
            prevState = state;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.level.Draw(spriteBatch, cameraPosition, this.player.Position, TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition), TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition + new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) + new Vector2(TheCavesOfKardun.TileWidth)));

            this.player.Draw(spriteBatch, cameraPosition);

            spriteBatch.Begin();
            spriteBatch.Draw(this.hoverTexture, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the AI
        /// </summary>
        private void UpdateAI()
        {
        }

        /// <summary>
        /// Handles movement for the player.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void UpdateMovement(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (this.player.Motion.X == 1 && this.player.Motion.Y == 0)
            {
                if (this.player.Position.X >= this.player.TargetPosition.X)
                {
                    this.player.Position.X = this.player.TargetPosition.X;
                    this.player.Motion = Vector2.Zero;
                    return;
                }
            }
            else if (this.player.Motion.X == -1 && this.player.Motion.Y == 0)
            {
                if (this.player.Position.X <= this.player.TargetPosition.X)
                {
                    this.player.Position.X = this.player.TargetPosition.X;
                    this.player.Motion = Vector2.Zero;
                    return;
                }
            }
            else if (this.player.Motion.X == 0 && this.player.Motion.Y == 1)
            {
                if (this.player.Position.Y >= this.player.TargetPosition.Y)
                {
                    this.player.Position.Y = this.player.TargetPosition.Y;
                    this.player.Motion = Vector2.Zero;
                    return;
                }
            }
            else if (this.player.Motion.X == 0 && this.player.Motion.Y == -1)
            {
                if (this.player.Position.Y <= this.player.TargetPosition.Y)
                {
                    this.player.Position.Y = this.player.TargetPosition.Y;
                    this.player.Motion = Vector2.Zero;
                    return;
                }
            }
            else
            {
                
                Vector2 motion = Vector2.Zero;
                if (state.IsKeyDown(Keys.W) && !prevState.IsKeyUp(Keys.W))
                    motion.Y--;
                else if (state.IsKeyDown(Keys.S) && !prevState.IsKeyUp(Keys.S))
                    motion.Y++;
                else if (state.IsKeyDown(Keys.A) && !prevState.IsKeyUp(Keys.A))
                    motion.X--;
                else if (state.IsKeyDown(Keys.D) && !prevState.IsKeyUp(Keys.D))
                    motion.X++;

                if (motion != Vector2.Zero)
                {
                    motion.Normalize();

                    int[,] mapData = this.level.MapData;

                    Point playerCoords = TheCavesOfKardun.ConvertPositionToCell(this.player.Center);
                    this.player.TargetPosition = Vector2.Zero;

                    if (motion.X == 1 && motion.Y == 0 && mapData[playerCoords.X + 1, playerCoords.Y] == 1)
                        this.player.TargetPosition.X = this.player.Position.X + TheCavesOfKardun.TileWidth;
                    else if (motion.X == -1 && motion.Y == 0 && mapData[playerCoords.X - 1, playerCoords.Y] == 1)
                        this.player.TargetPosition.X = this.player.Position.X - TheCavesOfKardun.TileWidth;
                    else if (motion.X == 0 && motion.Y == 1 && mapData[playerCoords.X, playerCoords.Y + 1] == 1)
                        this.player.TargetPosition.Y = this.player.Position.Y + TheCavesOfKardun.TileHeight;
                    else if (motion.X == 0 && motion.Y == -1 && mapData[playerCoords.X, playerCoords.Y - 1] == 1)
                        this.player.TargetPosition.Y = this.player.Position.Y - TheCavesOfKardun.TileHeight;

                    if (this.player.TargetPosition != Vector2.Zero)
                        this.player.Motion = motion;
                }
            }

            this.player.Position.X += this.player.Motion.X * this.player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.player.Position.Y += this.player.Motion.Y * this.player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        #endregion
    }
}