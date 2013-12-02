using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Caves_of_Kardun
{
    public sealed class Monster : Character
    {
        #region Properties

        /// <summary>
        /// Gets the amount of tiles to move.
        /// </summary>
        public override int AmountOfTilesToMove
        {
            get { return 1; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty monster object.
        /// </summary>
        public Monster()
            : base(null, Vector2.Zero, 0, 0, null) { }

        /// <summary>
        /// Creates a new monster.
        /// </summary>
        /// <param name="texture">The texture of the monster.</param>
        /// <param name="position">The position of the monster.</param>
        /// <param name="speed">The speed of the monster.</param>
        public Monster(Texture2D texture, Vector2 position, float speed, int health, SpriteFont combatFont)
            : base(texture, position, speed, health, combatFont) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attack a monster.
        /// </summary>
        /// <param name="monster">The monster to attack.</param>
        public void Attack(Player player)
        {
            player.CombatText = "-30 Damage";
            player.Health -= 30;
        }

        #endregion
    }
}
