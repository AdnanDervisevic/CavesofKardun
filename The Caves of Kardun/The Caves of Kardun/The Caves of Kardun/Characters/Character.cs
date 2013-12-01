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

        private Texture2D texture;

        public Vector2 Motion;
        public Vector2 Position;
        public Vector2 TargetPosition;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets he speed of the character.
        /// </summary>
        public float Speed { get; set; }

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
        public Character(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            this.Position = position;
            this.Speed = speed;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws the character.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used to draw the character.</param>
        /// <param name="cameraPosition">The camera position.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            /*
            spriteBatch.Begin();
            spriteBatch.Draw(this.texture,
                new Rectangle(
                    (int)(this.Position.X - cameraPosition.X),
                    (int)(this.Position.Y - cameraPosition.Y),
                    TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight), Color.White);
            spriteBatch.End();
            */

            spriteBatch.Begin();
            spriteBatch.Draw(this.texture,
                new Rectangle(
                    (int)(this.Position.X - cameraPosition.X),
                    (int)(this.Position.Y - cameraPosition.Y),
                    TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight), Color.White);
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
