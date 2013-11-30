#region File Description
//////////////////////////////////////////////////////////////////////////
// Player                                                         //
//                                                                      //
// Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
#endregion End of Using Statements

namespace The_Caves_of_Kardun
{
    public sealed class Player
    {
        #region Fields

        private Texture2D texture;

        public Vector2 Motion;
        public Vector2 Position;
        public Vector2 TargetPosition;

        #endregion

        #region Properties

        public bool Attacks { get; set; }

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

        #endregion

        #region Constructors

        public Player(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            this.Position = position;
            this.Speed = speed;
        }

        #endregion

        #region Public Methods

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(this.texture, 
                new Rectangle(
                    (int)(this.Position.X - cameraPosition.X),
                    (int)(this.Position.Y - cameraPosition.Y),
                    TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight), Color.White);
            spriteBatch.End();
        }

        #endregion
    }
}