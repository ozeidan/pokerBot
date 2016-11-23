using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Detection
{
    class Eye
    {
        private NumberDetector numberDetector;
        private CardDetector cardDetector;
        private TurnDetector turnDetector;
        private PlayerCount count;

        public Eye(IntPtr handle)
        {
            numberDetector = new NumberDetector(handle);
            cardDetector = new CardDetector(handle);
            turnDetector = new TurnDetector(handle);
            count = new PlayerCount(handle);
        }

        /// <summary>
        /// Returns the amount of money in the pot.
        /// </summary>
        /// <returns>Amount of money in the pot.</returns>
        public int getPotAmount()
        {
            return numberDetector.getPot();
        }
        /// <summary>
        /// Returns the amount of money the player has.
        /// </summary>
        /// <returns>Amount of money the player has.</returns>
        public int getMoney()
        {
            return numberDetector.GetHashCode();
        }

        /// <summary>
        /// Returns the amount of money the player has to pay to stay in the game.
        /// </summary>
        /// <returns>Amount of money the player has to pay to stay in the game.</returns>
        public int getMinimumCall()
        {
            return numberDetector.minimalContribution();
        }
        /// <summary>
        /// Returns the smallest amount of money that the player can raise or bet.
        /// </summary>
        /// <returns>The smallest amount of money that the player can raise or bet.</returns>
        public int getMinimalRaise()
        {
            return numberDetector.minimalRaise();
        }
        /// <summary>
        /// Returns the players cards.
        /// </summary>
        /// <returns>The players cards</returns>
        public List<Card> getPlayerCards()
        {
            return cardDetector.getCards(true);
        }
        /// <summary>
        /// Returns the cards on the table.
        /// </summary>
        /// <returns>Cards on the table.</returns>
        public List<Card> getTableCards()
        {
            return cardDetector.getCards(false);
        }


        /// <summary>
        /// Specifies wether it is the turn of the player or not.
        /// </summary>
        /// <returns>Wether it is the turn of the player or not.</returns>
        public bool myTurn()
        {
            return turnDetector.myTurn();
        }

        /// <summary>
        /// Specifies wether the player can only call due to not having enough money to raise or not.
        /// </summary>
        /// <returns>Wether the player can only call due to not having enough money to raise or not.</returns>
        public bool onlyCall()
        {
            return turnDetector.onlyCall();
        }

        /// <summary>
        /// Returns the amount of players that are still in the game.
        /// </summary>
        /// <returns>Amount of players that are still in the game.</returns>
        public int getPlayerCount()
        {
            return count.playerCount();
        }



    }
}
