using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// A class holding a list of items.
    /// </summary>
    public sealed class MonstersData
    {
        #region Properties

        /// <summary>
        /// A list of items.
        /// </summary>
        public List<MonsterData> Values { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new item collection.
        /// </summary>
        public MonstersData()
        {
            this.Values = new List<MonsterData>();
        }

        #endregion
    }

    public sealed class MonsterData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the texture name.
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// Gets or sets the texture of the item.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Gets or sets the health for this monster.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Gets or sets the maximum damage that the monster should take on his enemy.
        /// </summary>
        public int MinDamage { get; set; }

        /// <summary>
        /// Gets or sets the minimum damage that the monster should take on his enemy.
        /// </summary>
        public int MaxDamage { get; set; }

        /// <summary>
        /// Gets or sets whether this monster is a boss.
        /// </summary>
        public bool Boss { get; set; }

        /// <summary>
        /// Gets or sets the value of this boss.
        /// </summary>
        public int Value { get; set; }

        #endregion
    }
}