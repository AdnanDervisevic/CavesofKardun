using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace The_Caves_of_Kardun
{
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

    #region Content Type Reader

    /// <summary>
    /// Class for reading an item from a content file.
    /// </summary>
    public class MonsterReader : ContentTypeReader<List<MonsterData>>
    {
        /// <summary>
        /// Reads the Item from the content file.
        /// </summary>
        /// <param name="input">The input file.</param>
        /// <param name="existingInstance">The existing instance.</param>
        /// <returns>Returns the newly created item.</returns>
        protected override List<MonsterData> Read(ContentReader input, List<MonsterData> existingInstance)
        {
            List<MonsterData> monstersData = existingInstance;
            if (monstersData == null)
                monstersData = new List<MonsterData>();

            int count = input.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                MonsterData monsterData = new MonsterData();

                monsterData.Name = input.ReadString();
                monsterData.TextureName = input.ReadString();
                if (!string.IsNullOrWhiteSpace(monsterData.TextureName))
                    monsterData.Texture = input.ContentManager.Load<Texture2D>(monsterData.TextureName);
                monsterData.Value = input.ReadInt32();
                monsterData.Health = input.ReadInt32();
                monsterData.MinDamage = input.ReadInt32();
                monsterData.MaxDamage = input.ReadInt32();
                monsterData.Boss = input.ReadBoolean();

                monstersData.Add(monsterData);
            }

            return monstersData;
        }
    }

    #endregion
}