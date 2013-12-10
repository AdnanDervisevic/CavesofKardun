#region File Description
    //////////////////////////////////////////////////////////////////////////
   // Inventory                                                            //
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
    public sealed class Inventory
    {
        #region Consts

        public const int MaxSlots = 9;
        public const int SlotsPerRow = 3;

        public const int Margin = 2;
        public const int Spacing = 2;
        public const int SlotWidth = 80;

        #endregion

        #region Fields

        private Random random;

        private Texture2D backgroundTexture;
        private Item[] items;
        private Vector2 positionOffset;

        private Rectangle[] bounds;

        private Texture2D menuTexture;
        private Texture2D menuChoiceTexture;
        
        private Vector2 menuPosition;
        private Rectangle[] menuBounds;
        private int itemIndexClicked;

        private Item hover;
        private Player player;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether inventory should be drawn.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the gold in the inventory.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets all the items in the inventory.
        /// </summary>
        public Item[] Items
        {
            get { return this.items; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates the inventory.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="positionOffset">The position offset from the top left corner (0, 0).</param>
        /// <param name="backgroundTexture">The background texture.</param>
        public Inventory(Player player, Vector2 positionOffset, Texture2D backgroundTexture, Texture2D menuTexture, Texture2D menuChoiceTexture)
        {
            this.menuTexture = menuTexture;
            this.menuChoiceTexture = menuChoiceTexture;
            this.itemIndexClicked = -1;
            this.player = player;
            this.random = new Random();
            this.items = new Item[Inventory.MaxSlots];
            this.bounds = new Rectangle[Inventory.MaxSlots];
            this.positionOffset = positionOffset;
            this.backgroundTexture = backgroundTexture;

            for (int i = 0; i < Inventory.MaxSlots; i++)
                this.bounds[i] = new Rectangle(((int)positionOffset.X + (Margin + ((SlotWidth + Spacing) * (i % 3)))), ((int)positionOffset.Y + (Margin + ((SlotWidth + Spacing) * (i / 3)))), SlotWidth, SlotWidth);

            this.Visible = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Try add a new item to the inventory.
        /// </summary>
        /// <param name="item">The item you're trying to add.</param>
        /// <returns>Returns true if the item was added; otherwise false.</returns>
        public bool TryAddItem(Item item)
        {
            if (item.Type == ItemTypes.Ladder)
                return false;

            if (item.Type == ItemTypes.Gold)
            {
                int minGold = Math.Min(item.MinGold, item.MaxGold + 1);
                int maxGold = Math.Max(item.MinGold, item.MaxGold + 1);
                this.Gold += random.Next(minGold, maxGold);

                return true;
            }

            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] == null)
                {
                    this.items[i] = item;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the inventory, checks for mouse clicks etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            if (!this.Visible)
                return;

            if (this.itemIndexClicked >= 0)
            {
                this.hover = null;
                Tooltip.Hide();

                if (this.menuBounds[0].Intersects(new Rectangle(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y, 1, 1)))
                {
                    for (int i = 1; i < this.menuBounds.Length; i++)
                    {
                        if (this.menuBounds[i].Intersects(new Rectangle(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y, 1, 1)))
                        {
                            if (TheCavesOfKardun.CurrentMouseState.LeftButton == ButtonState.Pressed && TheCavesOfKardun.PreviousMouseState.LeftButton == ButtonState.Released)
                            {
                                if (i == this.menuBounds.Length - 1)
                                    this.items[this.itemIndexClicked] = null;
                                else
                                    this.items[this.itemIndexClicked] = this.player.EquipItem(this.items[this.itemIndexClicked], ((i == 2) || (this.player.NegativeTraits & NegativeTraits.MissingAnArm) == NegativeTraits.MissingAnArm));

                                this.itemIndexClicked = -1;
                            }
                        }
                    }
                }
                else
                {
                    this.itemIndexClicked = -1;
                }                
            }
            else
            {
                bool hoverNothing = true;

                for (int i = 0; i < Inventory.MaxSlots; i++)
                {
                    // Check if we intersects with any item slot.
                    if (this.bounds[i].Intersects(new Rectangle(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y, 1, 1)))
                    {
                        this.hover = this.items[i];

                        if (this.hover == null)
                        {
                            Tooltip.Hide();
                            return;
                        }
                        hoverNothing = false;
                        Tooltip.Show(this.hover);

                        if (TheCavesOfKardun.CurrentMouseState.LeftButton == ButtonState.Pressed && TheCavesOfKardun.PreviousMouseState.LeftButton == ButtonState.Released)
                        {
                            // Equips the item and moves the old item to the inventory.
                            bool AllowTwoSwords = AllowTwoSwords = (((this.player.PositiveTraits & PositiveTraits.Ambidextrous) == PositiveTraits.Ambidextrous) && ((this.player.NegativeTraits & NegativeTraits.MissingAnArm) != NegativeTraits.MissingAnArm)) ? true : false;

                            if (AllowTwoSwords && this.hover.Type == ItemTypes.Sword)
                            {
                                // Show both options.
                                this.menuBounds = new Rectangle[4];
                                this.menuPosition = new Vector2(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y);
                                this.menuPosition.Y -= 51;
                                this.menuPosition.X -= 67;

                                this.menuBounds[0] = new Rectangle((int)this.menuPosition.X, (int)this.menuPosition.Y, 135, 103);
                                this.menuBounds[1] = new Rectangle((int)this.menuPosition.X + 3, (int)this.menuPosition.Y + 28, 129, 22);
                                this.menuBounds[2] = new Rectangle((int)this.menuPosition.X + 3, (int)this.menuPosition.Y + 53, 129, 22);
                                this.menuBounds[3] = new Rectangle((int)this.menuPosition.X + 3, (int)this.menuPosition.Y + 78, 129, 22);
                            }
                            else
                            {
                                // Show only equip
                                this.menuBounds = new Rectangle[3];
                                this.menuPosition = new Vector2(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y);
                                this.menuPosition.Y -= 26;
                                this.menuPosition.X -= 67;

                                this.menuBounds[0] = new Rectangle((int)this.menuPosition.X, (int)this.menuPosition.Y, 135, 53);
                                this.menuBounds[1] = new Rectangle((int)this.menuPosition.X + 3, (int)this.menuPosition.Y + 3, 129, 22);
                                this.menuBounds[2] = new Rectangle((int)this.menuPosition.X + 3, (int)this.menuPosition.Y + 28, 129, 22);
                            }

                            this.itemIndexClicked = i;
                        }
                    }
                }

                if (hoverNothing)
                {
                    this.hover = null;
                    Tooltip.Hide();
                }
            }
        }

        /// <summary>
        /// Draws the inventory.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used for drawing.</param>
        /// <param name="cameraPosition">The camera position.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!this.Visible)
                return;

            spriteBatch.Draw(this.backgroundTexture, this.positionOffset, Color.White);

            // Draw Items.
            for (int i = 0; i < this.items.Length; i++)
                if (this.items[i] != null && this.items[i].Type != ItemTypes.Gold)
                    spriteBatch.Draw(this.items[i].Texture, 
                        this.bounds[i], Color.White);

            if (this.itemIndexClicked >= 0)
            {
                if (this.menuBounds.Length == 4)
                    spriteBatch.Draw(this.menuChoiceTexture, this.menuPosition, Color.White);
                else
                    spriteBatch.Draw(this.menuTexture, this.menuPosition, Color.White);
            }
        }

        #endregion
    }
}