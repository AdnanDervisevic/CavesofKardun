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
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

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
            this.Name = string.Empty;
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

    #region Content Type Reader

    /// <summary>
    /// Class for reading an item from a content file.
    /// </summary>
    public class ItemReader : ContentTypeReader<Item>
    {
        /// <summary>
        /// Reads the Item from the content file.
        /// </summary>
        /// <param name="input">The input file.</param>
        /// <param name="existingInstance">The existing instance.</param>
        /// <returns>Returns the newly created item.</returns>
        protected override Item Read(ContentReader input, Item existingInstance)
        {
            Item item = existingInstance;
            if (item == null)
                item = new Item();

            item.Name = input.ReadString();
            item.OverworldTextureName = input.ReadString();
            if (!string.IsNullOrWhiteSpace(item.OverworldTextureName))
                item.OverworldTexture = input.ContentManager.Load<Texture2D>(item.OverworldTextureName);
            item.TextureName = input.ReadString();
            if (!string.IsNullOrWhiteSpace(item.TextureName))
                item.Texture = input.ContentManager.Load<Texture2D>(item.TextureName);
            item.Type = (ItemTypes)input.ReadInt32();
            item.Value = input.ReadInt32();
            item.MinDamage = input.ReadInt32();
            item.MaxDamage = input.ReadInt32();
            item.DotDamage = input.ReadInt32();
            item.DotDuration = input.ReadInt32();
            item.MissChance = input.ReadInt32();
            item.Speed = input.ReadInt32();
            item.Health = input.ReadInt32();
            item.Block = input.ReadInt32();
            item.Special = (ItemSpecials)input.ReadInt32();

            return item;
        }
    }

    #endregion
}
