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

        /// <summary>
        /// Gets or sets the dot doing damage to this monster.
        /// </summary>
        public Dot Dot { get; set; }

        /// <summary>
        /// Gets the spawn tile of this monster.
        /// </summary>
        public Point SpawnTile {  get; private set; }

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
        public Monster(Texture2D texture, Point spawnTile, float speed, int baseHealth, int baseDamage, SpriteFont combatFont)
            : base(texture, TheCavesOfKardun.ConvertCellToPosition(spawnTile), speed, baseHealth, combatFont) 
        {
            this.Damage = baseDamage;
            this.SpawnTile = spawnTile;
        }

        #endregion

        #region Public Methods

        Vector2 motion = new Vector2(1, 0);
        /// <summary>
        /// Updates the AI of this monster.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="playerTile">The player position, as a tile.</param>
        /// <returns>Returns the damage to inflict to the player.</returns>
        public int UpdateAI(GameTime gameTime, Level level, Player player)
        {
            // If the monster is not alive, return zero.
            if (!this.Alive)
                return 0;

            Point playerTile = TheCavesOfKardun.ConvertPositionToCell(player.Center);
            Point monsterTile = TheCavesOfKardun.ConvertPositionToCell(this.Center);

            // If the player is not attacking this monster and the monster has a dot, inflict the damage and increase the round counter.
            if (player.Attacks != this && this.Dot != null)
            {
                if (this.Dot.RoundCounter < this.Dot.DotDuration)
                {
                    this.InflictDamage(this.Dot.DamagePerRound);
                    this.Dot.RoundCounter++;
                }
                else
                    this.Dot = null;
            }

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
                // Compare playerTile and monsterTile to see how far away they're from each other,
                // if they're close enough calculate the motion the monster should have and
                // then call if (!level.CanWalk(monster, calculatedMotion, 1, out monster.TargetPosition))
                // If this returns false something in the way and we need to re-calculate a new motion and try again.


                // TEST AI ! Går bara i sidled :!
                // Denna kod körs på alla monsters, inte bara dom i rummet så man måste kolla
                // om spelaren och monstret är tillräckligt nära varandra för att sätt en motion.
                // Om spelaren och monstret är för långt ifrån varandra sätt motion = Vector2.Zero

                if (motion == Vector2.Zero)
                    motion = new Vector2(1, 0);

                if (!level.CanWalk(this, motion, 1, out this.TargetPosition))
                {
                    motion *= -1;
                    if (!level.CanWalk(this, motion, 1, out this.TargetPosition))
                    {
                        motion = Vector2.Zero;
                    }
                }

                this.Motion = motion;
            }

            return 0; // Returns zero because we're not doing any damage to the player.
        }

        #endregion
    }
}
