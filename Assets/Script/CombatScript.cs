using UnityEngine;
using System.Collections;

/**
 * This script will be attached to each planet to run the Combat.
 * Combat should only receive updates when the planet is in Contested state.
 * This is a round based system where the attacker strikes first. However,
 * the defender has a slight defensive advantage. 
**/

public class CombatScript : MonoBehaviour {

	private AbstractPlanet planet;
	private SoldierUnit playerSoldiers;
	private SoldierUnit enemySoldiers;
	private float timer;
	private bool isPlayerAttacker;

	private const float ROUND_TICK = 1.5f;
	private const float SOLDIERS_PER_GROUP = 25.0f;
	private const float ATTACK_ROLL_MAX = 20.0f;
	private const float ATTACK_ROLL_MIN = 1.0f;
	private const float DAMAGE_ROLL_MAX = 6.0f;
	private const float DAMAGE_ROLL_MIN = 1.0f;
	private const int DEFENDER_ADVANTAGE = 1;

	// Use this for initialization
	void Start () {
		planet = GetComponent<AbstractPlanet> ();
		this.playerSoldiers = planet.playerSoldiers;
		this.enemySoldiers = planet.enemySoldiers;
		timer = 0.0f;
		isPlayerAttacker = (planet.planetOwnership == AbstractPlanet.Ownership.Enemy);
	}
	
	// Update is called once per frame
	void Update () {
		if (!planet.isContested)
			return;
		
		timer += Time.deltaTime;
		if (timer >= ROUND_TICK) {
			
			StartRound();

			Combat ();

			UpdatePlanetData ();

			timer = 0.0f;
		}

	}

	void StartRound() {
		this.playerSoldiers = planet.playerSoldiers;
		this.enemySoldiers = planet.enemySoldiers;
	}

	void Combat() {
		int attackerDamageRolls = DetermineDamageRolls(isPlayerAttacker, true);
		int defenderDamageRolls = DetermineDamageRolls(!isPlayerAttacker, false);
		DetermineDamage(attackerDamageRolls, isPlayerAttacker);
		DetermineDamage(defenderDamageRolls, !isPlayerAttacker);
	}

	void UpdatePlanetData() {
		planet.playerSoldiers = this.playerSoldiers;
		planet.enemySoldiers = this.enemySoldiers;
	}

	void DetermineDamage(int damageRolls, bool isPlayerTurn) {
		int totalDamage = 0;
		if (isPlayerTurn) {
			for (int i = 0; i < damageRolls; i++) {
				int rand = (int)Random.Range (DAMAGE_ROLL_MIN, DAMAGE_ROLL_MAX);
				totalDamage += (rand + playerSoldiers.attackMod);
			}
			enemySoldiers.soldierCount -= totalDamage;
			if (enemySoldiers.soldierCount < 0)
				enemySoldiers.soldierCount = 0;
		} else {
			for (int i = 0; i < damageRolls; i++) {
				int rand = (int)Random.Range (DAMAGE_ROLL_MIN, DAMAGE_ROLL_MAX);
				totalDamage += (rand + enemySoldiers.attackMod);
			}
			playerSoldiers.soldierCount -= totalDamage;
			if (playerSoldiers.soldierCount < 0)
				playerSoldiers.soldierCount = 0;
		}
	}

	int DetermineDamageRolls(bool isPlayerTurn, bool isAttacker) {
		int result = 0;
		if (isPlayerTurn) {
			int rolls = Mathf.Max(1, Mathf.FloorToInt(playerSoldiers.soldierCount / SOLDIERS_PER_GROUP));
			for (int i = 0; i < rolls; i++) {
				int rand = (int)Random.Range (ATTACK_ROLL_MIN, ATTACK_ROLL_MAX);
				if ((enemySoldiers.defense + enemySoldiers.defenseMod + (isAttacker ? DEFENDER_ADVANTAGE : 0)) <= rand)
					result++;
			}
		} else {
			int rolls = Mathf.Max(1, Mathf.FloorToInt(enemySoldiers.soldierCount / SOLDIERS_PER_GROUP));
			for (int i = 0; i < rolls; i++) {
				int rand = (int)Random.Range (ATTACK_ROLL_MIN, ATTACK_ROLL_MAX);
				if ((playerSoldiers.defense + playerSoldiers.defenseMod + (isAttacker ? DEFENDER_ADVANTAGE : 0)) <= rand)
					result++;
			}
		}
		return result;
	}
}
