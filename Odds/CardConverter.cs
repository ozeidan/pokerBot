using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Odds
{
    /// <summary>
    /// Card for converting one or multiple MyCard-objects into a string that holds the commonly used card description.
    /// </summary>
    class CardConverter
    {



        public static string convertCards(List<Card> cards)
        {
            string name = "";

            foreach (Card card in cards)
            {
                name += convertCard(card) + " ";
            }

            return name.Substring(0, name.Length - 1); //Leaving out the additional space
        }




        private static string convertCard(Card card)
        {
            string name = "";

            string value = "";

            switch (card.Value)
            {
                case Value.Zwei:
                    value = "2";
                    break;
                case Value.Drei:
                    value = "3";
                    break;
                case Value.Vier:
                    value = "4";
                    break;
                case Value.Fünf:
                    value = "5";
                    break;
                case Value.Sechs:
                    value = "6";
                    break;
                case Value.Sieben:
                    value = "7";
                    break;
                case Value.Acht:
                    value = "8";
                    break;
                case Value.Neun:
                    value = "9";
                    break;
                case Value.Zehn:
                    value = "T";
                    break;
                case Value.Bube:
                    value = "J";
                    break;
                case Value.Dame:
                    value = "Q";
                    break;
                case Value.König:
                    value = "K";
                    break;
                case Value.Ass:
                    value = "A";
                    break;
                default:
                    throw new Exception("Error Karte");
            }

            string suit = "";

            switch (card.Suit)
            {
                case (Suit.Karo):
                    suit = "d";
                    break;
                case (Suit.Herz):
                    suit = "h";
                    break;
                case (Suit.Pik):
                    suit = "s";
                    break;
                case (Suit.Kreuz):
                    suit = "c";
                    break;
                default:
                    throw new Exception("Error Karte");

            }

            name = value + suit;

            return name;
        }


    }
}
