#region File Description
    //////////////////////////////////////////////////////////////////////////
   // Item                                                            //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

#region Using Statements
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// An Item gives the character different bonuses.
    /// </summary>
    public sealed class Item
    {
        #region Properties

        /// <summary>
        /// Gets or sets the overworld texture name.
        /// </summary>
        public string OverworldTextureName { get; set; }

        /// <summary>
        /// Gets or sets the overworld texture of the item.
        /// </summary>
        public Texture2D OverworldTexture { get; set; }

        /// <summary>
        /// Gets or sets the texture name.
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// Gets or sets the texture of the item.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        public ItemTypes Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// The minimum damage.
        /// </summary>
        public int MinDamage { get; set; }

        /// <summary>
        /// The maximum damage.
        /// </summary>
        public int MaxDamage { get; set; }

        /// <summary>
        /// The dot damage.
        /// </summary>
        public int DotDamage { get; set; }

        /// <summary>
        /// The dot duration, in rounds.
        /// </summary>
        public int DotDuration { get; set; }

        /// <summary>
        /// The miss chance bonus, in percentage.
        /// </summary>
        public int MissChance { get; set; }

        /// <summary>
        /// The speed bonus, in tiles.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// The health bonus.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// The block bonus.
        /// </summary>
        public int Block { get; set; }

        /// <summary>
        /// Gets or sets the item special.
        /// </summary>
        public ItemSpecials Special { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new item.
        /// </summary>
        public Item()
        {
            this.OverworldTextureName = string.Empty;
            this.TextureName = string.Empty;
            this.Type = ItemTypes.None;
            this.Value = 0;
            this.MinDamage = 0;
            this.MaxDamage = 0;
            this.DotDamage = 0;
            this.DotDuration = 0;
            this.MissChance = 0;
            this.Speed = 0;
            this.Health = 0;
            this.Block = 0;
            this.Special = ItemSpecials.None;
        }

        #endregion
    }
}
