# pokerBot

## What is it?
pokerBot is an artificial intelligence for Texas Holdem Poker which I created in my free time. It was written in C# and specifically runs on the **Pokerstars** client, but is extensible to every poker client. It was designed to run fully automatically on an unlimited number of poker sessions.

## How does it work?


For gathering the information the pokerBot takes a screenshot of the poker session and analyzes the state of the game via OCR (Google's tesseract) and simple screen scamming. Due to the screen scamming being hard coded, the pokerBot currently only works on a very specific screen resolution (2736x1824). (Todo)


On the first round of betting (preflop) the pokerBot looks up the player's Odds from a look up table, which lists the odds depending on the cards the player has and the ammount of opponents that the player faces. In laters betting rounds odds are calculated via Monte Carlo Simulation. The pokerBot then decides on what to do considering the pot odds.


All interactions with the Pokerstars program are implemented via User32.dll function calls to move the mouse and the .Net SendKeys() to enter the betting ammount.

## Results

Untill now the pokerBot has had very succesfull runs on Pokerstars, despite having a simple AI. I am consistently able to multiply my starting ammount by 10, untill the pokerBot decides to go all in and loses, which represents a major flaw of the AI. 

