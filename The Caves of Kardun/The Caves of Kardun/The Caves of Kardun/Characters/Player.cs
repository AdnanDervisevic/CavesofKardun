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
    public sealed class Player : Character
    {
        #region Properties

        /// <summary>
        /// Gets the amount of tiles to move.
        /// </summary>
        public override int AmountOfTilesToMove
        {
            get { return 2; }
        }

        #endregion

        #region Constructors

        public Player(Texture2D texture, Vector2 position, float speed)
            : base(texture, position, speed) { }

        #endregion
    }
    
}