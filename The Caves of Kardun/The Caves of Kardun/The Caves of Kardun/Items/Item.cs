#region File Description
    //////////////////////////////////////////////////////////////////////////
   // Item                                                            //
  //                                                                      //
 // Copyright (C) Untitled. All Rights reserved.                         //
//////////////////////////////////////////////////////////////////////////
#endregion

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// An Item gives the character bonus.
    /// </summary>
    public struct Item
    {
        #region Properties

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
        public int MinDamge { get; set; }

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
    }
}
