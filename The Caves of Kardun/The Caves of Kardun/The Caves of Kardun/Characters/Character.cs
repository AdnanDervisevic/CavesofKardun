using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Caves_of_Kardun
{
    public class Character
    {
        #region Fields

        private SpriteFont combatFont;
        private float timer = 0;

        protected Texture2D texture;

        public Vector2 Motion;
        public Vector2 Position;
        public Vector2 TargetPosition;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the health of the character.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Gets or sets he speed of the character.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets the center position of the character.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(
                    this.Position.X + texture.Width / 2,
                    this.Position.Y + texture.Height / 2);
            }
        }

        /// <summary>
        /// Gets or sets the combat text.
        /// </summary>
        public string CombatText { get; set; }

        /// <summary>
        /// Gets the position where to position the combat text.
        /// </summary>
        public Vector2 CombatTextPosition
        {
            get
            {
                Vector2 measure = this.combatFont.MeasureString(this.CombatText);
                return new Vector2((this.Position.X + TheCavesOfKardun.TileWidth / 2) - measure.X / 2, this.Position.Y + 30);
            }
        }

        /// <summary>
        /// Gets the amount of tiles to move.
        /// </summary>
        public virtual int AmountOfTilesToMove { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates the character.
        /// </summary>
        /// <param name="texture">The character texture.</param>
        /// <param name="position">The character position.</param>
        /// <param name="speed">The speed of the character.</param>
        public Character(Texture2D texture, Vector2 position, float speed, int health, SpriteFont combatFont)
        {
            this.texture = texture;
            this.Position = position;
            this.Speed = speed;
            this.Health = health;
            this.combatFont = combatFont;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws the character.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used to draw the character.</param>
        /// <param name="cameraPosition">The camera position.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            if (this.Health <= 0)
                return;

            spriteBatch.Begin();
            spriteBatch.Draw(this.texture,
                new Rectangle(
                    (int)(this.Position.X - cameraPosition.X),
                    (int)(this.Position.Y - cameraPosition.Y),
                    TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight), Color.White);

            if (!string.IsNullOrWhiteSpace(this.CombatText))
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                spriteBatch.DrawString(this.combatFont, this.CombatText, 
                    new Vector2(
                        this.CombatTextPosition.X - cameraPosition.X, 
                        this.CombatTextPosition.Y - cameraPosition.Y),
                    Color.Red);

                if (timer >= 1)
                {
                    timer = 0;
                    this.CombatText = string.Empty;
                }
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Updates the animation of the character.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <returns>Returns true if we've landed on our target position or if we're still moving; otherwise false.</returns>
        public bool UpdateMovement(GameTime gameTime)
        {
            // Move the character
            this.Position.X += this.Motion.X * this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.Position.Y += this.Motion.Y * this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if we're moving and if we've landed on our target position.
            if (this.Motion.X == 1 && this.Motion.Y == 0)
            {
                if (this.Position.X >= this.TargetPosition.X)
                {
                    this.Position.X = this.TargetPosition.X;
                    this.Motion = Vector2.Zero;
                    return true;
                }
            }
            else if (this.Motion.X == -1 && this.Motion.Y == 0)
            {
                if (this.Position.X <= this.TargetPosition.X)
                {
                    this.Position.X = this.TargetPosition.X;
                    this.Motion = Vector2.Zero;
                    return true;
                }
            }
            else if (this.Motion.X == 0 && this.Motion.Y == 1)
            {
                if (this.Position.Y >= this.TargetPosition.Y)
                {
                    this.Position.Y = this.TargetPosition.Y;
                    this.Motion = Vector2.Zero;
                    return true;
                }
            }
            else if (this.Motion.X == 0 && this.Motion.Y == -1)
            {
                if (this.Position.Y <= this.TargetPosition.Y)
                {
                    this.Position.Y = this.TargetPosition.Y;
                    this.Motion = Vector2.Zero;
                    return true;
                }
            }

            // Check if we're not moving.
            if (this.Motion != Vector2.Zero)
                return true;

            // Returns false if we're not moving or if we have not landed on our target position.
            return false;
        }

        #endregion
    }
}
