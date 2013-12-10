#region File Description
    //////////////////////////////////////////////////////////////////////////
   // Character                                                            //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion End of Using Statements

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
        private int baseHealth;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the health of the character.
        /// </summary>
        public virtual int Health
        {
            get { return this.baseHealth; }
        }

        /// <summary>
        /// Gets the total damage taken to this character.
        /// </summary>
        public int DamageTaken { get; set; }

        /// <summary>
        /// Determines if the character is alive.
        /// </summary>
        public bool Alive
        {
            get 
            {
                if (GodMode)
                    return true;

                return Health > DamageTaken; 
            }
        }

        /// <summary>
        /// Gets or sets whether this character should be able to die.
        /// </summary>
        public bool GodMode { get; set; }

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
                    this.Position.X + TheCavesOfKardun.TileWidth / 2,
                    this.Position.Y + TheCavesOfKardun.TileHeight / 2);
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
                    this.fadeDelay = .020f;
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
        public Character(Texture2D texture, Vector2 position, float speed, int baseHealth, SpriteFont combatFont)
        {
            this.texture = texture;
            this.Position = position;
            this.Speed = speed;
            this.DamageTaken = 0;
            this.baseHealth = baseHealth;
            this.combatFont = combatFont;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inflicts damage to this character.
        /// </summary>
        /// <param name="damage">The damage to inflict.</param>
        public void InflictDamage(int damage)
        {
            this.DamageTaken += damage;
            this.CombatText = "-" + damage + " Damage";
        }

        /// <summary>
        /// Draws the character.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used to draw the character.</param>
        /// <param name="cameraPosition">The camera position.</param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            // If we're alive then draw our texture.
            if (this.Alive)
            {
                spriteBatch.Draw(this.texture, new Rectangle(
                    (int)(this.Position.X - cameraPosition.X),
                    (int)(this.Position.Y - cameraPosition.Y),
                    TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight), Color.White);
            }

            UpdateCombatText(gameTime, spriteBatch, cameraPosition);
        }

        /// <summary>
        /// Updates the combat text and draws it.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">The spritebatch used to draw.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        protected void UpdateCombatText(GameTime gameTime, SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            // If combatText is not set to empty or whitespaces only draw it.
            if (!string.IsNullOrWhiteSpace(this.CombatText))
            {
                // Fade out the text.
                fadeDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (fadeDelay <= 0)
                {
                    fadeDelay = .035f;
                    alphaValue -= .06f;
                }

                // Move the combat text position.
                this.combatTextPosition.Y += -75f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Draw the combat text.
                DrawCombatText(spriteBatch, cameraPosition);

                // If we've faded out then set the combat text to empty.
                if (alphaValue <= 0)
                {
                    this.CombatText = string.Empty;
                }
            }
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

            color = new Color(34, 133, 255) * MathHelper.Clamp(alphaValue, 0, 1);
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