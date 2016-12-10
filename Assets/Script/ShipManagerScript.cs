using UnityEngine;
using System.Collections;

public class ShipManagerScript : Singleton<ShipManagerScript> {

    public ShipScript[] playerShipPrefabs;
    public ShipScript[] enemyShipPrefabs;



    private ShipScript CreateShip(ShipScript prefab)
    {
        ShipScript ship = Instantiate(prefab) as ShipScript;
        ship.gameObject.SetActive(true);
        ship.transform.SetParent(ManagerScript.Instance.shipContainer.transform);
        ship.GetShipRenderer().enabled = false;
        return ship;
    }
    private bool HasLevel(ShipScript[] shipPrefabs, int level)
    {
        return level >= 0 && level < shipPrefabs.Length;
    }
    public ShipScript CreateShip(AbstractPlanet planet, int level)
    {
        ShipScript ship = null;
        AbstractPlanet.Ownership ownership = planet.planetOwnership;
        if (ownership == AbstractPlanet.Ownership.Player && HasLevel(playerShipPrefabs, level))
        {
            ship = CreateShip(playerShipPrefabs[level]);
            ship.shipOwnership = AbstractPlanet.Ownership.Player;
        }

        if (ownership == AbstractPlanet.Ownership.Enemy && HasLevel(enemyShipPrefabs, level))
        {
            ship = CreateShip(enemyShipPrefabs[level]);
            ship.shipOwnership = AbstractPlanet.Ownership.Enemy;
        }

        ship.dockedPlanet = planet;
        return ship;
    }
}
