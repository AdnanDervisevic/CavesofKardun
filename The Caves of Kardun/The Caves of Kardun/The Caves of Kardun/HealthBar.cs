#region File Description
/*
 * HealthBar
 * 
 * Copyright (C) Untitled. All Rights Reserved.
 */
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
#endregion

namespace The_Caves_of_Kardun
{
    public sealed class HealthBar
    {
        #region Fields

        private SpriteFont titleFont;
        private SpriteFont textFont;

        private Texture2D healthBar;
        private Texture2D blackTexture;
        private Texture2D healthBarBorder;

        private Vector2 position;
        private RasterizerState rasterizerState;

        private bool left;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the character associated with this health bar.
        /// </summary>
        public Character Character { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new health bar.
        /// </summary>
        /// <param name="character">The character associated with this health bar.</param>
        /// <param name="position">The position of this health bar.</param>
        public HealthBar(Character character, Vector2 position, bool left)
        {
            this.rasterizerState = new RasterizerState() { ScissorTestEnable = true };
            this.Character = character;
            this.position = position;
            this.left = left;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads all the content.
        /// </summary>
        /// <param name="content">The content manager used to load content.</param>
        public void LoadContent(ContentManager content)
        {
            this.blackTexture = content.Load<Texture2D>("Textures/blackTexture");
            this.healthBarBorder = content.Load<Texture2D>("Textures/healthBarBorder");
            this.healthBar = content.Load<Texture2D>("Textures/healthBar");
            this.titleFont = content.Load<SpriteFont>("Fonts/healthBarTitle");
            this.textFont = content.Load<SpriteFont>("Fonts/healthBarText");
        }

        /// <summary>
        /// Draws the health bar.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used to draw.</param>
        public void Draw(SpriteBatch spriteBatch, Effect effect)
        {
            if (Character == null)
                return;

            StringBuilder sb = new StringBuilder(this.Character.CurrentHealth.ToString());
            sb.Append("/");
            sb.Append(this.Character.Health);
            sb.Append(" Health");

            Vector2 barPosition = Vector2.Zero;
            Vector2 namePosition = Vector2.Zero;

            if (left)
            {
                barPosition = this.position;
                barPosition.X += this.titleFont.MeasureString(this.Character.Name).X + 10;
                barPosition.Y += 10;

                namePosition = this.position;

            }
            else
            {
                int width = (int)this.titleFont.MeasureString(this.Character.Name).X + 20 + 204;

                barPosition = this.position;
                barPosition.X -= width;
                barPosition.Y += 10;

                namePosition = this.position;
                namePosition.X -= width - 10 - 204;
            }

            Vector2 textPosition = new Vector2(barPosition.X + 100 - this.textFont.MeasureString(sb.ToString()).X / 2, barPosition.Y + 20);
            Vector2 healthPosition = new Vector2(barPosition.X + 2 - ((1 - (this.Character.CurrentHealth / (float)this.Character.Health)) * 240), barPosition.Y + 2);

            Rectangle currentRect = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle((int)barPosition.X + 2, (int)barPosition.Y, 200, 16);

            if (effect != null)
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect);
            else
                spriteBatch.Begin();

            spriteBatch.DrawString(this.titleFont, this.Character.Name, namePosition, Color.White);
            spriteBatch.DrawString(this.textFont, sb.ToString(), textPosition, Color.White);
            spriteBatch.Draw(this.healthBarBorder, barPosition, Color.White);
            spriteBatch.Draw(this.blackTexture, new Rectangle((int)barPosition.X + 2, (int)barPosition.Y + 2, 200, 12), Color.White);
            spriteBatch.End();

            if (effect != null)
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState, effect);
            else
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizerState);

            spriteBatch.Draw(this.healthBar, healthPosition, Color.White);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
        }
        #endregion
    }
}