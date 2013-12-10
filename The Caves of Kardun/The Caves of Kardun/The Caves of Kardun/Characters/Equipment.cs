using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Caves_of_Kardun
{
    public sealed class Equipment
    {
        #region Fields

        private Vector2 positionOffset;
        private Texture2D backgroundTexture;

        private Rectangle[] bounds;
        private Item hover;

        private Player player;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the head the player has equiped.
        /// </summary>
        public Item Helmet { get; set; }

        /// <summary>
        /// Gets or sets the sword or shield the player has equiped in the left hand.
        /// </summary>
        public Item LeftHand { get; set; }

        /// <summary>
        /// Gets or sets the sword or shield the player has equiped in the right hand.
        /// </summary>
        public Item RightHand { get; set; }

        /// <summary>
        /// Gets or sets the boots the player has equiped.
        /// </summary>
        public Item Boots { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new equipment.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="positionOffset">The position offset from the top left corner.</param>
        /// <param name="backgroundTexture">The background texture.</param>
        public Equipment(Player player, Vector2 positionOffset, Texture2D backgroundTexture)
        {
            this.player = player;
            this.bounds = new Rectangle[5];
            this.positionOffset = positionOffset;
            this.backgroundTexture = backgroundTexture;

            this.bounds[0] = new Rectangle((int)positionOffset.X, (int)positionOffset.Y, 162, 246);
            this.bounds[1] = new Rectangle((int)positionOffset.X + 44, (int)positionOffset.Y + 2, 80, 80);
            this.bounds[2] = new Rectangle((int)positionOffset.X + 2, (int)positionOffset.Y + 84, 80, 80);
            this.bounds[3] = new Rectangle((int)positionOffset.X + 82, (int)positionOffset.Y + 84, 80, 80);
            this.bounds[4] = new Rectangle((int)positionOffset.X + 44, (int)positionOffset.Y + 166, 80, 80);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the inventory, check for mouse clicks etc.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            bool hoverNothing = true;

            if (this.bounds[0].Intersects(new Rectangle(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y, 1, 1)))
            {
                for (int i = 1; i < this.bounds.Length; i++)
                {
                    if (this.bounds[i].Intersects(new Rectangle(TheCavesOfKardun.CurrentMouseState.X, TheCavesOfKardun.CurrentMouseState.Y, 1, 1)))
                    {
                        Item item = IndexToItem(i);
                        this.hover = item;

                        if (this.hover == null)
                        {
                            Tooltip.Hide();
                            return;
                        }

                        Tooltip.Show(this.hover);
                        hoverNothing = false;

                        if (TheCavesOfKardun.CurrentMouseState.LeftButton == ButtonState.Pressed && TheCavesOfKardun.PreviousMouseState.LeftButton == ButtonState.Released)
                        {
                            // Unequip the item amd move it to the inventory, if full do nothing.
                            if (this.player.UnequipItem(item))
                            {
                                if (i == 1)
                                    this.Helmet = null;
                                else if (i == 2)
                                    this.LeftHand = null;
                                else if (i == 3)
                                    this.RightHand = null;
                                else if (i == 4)
                                    this.Boots = null;
                            }
                            else
                            {
                                // Error our bag is full :!
                                throw new Exception("Bag is full.!");
                            }
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
        /// Draws the equipment.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used to draw.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.backgroundTexture, this.positionOffset, Color.White);

            if (this.Helmet != null)
                spriteBatch.Draw(this.Helmet.Texture, this.bounds[1], Color.White);

            if (this.LeftHand != null)
                spriteBatch.Draw(this.LeftHand.Texture, this.bounds[2], Color.White);

            if (this.RightHand != null)
                spriteBatch.Draw(this.RightHand.Texture, this.bounds[3], Color.White);

            if (this.Boots != null)
                spriteBatch.Draw(this.Boots.Texture, this.bounds[4], Color.White);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Converts an index to the right item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Returns the item of the index or null.</returns>
        private Item IndexToItem(int index)
        {
            switch (index)
            {
                case 1:
                    return this.Helmet;
                case 2:
                    return this.LeftHand;
                case 3:
                    return this.RightHand;
                case 4:
                    return this.Boots;

                default:
                    return null;
            }
        }

        #endregion

    }
}
