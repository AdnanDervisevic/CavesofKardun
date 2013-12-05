using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace The_Caves_of_Kardun
{
    public sealed class Monster : Character
    {
        #region Properties

        /// <summary>
        /// Gets the damage that the monster should take on his enemy.
        /// </summary>
        public int Damage { get; private set; }

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
        public Monster(Texture2D texture, Vector2 position, float speed, int baseHealth, int baseDamage, SpriteFont combatFont)
            : base(texture, position, speed, baseHealth, combatFont) 
        {
            this.Damage = baseDamage;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the AI of this monster.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="playerTile">The player position, as a tile.</param>
        /// <returns>Returns the damage to inflict to the player.</returns>
        public int UpdateAI(GameTime gameTime, Point playerTile)
        {
            // If the monster is not alive, return zero.
            if (!this.Alive)
                return 0;

            Point monsterTile = TheCavesOfKardun.ConvertPositionToCell(this.Center);

            // If the player is to the left of the monster, attack.
            if (monsterTile.X + 1 == playerTile.X && monsterTile.Y == playerTile.Y) // If the player is to the right of the monster, return the damage the monster should inflict.
                return this.Damage;
            else if (monsterTile.X - 1 == playerTile.X && monsterTile.Y == playerTile.Y) // If the player is to the left of the monster, return the damage the monster should inflict.
                return this.Damage;
            else if (monsterTile.X == playerTile.X && monsterTile.Y + 1 == playerTile.Y) // If the player is below the monster, return the damage the monster should inflict.
                return this.Damage;
            else if (monsterTile.X == playerTile.X && monsterTile.Y - 1 == playerTile.Y) // If the player is above the monster, return the damage the monster should inflict.
                return this.Damage;
            else // If the monster is not close to the player, update the AI.
            {
                // If the monster is between two tiles then return zero because we're not doing any damage to the player.
                if (UpdateMovement(gameTime))
                    return 0;
                // Otherwise we're calculating the movement AI

                // Compare playerTile and monsterTile to calculate the motion the monster should have.
                // Then call if (CanWalk(monster, calculatedMotion, 1, out monster.TargetPosition))
                // If this returns true just continue to the next iteration like we did @line 171
                // Otherwise there're something in the way and we need to re-calculate a new motion and try again.
            }

            return 0; // Returns zero because we're not doing any damage to the player.
        }

        #endregion
    }
}
