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
        private Player boss;
        private Level level;

        private KeyboardState prevState;
        private MouseState mouseState;
        private MouseState prevMouseState;

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

            this.level = new Level(this.Content, new Point(75, 75), null, 20, 100);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.level.LoadContent();

            this.hoverTexture = Content.Load<Texture2D>("Hover");

            this.player = new Player(Content.Load<Texture2D>("Tiles/player"), new Vector2(
                this.level.Rooms[this.level.RoomSpawnIndex].Center.X * TheCavesOfKardun.TileWidth,
                this.level.Rooms[this.level.RoomSpawnIndex].Center.Y * TheCavesOfKardun.TileHeight), 500, 200,
                Content.Load<SpriteFont>("combatFont"));

            this.boss = new Player(Content.Load<Texture2D>("Tiles/player"), new Vector2(
                this.level.Rooms[this.level.BossRoomIndex].Center.X * TheCavesOfKardun.TileWidth,
                this.level.Rooms[this.level.BossRoomIndex].Center.Y * TheCavesOfKardun.TileHeight), 500, 80,
                Content.Load<SpriteFont>("combatFont"));
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

        /// <summary>
        /// Converts a pixel to the tile coordinates.
        /// </summary>
        /// <param name="position">The pixel position.</param>
        /// <returns>The tile cell coordinates.</returns>
        public static Vector2 ConvertCellToPosition(Point cell)
        {
            return new Vector2(
                cell.X * TheCavesOfKardun.TileWidth,
                cell.Y * TheCavesOfKardun.TileHeight);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (state.IsKeyDown(Keys.Enter) && prevState.IsKeyUp(Keys.Enter))
            {
                this.level.RecreateLevel();

                this.player.Position = new Vector2(
                    this.level.Rooms[this.level.RoomSpawnIndex].Center.X * TheCavesOfKardun.TileWidth,
                    this.level.Rooms[this.level.RoomSpawnIndex].Center.Y * TheCavesOfKardun.TileHeight);

                this.boss.Position = new Vector2(
                    this.level.Rooms[this.level.BossRoomIndex].Center.X * TheCavesOfKardun.TileWidth,
                    this.level.Rooms[this.level.BossRoomIndex].Center.Y * TheCavesOfKardun.TileHeight);
            }

            if (state.IsKeyDown(Keys.Escape) && prevState.IsKeyUp(Keys.Escape))
                Exit();

            UpdateGameplay(gameTime);

            this.cameraPosition.X = this.player.Center.X - GraphicsDevice.Viewport.Width / 2;
            this.cameraPosition.Y = this.player.Center.Y - GraphicsDevice.Viewport.Height / 2;
         
            prevState = state;
            prevMouseState = mouseState;

            if (this.player.Health <= 0)
                throw new Exception("Player died");

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.level.Draw(gameTime, spriteBatch, cameraPosition, this.player.Position, TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition), TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition + new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) + new Vector2(TheCavesOfKardun.TileWidth)));

            this.player.Draw(gameTime, spriteBatch, cameraPosition);
            this.boss.Draw(gameTime, spriteBatch, cameraPosition);

            spriteBatch.Begin();
            spriteBatch.Draw(this.hoverTexture, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles movement for the player.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void UpdateGameplay(GameTime gameTime)
        {
            if (this.player.UpdateMovement(gameTime))
                return;

            Point targetTile = Point.Zero;

            // Reset the motion.
            Vector2 motion = Vector2.Zero;
            int amountOfTiles = 0;
            if (this.mouseState.LeftButton == ButtonState.Pressed && this.prevMouseState.LeftButton == ButtonState.Released)
            {
                // Check if we pressed the left mouse button.

                // Convert the players position to the playerTile and the targetTile from the mouse position.
                targetTile = TheCavesOfKardun.ConvertPositionToCell(new Vector2(this.mouseState.X + this.cameraPosition.X, this.mouseState.Y + this.cameraPosition.Y));
                Point playerTile = TheCavesOfKardun.ConvertPositionToCell(this.player.Center);

                // Does the X-axis match? 
                if (playerTile.X == targetTile.X)
                {
                    // Check if you clicked below or above the player.
                    for (int i = 1; i <= this.player.AmountOfTilesToMove; i++)
                    {
                        if (playerTile.Y + i == targetTile.Y)
                        {
                            motion.Y++;
                            amountOfTiles = i;
                            break;
                        }
                        else if (playerTile.Y - i == targetTile.Y)
                        {
                            motion.Y--;
                            amountOfTiles = i;
                            break;
                        }
                    }
                }
                else if (playerTile.Y == targetTile.Y) // Does the Y-axis match instead?
                {
                    // Check if you clicked to the left or to the right of the player.
                    for (int i = 1; i <= this.player.AmountOfTilesToMove; i++)
                    {
                        if (playerTile.X + i == targetTile.X)
                        {
                            motion.X++;
                            amountOfTiles = i;
                            break;
                        }
                        else if (playerTile.X - i == targetTile.X)
                        {
                            motion.X--;
                            amountOfTiles = i;
                            break;
                        }
                    }
                }
            }

            // If we should move, check if we can and if we can set the players motion.
            if (motion != Vector2.Zero)
            {
                motion.Normalize();

                Monster monster;
                Objects typeOfObject;
                
                if (this.level.CanWalk(this.player, motion, amountOfTiles, out this.player.TargetPosition)) // Check if we can walk.
                {
                    this.player.Motion = motion;
                    this.level.MonsterAI(this.player);
                }
                else if (targetTile != Point.Zero && amountOfTiles == 1 && this.level.EncounterMonster(targetTile, out monster)) // Check if we've clicked on a monster.
                {
                    if (string.IsNullOrWhiteSpace(this.player.CombatText))
                    {
                        this.player.Attack(gameTime, monster);
                        this.level.MonsterAI(this.player);
                    }
                }
                else if (targetTile != Point.Zero && amountOfTiles == 1 && this.level.EncounterObject(targetTile, out typeOfObject)) // Check if we clicked on an item.
                {
                    this.player.PickUp(typeOfObject);
                    this.level.MonsterAI(this.player);
                }
            }
        }

        #endregion
    }
}