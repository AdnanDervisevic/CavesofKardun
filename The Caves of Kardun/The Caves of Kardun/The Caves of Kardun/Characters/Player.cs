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
                for (int i = 0; i < this.Equipment.Length; i++)
                    if (this.Equipment[i] != null)
                        bonusHealth += this.Equipment[i].Health;

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
                bool sword = false;
                for (int i = 0; i < this.Equipment.Length; i++)
                {
                    if (this.Equipment[i] != null && this.Equipment[i].Type == ItemTypes.Sword)
                    {
                        sword = true;
                        break;
                    }
                }

                int minDamage = sword ? 0 : 1;
                int maxDamage = sword ? 0 : 1;

                for (int i = 0; i < this.Equipment.Length; i++)
                {
                    if (this.Equipment[i] == null)
                        continue;

                    minDamage += this.Equipment[i].MinDamage;
                    maxDamage += this.Equipment[i].MaxDamage;
                }

                return random.Next(minDamage, maxDamage);
            }
        }

        /// <summary>
        /// Gets or sets the inventory.
        /// </summary>
        public Inventory Inventory { get; set; }

        /// <summary>
        /// The items the player has equiped.
        /// </summary>
        public Item[] Equipment { get; private set; }

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
            this.Equipment = new Item[4];
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
        public Item EquipItem(Item item)
        {
            throw new Exception("Equip Item");
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