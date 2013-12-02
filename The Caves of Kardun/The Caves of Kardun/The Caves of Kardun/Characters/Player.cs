#region File Description
    //////////////////////////////////////////////////////////////////////////
   // Player                                                               //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion End of Using Statements

namespace The_Caves_of_Kardun
{
    public sealed class Player : Character
    {
        #region Properties

        /// <summary>
        /// Gets or sets the sword this player is wealding.
        /// </summary>
        public Item Sword { get; set; }

        /// <summary>
        /// Gets or sets the shield this player is wealding.
        /// </summary>
        public Item Shield { get; set; }

        /// <summary>
        /// Gets the amount of tiles to move.
        /// </summary>
        public override int AmountOfTilesToMove
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the monster the player is attacking.
        /// </summary>
        public Monster AttacksMonster { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="texture">The texture of the player.</param>
        /// <param name="position">The position of the player.</param>
        /// <param name="speed">The movement speed of the player.</param>
        public Player(Texture2D texture, Vector2 position, float speed, int health, SpriteFont combatFont)
            : base(texture, position, speed, health, combatFont) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attack a monster.
        /// </summary>
        /// <param name="monster">The monster to attack.</param>
        public void Attack(GameTime gameTime, Monster monster)
        {
            monster.CombatText = "-30 Damage";
            monster.Health -= 30;
            this.AttacksMonster = monster;

            // If the monster dies then we're not attacking a monster anymore.
            if (monster.Health <= 0)
                this.AttacksMonster = null;
        }

        /// <summary>
        /// Pick up an item.
        /// </summary>
        /// <param name="typeOfObject">The object to pick up.</param>
        public void PickUp(Objects typeOfObject)
        {
        }

        #endregion
    }
}