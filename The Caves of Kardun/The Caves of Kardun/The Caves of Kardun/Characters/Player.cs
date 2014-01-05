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
using Microsoft.Xna.Framework.Content;
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
                if (this.Equipment.Helmet != null)
                {
                    bonusHealth += this.Equipment.Helmet.Health;
                    if (this.Equipment.Helmet.Special == ItemSpecials.HelmetOfLife)
                        bonusHealth += 15;
                }

                if (this.Equipment.LeftHand != null)
                    bonusHealth += this.Equipment.LeftHand.Health;

                if (this.Equipment.RightHand != null)
                    bonusHealth += this.Equipment.RightHand.Health;

                if (this.Equipment.Boots != null)
                    bonusHealth += this.Equipment.Boots.Health;

                if ((this.PositiveTraits & PositiveTraits.SuperLife) == PositiveTraits.SuperLife)
                    return (int)(((base.Health + bonusHealth) * 2) * Math.Max(((float)Level.LevelCount / 2), 1));
                else
                    return (int)((base.Health + bonusHealth) * Math.Max(((float)Level.LevelCount / 2), 1));
            }
        }

        /// <summary>
        /// Gets the health of the player.
        /// </summary>
        public int Block
        {
            get
            {
                int blonusBlock = 0;
                if (this.Equipment.Helmet != null)
                    blonusBlock += this.Equipment.Helmet.Block;

                if (this.Equipment.LeftHand != null)
                    blonusBlock += this.Equipment.LeftHand.Block;

                if (this.Equipment.RightHand != null)
                    blonusBlock += this.Equipment.RightHand.Block;

                if (this.Equipment.Boots != null)
                    blonusBlock += this.Equipment.Boots.Block;

                return (int)(blonusBlock * Math.Max(((float)Level.LevelCount / 2), 1));
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
                bool swordOrShield = (this.Equipment.RightHand != null && (this.Equipment.RightHand.Type == ItemTypes.Sword || this.Equipment.RightHand.Type == ItemTypes.Shield)) ||
                    (this.Equipment.LeftHand != null && this.Equipment.LeftHand.Type == ItemTypes.Sword);

                // If we're not holding a shield or sword set the default damage to 1.
                int minDamage = swordOrShield ? 0 : 3;
                int maxDamage = swordOrShield ? 0 : 3;

                // Add the min&max damage of all the gear.
                if (this.Equipment.Helmet != null)
                {
                    minDamage += this.Equipment.Helmet.MinDamage;
                    maxDamage += this.Equipment.Helmet.MaxDamage;
                }

                if (this.Equipment.LeftHand != null)
                {
                    minDamage += this.Equipment.LeftHand.MinDamage;
                    maxDamage += this.Equipment.LeftHand.MaxDamage;
                }

                if (this.Equipment.RightHand != null)
                {
                    minDamage += this.Equipment.RightHand.MinDamage;
                    maxDamage += this.Equipment.RightHand.MaxDamage;
                }

                if (this.Equipment.Boots != null)
                {
                    minDamage += this.Equipment.Boots.MinDamage;
                    maxDamage += this.Equipment.Boots.MaxDamage;
                }

                if ((this.PositiveTraits & PositiveTraits.SuperStrength) == PositiveTraits.SuperStrength)
                    return (int)(random.Next(minDamage * 2, maxDamage * 2 + 1) * Math.Max(((float)Level.LevelCount / 2), 1));
                else
                    return (int)(random.Next(minDamage, maxDamage + 1) * Math.Max(((float)Level.LevelCount / 2), 1));
            }
        }

        /// <summary>
        /// Gets the damage the player should take on his enemy over time.
        /// </summary>
        public Dot DotDamage
        {
            get
            {
                Dot dot = new Dot();

                if (this.Equipment.RightHand != null && this.Equipment.RightHand.Type == ItemTypes.Sword)
                {
                    dot.Damage += this.Equipment.RightHand.DotDamage;
                    dot.DotDuration += this.Equipment.RightHand.DotDuration;
                }

                if (this.Equipment.LeftHand != null && this.Equipment.LeftHand.Type == ItemTypes.Sword)
                {
                    dot.Damage += this.Equipment.LeftHand.DotDamage;
                    dot.DotDuration += this.Equipment.LeftHand.DotDuration;
                }

                dot.Damage *= (int)Math.Max(((float)Level.LevelCount / 2), 1);

                return dot;
            }
        }

        /// <summary>
        /// Gets the miss chance.
        /// </summary>
        public int MissChance
        {
            get
            {
                int value = 0;

                if (this.Equipment.Helmet != null)
                    value += this.Equipment.Helmet.MissChance;

                if (this.Equipment.LeftHand != null)
                    value += this.Equipment.LeftHand.MissChance;

                if (this.Equipment.RightHand != null)
                    value += this.Equipment.RightHand.MissChance;

                if (this.Equipment.Boots != null)
                    value += this.Equipment.Boots.MissChance;

                return value;
            }
        }

        /// <summary>
        /// Gets or sets the inventory.
        /// </summary>
        public Inventory Inventory { get; set; }

        /// <summary>
        /// Gets or sets the equipment.
        /// </summary>
        public Equipment Equipment { get; set; }

        /// <summary>
        /// Gets the amount of tiles to move.
        /// </summary>
        public override int AmountOfTilesToMove
        {
            get { return (this.Equipment.Boots != null || (this.PositiveTraits & PositiveTraits.ElvenSpeed) == PositiveTraits.ElvenSpeed) ? 2 : 1;  }
        }

        /// <summary>
        /// Gets or sets the monster the player is attacking.
        /// </summary>
        public Monster Attacks { get; set; }

        /// <summary>
        /// Positive traits.
        /// </summary>
        public PositiveTraits PositiveTraits { get; private set; }

        /// <summary>
        /// Negative traits.
        /// </summary>
        public NegativeTraits NegativeTraits { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="texture">The texture of the player.</param>
        /// <param name="position">The position of the player.</param>
        /// <param name="speed">The movement speed of the player.</param>
        public Player(Texture2D texture, Vector2 position, float speed, int health, SpriteFont combatFont)
            : base("Player", texture, position, speed, health, combatFont) 
        {
            
            this.random = new Random();
            RandomTraits();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generate random traits for the player.
        /// </summary>
        public void RandomTraits()
        {
            this.PositiveTraits = 0;
            this.NegativeTraits = 0;

            int[] table = new int[] { 1, 2, 4, 8, 16 };

            int traits = 0;
            do
            {
                PositiveTraits trait = (PositiveTraits)table[random.Next(0, 5)];
                if ((this.PositiveTraits & trait) != trait)
                {
                    traits++;
                    this.PositiveTraits |= trait;
                }
            } while (traits < 2);

            traits = 0;
            do
            {
                NegativeTraits trait = (NegativeTraits)table[random.Next(0, 5)];
                if ((this.NegativeTraits & trait) != trait)
                {
                    traits++;
                    this.NegativeTraits |= trait;
                }
            } while (traits < 2);
        }

        /// <summary>
        /// Loads content for the player.
        /// </summary>
        /// <param name="Content">The contentManager.</param>
        /// <param name="inventoryPositionOffset">The inventory position offset from the upper left corner.</param>
        /// <param name="equipmentPositionOffset">The equipment position offset from the upper left corner.</param>
        public void LoadContent(ContentManager Content, Vector2 inventoryPositionOffset, Vector2 equipmentPositionOffset)
        {          
            Texture2D inventoryBackgroundTexture = Content.Load<Texture2D>("Textures/Inventory/Background");
            Texture2D inventoryEquipMenuTexture = Content.Load<Texture2D>("Textures/Inventory/EquipMenu");
            Texture2D inventoryEquipMenuChoiceTexture = Content.Load<Texture2D>("Textures/Inventory/EquipMenuChoice");
            Texture2D equipmentBackgroundTexture = Content.Load<Texture2D>("Textures/Equipment");
            Texture2D equipmentOneArmBackgroundTexture = Content.Load<Texture2D>("Textures/OneArmEquipment");

            this.Inventory = new Inventory(this, inventoryPositionOffset, inventoryBackgroundTexture, inventoryEquipMenuTexture, inventoryEquipMenuChoiceTexture);
            if ((this.NegativeTraits & NegativeTraits.MissingAnArm) != NegativeTraits.MissingAnArm)
                this.Equipment = new Equipment(this, equipmentPositionOffset, equipmentBackgroundTexture);
            else
                this.Equipment = new Equipment(this, equipmentPositionOffset, equipmentOneArmBackgroundTexture);
        }

        /// <summary>
        /// Resets the player, resets the gold, inventory and the equipment.
        /// </summary>
        public void Reset()
        {
            this.Inventory.Gold = 0;
            for (int i = 0; i < this.Inventory.Items.GetLength(0) - 1; i++)
                this.Inventory.Items[i] = null;

            this.Equipment.Helmet = null;
            this.Equipment.RightHand = null;
            this.Equipment.LeftHand = null;
            this.Equipment.Boots = null;

            this.Motion = Vector2.Zero;
            this.TargetPosition = Vector2.Zero;

            this.DamageTaken = 0;
        }

        /// <summary>
        /// Attack a monster.
        /// </summary>
        /// <param name="monster">The monster to attack.</param>
        public void Attack(GameTime gameTime, Monster monster)
        {
            // If the monster doesn't have a dot and the weapon we're using have dot damage then apply the dot and reset the round counter.
            if (monster.Dot == null && this.DotDamage.Damage > 0)
            {
                monster.Dot = this.DotDamage;
                monster.Dot.RoundCounter = 0;
            }

            // If our monster has a dot inflict the normal damage + the damage from the dot to the monster and increase the round counter.
            if (monster.Dot != null)
            {
                monster.InflictDamage(this.Damage + monster.Dot.DamagePerRound);
                monster.Dot.RoundCounter++;
            }
            else // If our monster doesn't have a dot then inflict the normal damage only.
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
                rItem = this.Equipment.Helmet;
                this.Equipment.Helmet = item;
            }
            else if (item.Type == ItemTypes.Boots)
            {
                rItem = this.Equipment.Boots;
                this.Equipment.Boots = item;
            }
            else if (item.Type == ItemTypes.Sword)
            {
                if (rightHand)
                {
                    rItem = this.Equipment.RightHand;
                    this.Equipment.RightHand = item;
                }
                else
                {
                    rItem = this.Equipment.LeftHand;
                    this.Equipment.LeftHand = item;
                }
            }
            else if (item.Type == ItemTypes.Shield)
            {
                rItem = this.Equipment.RightHand;
                this.Equipment.RightHand = item;
            }
            else if (item.Type == ItemTypes.Potion)
            {
                this.DamageTaken -= item.Health;
                if (this.DamageTaken < 0)
                    this.DamageTaken = 0;
            }

            return rItem;
        }

        /// <summary>
        /// Unequip an item.
        /// </summary>
        /// <param name="item">The item to unequip.</param>
        /// <returns>Returns true if the item could be unequipped.</returns>
        public bool UnequipItem(Item item)
        {
            return this.Inventory.TryAddItem(item);
        }

        /// <summary>
        /// Updates the player.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            this.Inventory.Update(gameTime);
            this.Equipment.Update(gameTime);
        }

        /// <summary>
        /// Draws the player.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">The spritebatch used to draw.</param>
        /// <param name="cameraPosition">The cameras position.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            if (this.Alive)
            {
                spriteBatch.Draw(this.texture,
                new Rectangle(
                    (int)(this.Position.X - cameraPosition.X),
                    (int)(this.Position.Y - cameraPosition.Y),
                    TheCavesOfKardun.TileWidth, TheCavesOfKardun.TileHeight), Color.White);
            }

            UpdateCombatText(gameTime, spriteBatch, cameraPosition);

            this.Inventory.Draw(spriteBatch);
            this.Equipment.Draw(spriteBatch);
        }

        #endregion
    }
}