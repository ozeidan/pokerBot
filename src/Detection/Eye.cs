using System;
using System.Collections.Generic;

namespace Bot.Detection
{
    internal class Eye
    {
        private readonly CardDetector _cardDetector;
        private readonly PlayerCount _count;
        private readonly NumberDetector _numberDetector;
        private readonly RoundDetector _turnDetector;

        public Eye(IntPtr handle)
        {
            _numberDetector = new NumberDetector(handle);
            _cardDetector = new CardDetector(handle);
            _turnDetector = new RoundDetector(handle);
            _count = new PlayerCount(handle);
        }

        /// <summary>
        ///     Returns the amount of money in the pot.
        /// </summary>
        /// <returns>Amount of money in the pot.</returns>
        public int GetPotAmount()
        {
            return _numberDetector.GetPot();
        }

        /// <summary>
        ///     Returns the amount of money the player has.
        /// </summary>
        /// <returns>Amount of money the player has.</returns>
        public int GetMoney()
        {
            return _numberDetector.GetHashCode();
        }

        /// <summary>
        ///     Returns the amount of money the player has to pay to stay in the game.
        /// </summary>
        /// <returns>Amount of money the player has to pay to stay in the game.</returns>
        public int GetMinimumCall()
        {
            return _numberDetector.MinimalContribution();
        }

        /// <summary>
        ///     Returns the smallest amount of money that the player can Raise or bet.
        /// </summary>
        /// <returns>The smallest amount of money that the player can Raise or bet.</returns>
        public int GetMinimalRaise()
        {
            return _numberDetector.MinimalRaise();
        }

        /// <summary>
        ///     Returns the players cards.
        /// </summary>
        /// <returns>The players cards</returns>
        public List<Data.Card> GetPlayerCards()
        {
            return _cardDetector.GetCards(true);
        }

        /// <summary>
        ///     Returns the cards on the table.
        /// </summary>
        /// <returns>Cards on the table.</returns>
        public List<Data.Card> GetTableCards()
        {
            return _cardDetector.GetCards(false);
        }


        /// <summary>
        ///     Specifies wether it is the turn of the player or not.
        /// </summary>
        /// <returns>Wether it is the turn of the player or not.</returns>
        public bool MyTurn()
        {
            return _turnDetector.MyTurn();
        }

        /// <summary>
        ///     Specifies wether the player can only call due to not having enough money to Raise or not.
        /// </summary>
        /// <returns>Wether the player can only call due to not having enough money to Raise or not.</returns>
        public bool OnlyCall()
        {
            return _turnDetector.OnlyCall();
        }

        /// <summary>
        ///     Returns the amount of players that are still in the game.
        /// </summary>
        /// <returns>Amount of players that are still in the game.</returns>
        public int GetPlayerCount()
        {
            return _count.playerCount();
        }
    }
}