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

        protected Texture2D texture;

        public Vector2 Motion;
        public Vector2 Position;
        public Vector2 TargetPosition;

        private string combatText;
        private float alphaValue = 1;
        private float fadeDelay = .020f;
        private Vector2 combatTextPosition;
        private SpriteFont combatFont;

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
        public string CombatText
        {
            get { return this.combatText; }
            set
            {
                this.combatText = value;
                if (!string.IsNullOrWhiteSpace(this.combatText))
                {
                    Vector2 measure = this.combatFont.MeasureString(this.CombatText);
                    this.combatTextPosition = new Vector2((this.Position.X + TheCavesOfKardun.TileWidth / 2) - measure.X / 2, this.Position.Y + TheCavesOfKardun.TileHeight - TheCavesOfKardun.TileHeight / 3);
                    this.alphaValue = 1;
                    this.fadeDelay = .035f;
                }
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
            spriteBatch.Begin();

            // If we're alive then draw our texture.
            if (this.Health > 0)
            {
                spriteBatch.Draw(this.texture,
                new Rectangle(
                    (int)(this.Position.X - cameraPosition.X),
                    (int)(this.Position.Y - cameraPosition.Y),
                    TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight), Color.White);
            }            

            // If combatText is not set to empty or whitespaces only draw it.
            if (!string.IsNullOrWhiteSpace(this.CombatText))
            {
                // Fade out the text.
                fadeDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (fadeDelay <= 0)
                {
                    fadeDelay = .035f;
                    alphaValue -= .08f;
                }

                // Move the combat text position.
                this.combatTextPosition.Y += -100f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Draw the combat text.
                DrawCombatText(spriteBatch, cameraPosition);

                // If we've faded out then set the combat text to empty.
                if (alphaValue <= 0)
                {
                    this.CombatText = string.Empty;
                }
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Helper for drawing the combat text and its outline.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used to draw.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        private void DrawCombatText(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {

            // Draws the text five times in different locations to make it look like an outline effect.
            Vector2 targetPosition = new Vector2(this.combatTextPosition.X, this.combatTextPosition.Y);
            Color color = Color.Black * MathHelper.Clamp(alphaValue, 0, 1);

            targetPosition.X += 1;
            targetPosition.Y += 1;
            spriteBatch.DrawString(this.combatFont, this.CombatText, targetPosition - cameraPosition, color);

            targetPosition.Y -= 2;
            spriteBatch.DrawString(this.combatFont, this.CombatText, targetPosition - cameraPosition, color);

            targetPosition.X -= 2;
            targetPosition.Y += 2;
            spriteBatch.DrawString(this.combatFont, this.CombatText, targetPosition - cameraPosition, color);

            targetPosition.Y -= 2;
            spriteBatch.DrawString(this.combatFont, this.CombatText, targetPosition - cameraPosition, color);

            color = new Color(227, 66, 52) * MathHelper.Clamp(alphaValue, 0, 1);
            spriteBatch.DrawString(this.combatFont, this.CombatText, this.combatTextPosition - cameraPosition, color);

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
