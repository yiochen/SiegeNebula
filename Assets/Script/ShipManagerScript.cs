using UnityEngine;
using System.Collections;

public class ShipManagerScript : Singleton<ShipManagerScript> {

    public ShipScript playerShipPrefab;
    public ShipScript enemyShipPrefab;


    private ShipScript CreateShip(ShipScript prefab)
    {
        ShipScript ship = Instantiate(prefab) as ShipScript;
        ship.gameObject.SetActive(true);
        ship.transform.SetParent(ManagerScript.Instance.shipContainer.transform);
        ship.GetShipRenderer().enabled = false;
        return ship;
    }
	public ShipScript CreatePlayerShip(AbstractPlanet planet)
    {
        ShipScript ship = CreateShip(playerShipPrefab);
        ship.shipOwnership = AbstractPlanet.Ownership.Player;
		ship.dockedPlanet = planet;
        return ship;
    }

	public ShipScript CreateEnemyShip(AbstractPlanet planet)
    {
        ShipScript ship = CreateShip(enemyShipPrefab);
        ship.shipOwnership = AbstractPlanet.Ownership.Enemy;
		ship.dockedPlanet = planet;
        return ship;
    }
}
