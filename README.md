# Siege Nebula
Game Project for CSE381  
## Overview  
**Siege Nebula** is a 3D RTS(Real time strategy) game composed of multiple stages. Players act as a space commanders in charge of allocating the units on different planets. The objective is to take down all the enemy planets.
## Platform  
We are using Unity for this game project. The final game will be playable on PC.  
## Game Mechanics
1. **Planets:** (4 Functions)
  1. **Holding Territory:**
    1. *Planet States* (From player perspective):
      1. Player - You have possession.
      2. Enemy - Enemy possession.
      3. Contested - Player or Enemy have units on the oppositions planet.
      4. Neutral - No one has possession.
    2. *Planet Acquisition:*
      1. At the start of the game the player and enemy will begin with a certain number of planets
      in their possession based on the map.
      2. If the player has soldiers on a Neutral planet, then after X seconds the planet goes into the Player state.
    3. *Losing Planet:*
      1. The player has no soldiers or engineers on a Player planet for X minutes, then the planet goes to the Neutral state.
      2. The enemy is occupying your planet with soldiers for X seconds without any of your soldiers present, then the planet goes into
       the Neutral state.
    4. *Defending a Planet:*
      1. The enemy has soldiers present on your planet. This planet is now in the Contested state. If you eliminate the enemy soldiers then the planet goes to the Player state after X seconds.
  2. **Resource Gathering:**
    1. Resources can only be gathered on Resource planets or Hybrid planets (Soldier and Unit planets) in your possession.
    2. Resources can only be gathered when an engineers are on the planet.
  3. **Soldier Production:**
    1. Soldiers can only be produced on Soldier planets or Hybrid planets (Soldier and Unit planets) in your possession.
    2. Soldiers have a cost and build time.
  4. **Ship Production:**
    1. Ships can be produced on any planet in your possession.
    2. Ships have a cost and build time.
2. **Game Units:**
  1. **Soldiers:**
    1. *Overview:* These are the units that will be transported by ships and land on planets to fight or occupy a planet. They are the only units that can capture planets. 
    2. *Properties:*
      1. Health - Amount of damage a unit can receive before death.
      2. Combat Power - A measure of the units combat power.
      3. Cost - Amount of resources required to train unit.
      4. Build Time - Amount of time required to train unit.
    3. *Movement:* Soldiers can only be transported onto or off of your ships.
  2. **Engineers:**
    1. *Overview:* These are the units that will gather resources from Resource planets.
    2. *Properties:*
      1. Gathering Speed - Rate at which resources are gathered.
      2. Cost - Amount of resources required to train unit.
      3. Build Time - Amount of time required to train unit.
    3. *Movement:* Engineers can only move on onto and off of ships.
    4. *Changing Allegiance:* Enginners can be captured by the enemy if they are on a planet that changes to the Enemy state. They become the enemies units.
  3. **Ships:**
    1. *Overview:* These are the units that transport soldiers and engineers between planets.
    2. *Properties:*
      1. Movement Speed - The rate with which the ship can travel between planets.
      2. Unit Capacity - The amount of soldiers or engineers that the ship can transport at once.
      3. Durability - The distance the ship can travel before breaking down.
      4. Cost - Amount of resources required to build the ship.
      5. Build Time - Amount of time required to build the ship.
    3. *Movement:* Ships can only along fxed paths between planets. These paths will typically ajoin 2 or 3 planets based on level configuration. 
    4. *Unit Transport:* Units (Soldiers or Engineers) can be loaded onto the ship up to their capacity. This process is automated and takes a X seconds per unit to load. Unloading units from a ship is completed in X seconds and all units are unloaded at once.
3. **Economy:**
4. **Combat:**
