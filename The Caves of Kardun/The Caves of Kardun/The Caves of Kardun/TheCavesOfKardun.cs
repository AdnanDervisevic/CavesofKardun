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
        #region Consts

        //public const int TileWidth = 96;
        //public const int TileHeight = 96;

        #endregion

        #region Fields

        private Random random;
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        private Texture2D hoverTexture;

        private Vector2 cameraPosition;
        private Player player;
        private Level level;

        private RenderTarget2D minimapRenderTarget;
        private Texture2D minimapTexture;
        private bool updateMiniMap;

        private static MouseState currentMouseState;
        private static MouseState previousMouseState;

        private static KeyboardState currentKeyboardState;
        private static KeyboardState previousKeyboardState;

        #endregion

        #region Properties

        public static int TileWidth { get; set; }

        public static int TileHeight { get; set; }

        /// <summary>
        /// Gets the current mouse state.
        /// </summary>
        public static MouseState CurrentMouseState
        {
            get { return currentMouseState; }
        }

        /// <summary>
        /// Gets the previous mouse state.
        /// </summary>
        public static MouseState PreviousMouseState
        {
            get { return previousMouseState; }
        }

        /// <summary>
        /// Gets the current keyboard state.
        /// </summary>
        public static KeyboardState CurrentKeyboardState
        {
            get { return currentKeyboardState; }
        }

        /// <summary>
        /// Gets the previous keyboard state.
        /// </summary>
        public static KeyboardState PreviousKeyboardState
        {
            get { return previousKeyboardState; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public TheCavesOfKardun()
        {
            TheCavesOfKardun.TileWidth = 96;
            TheCavesOfKardun.TileHeight = 96;

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

            this.minimapRenderTarget = new RenderTarget2D(GraphicsDevice, 200, 200, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per 
        /// and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            this.level = new Level(this.Content, new Point(75, 75), null, 20, 100);
            this.level.LoadContent();

            this.hoverTexture = Content.Load<Texture2D>("Textures/Hover");

            this.player = new Player(Content.Load<Texture2D>("Textures/Characters/player"), new Vector2(
                this.level.Rooms[this.level.RoomSpawnIndex].Center.X * TheCavesOfKardun.TileWidth,
                this.level.Rooms[this.level.RoomSpawnIndex].Center.Y * TheCavesOfKardun.TileHeight), 500, 5,
                Content.Load<SpriteFont>("Fonts/combatFont"));
            this.player.GodMode = true;
            this.player.LoadContent(Content, new Vector2(GraphicsDevice.Viewport.Width - 248, GraphicsDevice.Viewport.Height - 248), new Vector2(0, GraphicsDevice.Viewport.Height - 248));
            this.level.Player = this.player;
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
            TheCavesOfKardun.currentKeyboardState = Keyboard.GetState();
            TheCavesOfKardun.currentMouseState = Mouse.GetState();

            if (TheCavesOfKardun.CurrentKeyboardState.IsKeyDown(Keys.Enter) && TheCavesOfKardun.PreviousKeyboardState.IsKeyUp(Keys.Enter))
            {
                this.level.RecreateLevel();
                this.player.Position = new Vector2(
                    this.level.Rooms[this.level.RoomSpawnIndex].Center.X * TheCavesOfKardun.TileWidth,
                    this.level.Rooms[this.level.RoomSpawnIndex].Center.Y * TheCavesOfKardun.TileHeight);

                this.level.ResetMonstersSpawn();
            }

            if (TheCavesOfKardun.CurrentKeyboardState.IsKeyDown(Keys.Escape) && TheCavesOfKardun.PreviousKeyboardState.IsKeyUp(Keys.Escape))
                Exit();

            UpdateGameplay(gameTime);

            this.player.Update(gameTime);

            if (!this.player.Alive)
                Exit();

            TheCavesOfKardun.previousKeyboardState = TheCavesOfKardun.currentKeyboardState;
            TheCavesOfKardun.previousMouseState = TheCavesOfKardun.currentMouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (updateMiniMap)
            {
                this.minimapTexture = DrawMiniMap(gameTime);
                this.updateMiniMap = false;
            }

            this.cameraPosition.X = this.player.Center.X - GraphicsDevice.Viewport.Width / 2;
            this.cameraPosition.Y = this.player.Center.Y - GraphicsDevice.Viewport.Height / 2;

            GraphicsDevice.Clear(Color.Black);
            this.level.Draw(gameTime, spriteBatch, cameraPosition, this.player.Position, TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition), TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition + new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) + new Vector2(TheCavesOfKardun.TileWidth)));

            spriteBatch.Begin();
            spriteBatch.Draw(this.hoverTexture, Vector2.Zero, Color.White);
            spriteBatch.End();

            this.player.Draw(gameTime, spriteBatch, cameraPosition);

            spriteBatch.Begin();
            spriteBatch.Draw(this.minimapTexture, Vector2.Zero, Color.White * 0.5f);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper for drawing the mini map.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <returns></returns>
        private Texture2D DrawMiniMap(GameTime gameTime)
        {
            
            Point playerCoords = TheCavesOfKardun.ConvertPositionToCell(this.player.Center);
            List<Point> monsterCoords = new List<Point>();
            for (int i = 0; i < this.level.Monsters.Count; i++)
                monsterCoords.Add(TheCavesOfKardun.ConvertPositionToCell(this.level.Monsters[i].Center));

            TheCavesOfKardun.TileWidth = 8;
            TheCavesOfKardun.TileHeight = 8;

            this.player.Position = TheCavesOfKardun.ConvertCellToPosition(playerCoords);
            for (int i = 0; i < this.level.Monsters.Count; i++)
                this.level.Monsters[i].Position = TheCavesOfKardun.ConvertCellToPosition(monsterCoords[i]);
            

            TheCavesOfKardun.TileWidth = 8;
            TheCavesOfKardun.TileHeight = 8;

            GraphicsDevice.SetRenderTarget(this.minimapRenderTarget);

            this.cameraPosition.X = this.player.Center.X - this.minimapRenderTarget.Width / 2;
            this.cameraPosition.Y = this.player.Center.Y - this.minimapRenderTarget.Height / 2;

            GraphicsDevice.Clear(Color.Transparent);
            this.level.Draw(gameTime, spriteBatch, cameraPosition, this.player.Position, TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition), TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition + new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) + new Vector2(TheCavesOfKardun.TileWidth)));
            this.player.Draw(gameTime, spriteBatch, cameraPosition);

            GraphicsDevice.SetRenderTarget(null);
            
            TheCavesOfKardun.TileWidth = 96;
            TheCavesOfKardun.TileHeight = 96;

            this.player.Position = TheCavesOfKardun.ConvertCellToPosition(playerCoords);
            for (int i = 0; i < this.level.Monsters.Count; i++)
                this.level.Monsters[i].Position = TheCavesOfKardun.ConvertCellToPosition(monsterCoords[i]);            

            return (Texture2D)this.minimapRenderTarget;
        }

        /// <summary>
        /// Handles movement for the player.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void UpdateGameplay(GameTime gameTime)
        {
            bool updateMiniMap = true;

            for (int i = 0; i < this.level.Monsters.Count; i++)
                if (this.level.Monsters[i].UpdateMovement(gameTime))
                    updateMiniMap = false;

            if (this.player.UpdateMovement(gameTime))
                return;

            if (updateMiniMap)
                this.updateMiniMap = true;

            Point targetTile = Point.Zero;

            // Reset the motion.
            Vector2 motion = Vector2.Zero;
            int amountOfTiles = 0;
            if (TheCavesOfKardun.CurrentMouseState.LeftButton == ButtonState.Pressed && TheCavesOfKardun.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                // Check if we pressed the left mouse button.

                // Convert the players position to the playerTile and the targetTile from the mouse position.
                targetTile = TheCavesOfKardun.ConvertPositionToCell(new Vector2(TheCavesOfKardun.CurrentMouseState.X + this.cameraPosition.X, TheCavesOfKardun.CurrentMouseState.Y + this.cameraPosition.Y));
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
                Item item;
                
                if (this.level.CanWalk(this.player, motion, amountOfTiles, out this.player.TargetPosition)) // Check if we can walk.
                {
                    this.player.Motion = motion;
                    this.level.UpdateMonstersAI(gameTime, this.player);
                }
                else if (targetTile != Point.Zero && amountOfTiles == 1 && this.level.EncounterMonster(targetTile, out monster)) // Check if we've clicked on a monster.
                {
                    // We can't spam kill him, wait for the combat text to fade out.
                    if (string.IsNullOrWhiteSpace(this.player.CombatText))
                    {
                        this.player.Attacks = monster;
                        this.level.UpdateMonstersAI(gameTime, this.player);
                        this.player.Attack(gameTime, monster);
                        this.player.Attacks = null;
                    }
                }
                else if (targetTile != Point.Zero && amountOfTiles == 1 && this.level.EncounterItem(targetTile, out item)) // Check if we clicked on an item.
                {
                    if (this.player.PickUp(item))
                    {
                        this.level.RemoveItemFromTile(targetTile);
                        this.level.UpdateMonstersAI(gameTime, this.player);
                    }
                }
            }
        }

        #endregion
    }
}