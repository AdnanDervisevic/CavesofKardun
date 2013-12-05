namespace The_Caves_of_Kardun
{
    /// <summary>
    /// A struct containing data about how much damage you should take other the amoung of rounds.
    /// </summary>
    public struct Dot
    {
        /// <summary>
        /// Gets or sets the damage.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// Gets or sets the rounds.
        /// </summary>
        public int Rounds { get; set; }

        /// <summary>
        /// Gets the damage per round.
        /// </summary>
        public int DamagePerRound
        {
            get
            {
                return this.Damage / this.Rounds;
            }
        }
    }
}
