 ![Lobby](Sabodash%20Unity/Images/lobby.png)

SABODASH is a local multiplayer infinite runner made in collaboration with [Reece Dubin](https://github.com/RDubinNU) and [Felipe Jannarone](https://github.com/felipejannarone). The goal of the game is to collect sabotages that allow you to disadvantage your friends, and eventually be the last person standing. SABODASH supports as many players as the number of controllers that can connect to your computer. 

To play the game, clone this repo and open the Sabotage.exe file within the [Sabodash Build](https://github.com/julio-villa/Sabodash/tree/main/Sabodash%20Build) directory. If you are on MacOS or otherwise unable to open .exe files, open the DefaultRunning.unity scene in [Sabodash Unity/Assets/Scenes](https://github.com/julio-villa/Sabodash/tree/main/Sabodash%20Unity/Assets/Scenes) within Unity and run the game.
## How to play

Each player will have their own form of control.  Most players will be playing on a game controller. One player may play using the computer's keyboard, and their controls will be different.

MOVE LEFT/RIGHT: Moves the character left or right.
- Controller: Joystick or d-pad
- Keyboard: A/D or LEFT/RIGHT arrows


JUMP AND FLY: Causes the player to jump when on the ground, or to fly when in the air.
- Controller: West face button (rightmost button)
- Keyboard: W or UP arrow


SELECT: When in the lobby, cycles the player's color through all available options.
- Controller: L/R Triggers OR L/R Shoulder buttons
- Keyboard: Q/E or PERIOD/COMMA


DEPLOY/READY: When in the lobby, toggles whether the player is "ready" to start the game.  When in a round this button is used to deploy a sabotage.
- Controller: North face button (topmost button)
- Keyboard: 2 or SHIFT

## Rounds

SABODASH is played in multiple rounds. Before a round starts, all players can move freely around the lobby aimlessly. Each player can use the SELECT buttons to pick a color of their choosing. Once finished, players can press their READY button to signify that they are ready to begin.  Once all players are ready, a round begins.

When a round begins, the screen will begin to scroll at a slow pace. The aim of the game is to stay on the screen longer than all other players as various obstacles scroll past the screen. When all but one players have fallen off of the screen, that last player is declared the winner of the round! All players are returned to the lobby, and the winning player's win counter increases by 1, and the SABODASH title will change to match their color. 

Over the course of many rounds, players will battle to have the most overall wins. The player (or players during a tie) with the most wins will have a crown above their character while in the lobby. Whoever has the most wins at the end of a play session wins!

## Sabotages

As players fly around the various levels, they will encounter small rainbow-flashing collectibles. These, rather than being power-ups for the player who collected it, are SABOTAGES, which influence all other players. To use as sabotage, press the DEPLOY button, and watch the chaos unfold! The sabotages are as follows:

- Cause others to grow to twice their size and move slower!
- Change all other players' colors to gray, so nobody knows who is who!
- Invert their gravity!
- Swap their left and right controls!
- Make them extremely bouncy!
- Lock other players' horizontal movement!
- Teleport all other players to you!
