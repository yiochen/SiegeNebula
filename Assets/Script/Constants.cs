﻿class Layers
{
    public const int DEFAULT = 0;
    public const int TRANSPARENT_FX = 1;
    public const int IGNORE_RAYCAST = 2;

    public const int WATER = 4;
    public const int UI = 5;

    public const int CAMERA = 8;
    public const int MANAGER = 9;

}

class AI {
	public const float THINK_TIME_EASY = 3.0f;
}

class Indices {
	public const int SHIP_PLAYER = 0;
	public const int SHIP_ENEMY = 1;
}

class GamePlay 
{
	public const int SHIP_COST = 300;
	public const int SHIP_CAPACITY = 100;
	public const int SOLDIER_COST = 10;
	public const int ENGINEER_COST = 3;

	public const float PLANET_TICK = 1.5f;
	public const float PLANET_CHANGE = 5.0f;
	public const int PLANET_RESOURCE_STD = 1200;
}