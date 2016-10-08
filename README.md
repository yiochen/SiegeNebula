# CSE381
game project for CSE381  
## Overview  
This is a 3D RTS(Real time strategy) game composed of multiple stages. Players act as a space commanders in charge of allocating the units on different planets. The objective is to take down all the enemy planets.
## Platform  
We are using Unity for this game project. The final game will be playable on PC.  
## Rules  
You play against another player or against AI. At the start of each stage. You will be presented with a universe with multiple planets. Each planet has a color, indicating the ownership of it. Some planets are yours, some are your opponent's, some are neutual. At any time, you can: 

1. **move some amount of your units from one planet to another planets.** You units will go through a space travel, which take a certain amount of time to arrive at destination, depending on the distance and the amount of units.
2. **Increase the defence of a planet of yours.** You can install shield on your planets, so that when enermy units want to land on the planet, they will receive damage, and some of their units will die. 
3. More to be decided

When some unit from one team, A, land of a neutual planet, the planet of be turned into team A's territory. If one planet has units from two side, they will fight until there are only inits from one team left. If a team manage to get rid of all opponent unit, this team claim the ownership of this planet.  

## Terms
**Planets**: Planet is a base for storing units. A planet can belong to on team or no team. 
**Resource**: Each planet has a set amount of resource, 
