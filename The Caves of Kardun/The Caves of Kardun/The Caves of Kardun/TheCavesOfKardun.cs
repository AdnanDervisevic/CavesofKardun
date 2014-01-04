#region File Description
    //////////////////////////////////////////////////////////////////////////
   // TheCavesOfKardun                                                     //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

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
#endregion End of Using Statements

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TheCavesOfKardun : Game
    {
        #region Consts

        public const int SCREENWIDTH = 1280;
        public const int SCREENHEIGHT = 720;
        public const bool FULLSCREEN = false;

        #endregion

        #region Fields

        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        private bool gameStarted;

        private SpriteFont deathScreenFont;

        private Texture2D startScreenTexture;
        private Texture2D deathScreenTexture;
        private Texture2D hoverTexture;
        private Texture2D leftEyeTexture;
        private Texture2D rightEyeTexture;

        private Vector2 deathScreenScorePosition;

        private Vector2 cameraPosition;
        public Player player;
        private Level level;

        private Texture2D traitsTexture;
        private Rectangle traitsPosition;

        private Effect blurEffect;
        private Effect grayscaleEffect;

        private RenderTarget2D defaultRenderTarget;
        private RenderTarget2D minimapRenderTarget;
        private Texture2D minimapTexture;
        private bool updateMiniMap;

        private static MouseState currentMouseState;
        private static MouseState previousMouseState;

        private static KeyboardState currentKeyboardState;
        private static KeyboardState previousKeyboardState;

        private SoundEffectInstance instancebackgroundMusic;
        private SoundEffect soundBackgroundMusic;
        private SoundEffect soundCoinPickup;
        private SoundEffect soundItemPickup;
        private SoundEffect soundSwordAttack;

        private Random random = new Random();

        private HealthBar playerHealthBar;
        private HealthBar enemyHealthBar;

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

        /// <summary>
        /// player returned for traits purposes
        /// </summary>
        public Player pC
        {
            get { return player; }
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
            graphicsDeviceManager.PreferredBackBufferWidth = TheCavesOfKardun.SCREENWIDTH;
            graphicsDeviceManager.PreferredBackBufferHeight = TheCavesOfKardun.SCREENHEIGHT;
            graphicsDeviceManager.IsFullScreen = TheCavesOfKardun.FULLSCREEN;
            graphicsDeviceManager.ApplyChanges();
            IsMouseVisible = true;
        }

        #endregion

        #region Public Methods

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
            this.defaultRenderTarget = new RenderTarget2D(GraphicsDevice, TheCavesOfKardun.SCREENWIDTH, TheCavesOfKardun.SCREENHEIGHT, true, GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);

            this.deathScreenScorePosition = new Vector2(600, GraphicsDevice.Viewport.Height - 280);
            this.updateMiniMap = true;
            this.traitsPosition = new Rectangle(150, GraphicsDevice.Viewport.Height - 80, 167, 76);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game
        /// and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Tooltip.Initialize(GraphicsDevice, Content.Load<SpriteFont>("Fonts/titleFont"), Content.Load<SpriteFont>("Fonts/textFont"));
            this.level = new Level(this.Content, new Point(75, 75), null, 20, 5, 100);
            this.level.LoadContent();

            this.startScreenTexture = Content.Load<Texture2D>("titleScreen");
            this.deathScreenTexture = Content.Load<Texture2D>("deathScreen");
            this.hoverTexture = Content.Load<Texture2D>("Textures/Hover");
            this.leftEyeTexture = Content.Load<Texture2D>("Textures/HoverLeft");
            this.rightEyeTexture = Content.Load<Texture2D>("Textures/HoverRight");
            this.traitsTexture = Content.Load<Texture2D>("Textures/traits");

            this.deathScreenFont = Content.Load<SpriteFont>("Fonts/deathScreenFont");

            this.player = new Player(Content.Load<Texture2D>("Textures/Characters/player"), new Vector2(
                this.level.Rooms[this.level.RoomSpawnIndex].Center.X * TheCavesOfKardun.TileWidth,
                this.level.Rooms[this.level.RoomSpawnIndex].Center.Y * TheCavesOfKardun.TileHeight), 500, 10,
                Content.Load<SpriteFont>("Fonts/combatFont"));
            this.player.LoadContent(Content, new Vector2(GraphicsDevice.Viewport.Width - 248, GraphicsDevice.Viewport.Height - 248), new Vector2(0, GraphicsDevice.Viewport.Height - 248));
            this.level.Player = this.player;

            this.playerHealthBar = new HealthBar(this.player, new Vector2(10, 10), true);
            this.playerHealthBar.LoadContent(Content);

            this.enemyHealthBar = new HealthBar(null, new Vector2(GraphicsDevice.Viewport.Width, 10), false);
            this.enemyHealthBar.LoadContent(Content);

            this.soundBackgroundMusic = Content.Load<SoundEffect>("Audio\\music");
            this.soundCoinPickup = Content.Load<SoundEffect>("Audio\\coinPickup");
            this.soundItemPickup = Content.Load<SoundEffect>("Audio\\itemPickUp");
            this.soundSwordAttack = Content.Load<SoundEffect>("Audio\\swordAttack");

            instancebackgroundMusic = soundBackgroundMusic.CreateInstance();
            instancebackgroundMusic.Volume = 0.1f;
            instancebackgroundMusic.IsLooped = true;
            instancebackgroundMusic.Play();

            this.grayscaleEffect = Content.Load<Effect>("Shaders/Grayscale");
            this.blurEffect = Content.Load<Effect>("Shaders/Blur");
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            TheCavesOfKardun.currentKeyboardState = Keyboard.GetState();
            TheCavesOfKardun.currentMouseState = Mouse.GetState();

            // Escape to exit.
            if (TheCavesOfKardun.CurrentKeyboardState.IsKeyDown(Keys.Escape) && TheCavesOfKardun.PreviousKeyboardState.IsKeyUp(Keys.Escape))
                Exit();

            if (!this.gameStarted)
            {
                if (TheCavesOfKardun.CurrentKeyboardState.IsKeyDown(Keys.Enter) && TheCavesOfKardun.PreviousKeyboardState.IsKeyUp(Keys.Enter))
                {
                    this.player.Reset();
                    this.gameStarted = true;
                }
            }
            else
            {
                UpdateGameplay(gameTime);
                this.player.Update(gameTime);

                Rectangle mousePos = new Rectangle(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y, 1, 1);
                for (int i = 0; i < this.level.Monsters.Count; i++)
                {
                    if (mousePos.Intersects(this.level.Monsters[i].GetMonsterBounds(this.cameraPosition)))
                    {
                        this.enemyHealthBar.Character = this.level.Monsters[i];
                        break;
                    }
                }

                if (!this.player.Alive)
                {
                    this.gameStarted = false;
                    this.level.ResetGame();
                    this.enemyHealthBar.Character = null;
                    this.player.Position = new Vector2(
                            this.level.Rooms[this.level.RoomSpawnIndex].Center.X * TheCavesOfKardun.TileWidth,
                            this.level.Rooms[this.level.RoomSpawnIndex].Center.Y * TheCavesOfKardun.TileHeight);
                    this.player.RandomTraits();
                }

                bool showOnce = false;
                if (this.traitsPosition.Intersects(new Rectangle(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y, 1, 1)))
                {
                    Tooltip.Show(this.player.PositiveTraits, this.player.NegativeTraits);
                    showOnce = true;
                }
                else
                {
                    if (showOnce)
                        Tooltip.Hide();
                }
            }

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
            if (!this.gameStarted)
            {
                spriteBatch.Begin();

                if (this.player.Alive)
                    spriteBatch.Draw(this.startScreenTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                else
                {
                    spriteBatch.Draw(this.deathScreenTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.DrawString(this.deathScreenFont, this.player.Inventory.Gold.ToString(), deathScreenScorePosition, Color.White);
                }

                spriteBatch.End();
            }
            else
            {
                if (updateMiniMap)
                {
                    this.minimapTexture = DrawMiniMap(gameTime);
                    this.updateMiniMap = false;
                }

                this.cameraPosition.X = this.player.Center.X - GraphicsDevice.Viewport.Width / 2;
                this.cameraPosition.Y = this.player.Center.Y - GraphicsDevice.Viewport.Height / 2;

                GraphicsDevice.SetRenderTarget(this.defaultRenderTarget);
                GraphicsDevice.Clear(Color.Black);

                if ((this.player.NegativeTraits & NegativeTraits.ColourBlind) == NegativeTraits.ColourBlind &&
                    (this.player.Equipment.Helmet == null || this.player.Equipment.Helmet.Special != ItemSpecials.Colour))
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, grayscaleEffect);
                else
                    spriteBatch.Begin();

                this.level.Draw(gameTime, spriteBatch, cameraPosition, this.player.Position, TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition), TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition + new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) + new Vector2(TheCavesOfKardun.TileWidth)));

                for (int i = 0; i < this.level.Monsters.Count; i++)
                    this.level.Monsters[i].Draw(gameTime, spriteBatch, cameraPosition);

                spriteBatch.End();

                // Om spelaren inte har synproblem eller saknar ett öga samt har optics, rita inte ut hover.
                if (((this.player.NegativeTraits & NegativeTraits.NearSighted) != NegativeTraits.NearSighted) &&
                    ((this.player.NegativeTraits & NegativeTraits.MissingAnEye) != NegativeTraits.MissingAnEye) &&
                    (this.player.Equipment.Helmet != null && this.player.Equipment.Helmet.Special == ItemSpecials.Sight) || 
                    ((this.player.PositiveTraits & PositiveTraits.TwentyTwentyVision) == PositiveTraits.TwentyTwentyVision))
                { }
                else
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(this.hoverTexture, Vector2.Zero, Color.White);

                    int eyeSide = random.Next(0, 1);

                    if ((this.player.NegativeTraits & NegativeTraits.MissingAnEye) == NegativeTraits.MissingAnEye)
                    {
                        if (eyeSide == 0)
                            spriteBatch.Draw(this.leftEyeTexture, Vector2.Zero, Color.White);
                        else
                            spriteBatch.Draw(this.leftEyeTexture, Vector2.Zero, Color.White);
                    }

                   spriteBatch.End();
                }

                if ((this.player.NegativeTraits & NegativeTraits.ColourBlind) == NegativeTraits.ColourBlind &&
                    (this.player.Equipment.Helmet == null || this.player.Equipment.Helmet.Special != ItemSpecials.Colour))
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, grayscaleEffect);
                else
                    spriteBatch.Begin();

                this.player.Draw(gameTime, spriteBatch, cameraPosition);

                if (this.player.Equipment.Helmet != null && this.player.Equipment.Helmet.Special == ItemSpecials.Minimap)
                    spriteBatch.Draw(this.minimapTexture, Vector2.Zero, Color.White * 0.5f);

                spriteBatch.Draw(this.traitsTexture, traitsPosition, Color.White);

                Tooltip.Draw(spriteBatch);
                spriteBatch.End();

                if ((this.player.NegativeTraits & NegativeTraits.ColourBlind) == NegativeTraits.ColourBlind &&
                    (this.player.Equipment.Helmet == null || this.player.Equipment.Helmet.Special != ItemSpecials.Colour))
                {
                    playerHealthBar.Draw(spriteBatch, grayscaleEffect);
                    enemyHealthBar.Draw(spriteBatch, grayscaleEffect);
                }
                else
                {
                    playerHealthBar.Draw(spriteBatch, null);
                    enemyHealthBar.Draw(spriteBatch, null);
                }

                GraphicsDevice.SetRenderTarget(null);

                if ((this.player.NegativeTraits & NegativeTraits.NearSighted) == NegativeTraits.NearSighted &&
                    (this.player.Equipment.Helmet == null || this.player.Equipment.Helmet.Special != ItemSpecials.Sight))
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, blurEffect);
                else
                    spriteBatch.Begin();

                spriteBatch.Draw((Texture2D)this.defaultRenderTarget, Vector2.Zero, Color.White);
                spriteBatch.End();
            }

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

            spriteBatch.Begin();
            this.level.Draw(gameTime, spriteBatch, cameraPosition, this.player.Position, TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition), TheCavesOfKardun.ConvertPositionToCell(this.cameraPosition + new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) + new Vector2(TheCavesOfKardun.TileWidth)));
            
            for (int i = 0; i < this.level.Monsters.Count; i++)
                this.level.Monsters[i].Draw(gameTime, spriteBatch, cameraPosition);

            this.player.Draw(gameTime, spriteBatch, cameraPosition);
            spriteBatch.End();

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
                
                if (targetTile != Point.Zero && amountOfTiles == 1 && this.level.EncounterMonster(targetTile, out monster)) // Check if we've clicked on a monster.
                {
                    // We can't spam kill him, wait for the combat text to fade out.
                    if (string.IsNullOrWhiteSpace(this.player.CombatText))
                    {
                        this.player.Attacks = monster;
                        this.level.UpdateMonstersAI(gameTime, this.player);
                        this.player.Attack(gameTime, monster);

                        this.soundSwordAttack.Play();
                        
                        if (monster.Boss == true && !monster.Alive)
                            this.level.DropBossLoot();

                        this.player.Attacks = null;
                    }
                }
                else if (targetTile != Point.Zero && amountOfTiles == 1 && this.level.EncounterItem(targetTile, out item)) // Check if we clicked on an item.
                {
                    if (item.Type == ItemTypes.Ladder)
                    {
                        this.level.NextLevel();
                        this.enemyHealthBar.Character = null;
                        this.player.Position = new Vector2(
                            this.level.Rooms[this.level.RoomSpawnIndex].Center.X * TheCavesOfKardun.TileWidth,
                            this.level.Rooms[this.level.RoomSpawnIndex].Center.Y * TheCavesOfKardun.TileHeight);
                    }
                    else if (this.player.PickUp(item))
                    {
                        this.level.RemoveItemFromTile(targetTile);
                        if (item.Type == ItemTypes.Gold)
                            this.soundCoinPickup.Play();
                        else
                            this.soundItemPickup.Play();

                        this.level.UpdateMonstersAI(gameTime, this.player);
                    }
                }
                else if (this.level.CanWalk(this.player, motion, amountOfTiles, out this.player.TargetPosition)) // Check if we can walk.
                {
                    if ((this.player.NegativeTraits & NegativeTraits.SenseOfDirection) == NegativeTraits.SenseOfDirection)
                    {
                        if (motion.X == 1)
                            motion.X = -1;
                        else if (motion.X == -1)
                            motion.X = 1;
                        else if (motion.Y == 1)
                            motion.Y = -1;
                        else if (motion.Y == -1)
                            motion.Y = 1;
                    }

                    this.player.Motion = motion;
                    this.level.UpdateMonstersAI(gameTime, this.player);
                }
            }
        }

        #endregion
    }
}