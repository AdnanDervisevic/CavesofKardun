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
        #region Fields

        private Random random;
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the health of the player.
        /// </summary>
        public override int Health
        {
            get
            {
                int bonusHealth = 0;
                if (this.Helmet != null)
                    bonusHealth += this.Helmet.Health;

                if (this.LeftHand != null)
                    bonusHealth += this.LeftHand.Health;

                if (this.RightHand != null)
                    bonusHealth += this.RightHand.Health;

                if (this.Boots != null)
                    bonusHealth += this.Boots.Health;

                return base.Health + bonusHealth;
            }
        }

        /// <summary>
        /// Gets the damage that the player should take on his enemy.
        /// </summary>
        public int Damage
        {
            get
            {
                // Check if we're holding a sword or a shield.
                bool swordOrShield = (this.RightHand != null && (this.RightHand.Type == ItemTypes.Sword || this.RightHand.Type == ItemTypes.Shield)) || (this.LeftHand != null && (this.LeftHand.Type == ItemTypes.Sword || this.LeftHand.Type == ItemTypes.Shield));

                // If we're not holding a shield or sword set the default damage to 1.
                int minDamage = swordOrShield ? 0 : 1;
                int maxDamage = swordOrShield ? 0 : 1;

                // Add the min&max damage of all the gear.
                if (this.Helmet != null)
                {
                    minDamage += this.Helmet.MinDamage;
                    maxDamage += this.Helmet.MaxDamage;
                }

                if (this.LeftHand != null)
                {
                    minDamage += this.LeftHand.MinDamage;
                    maxDamage += this.LeftHand.MaxDamage;
                }

                if (this.RightHand != null)
                {
                    minDamage += this.RightHand.MinDamage;
                    maxDamage += this.RightHand.MaxDamage;
                }

                if (this.Boots != null)
                {
                    minDamage += this.Boots.MinDamage;
                    maxDamage += this.Boots.MaxDamage;
                }

                return random.Next(minDamage, maxDamage);
            }
        }

        /// <summary>
        /// Gets the damage the player should take on his enemy over time.
        /// </summary>
        public Dot DotDamage
        {
            get
            {
                return new Dot();
            }
        }

        /// <summary>
        /// Gets or sets the inventory.
        /// </summary>
        public Inventory Inventory { get; set; }

        /// <summary>
        /// Gets the head the player has equiped.
        /// </summary>
        public Item Helmet { get; private set; }

        /// <summary>
        /// Gets the sword or shield the player has equiped in the left hand.
        /// </summary>
        public Item LeftHand { get; private set; }

        /// <summary>
        /// Gets the sword or shield the player has equiped in the right hand.
        /// </summary>
        public Item RightHand { get; private set; }

        /// <summary>
        /// Gets the boots the player has equiped.
        /// </summary>
        public Item Boots { get; private set; }

        /// <summary>
        /// Gets the amount of tiles to move.
        /// </summary>
        public override int AmountOfTilesToMove
        {
            get { return 2; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="texture">The texture of the player.</param>
        /// <param name="position">The position of the player.</param>
        /// <param name="speed">The movement speed of the player.</param>
        public Player(Texture2D texture, Vector2 position, float speed, int health, SpriteFont combatFont, Vector2 inventoryPositionOffset, Texture2D inventoryBackgroundTexture)
            : base(texture, position, speed, health, combatFont) 
        {
            this.random = new Random();
            this.Inventory = new Inventory(this, inventoryPositionOffset, inventoryBackgroundTexture);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attack a monster.
        /// </summary>
        /// <param name="monster">The monster to attack.</param>
        public void Attack(GameTime gameTime, Monster monster)
        {
            monster.InflictDamage(this.Damage);
        }

        /// <summary>
        /// Pick up an item.
        /// </summary>
        /// <param name="typeOfObject">The object to pick up.</param>
        public bool PickUp(Item item)
        {
            return this.Inventory.TryAddItem(item);
        }

        /// <summary>
        /// Equips an item.
        /// </summary>
        /// <param name="item">The item to equip.</param>
        public Item EquipItem(Item item, bool rightHand)
        {
            Item rItem = null;

            if (item.Type == ItemTypes.Helmet)
            {
                rItem = this.Helmet;
                this.Helmet = item;
            }
            else if (item.Type == ItemTypes.Boots)
            {
                rItem = this.Boots;
                this.Boots = item;
            }
            else if (item.Type == ItemTypes.Sword)
            {
                if (rightHand)
                {
                    rItem = this.RightHand;
                    this.RightHand = item;
                }
                else
                {
                    rItem = this.LeftHand;
                    this.LeftHand = item;
                }
            }
            else if (item.Type == ItemTypes.Shield)
            {
                if (rightHand)
                {
                    rItem = this.RightHand;
                    this.RightHand = item;
                }
                else
                {
                    rItem = this.LeftHand;
                    this.LeftHand = item;
                }
            }

            return rItem;
        }

        /// <summary>
        /// Updates the player.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            this.Inventory.Update(gameTime);
        }

        /// <summary>
        /// Draws the player.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">The spritebatch used to draw.</param>
        /// <param name="cameraPosition">The cameras position.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            base.Draw(gameTime, spriteBatch, cameraPosition);

            this.Inventory.Draw(spriteBatch);
        }

        #endregion
    }
}