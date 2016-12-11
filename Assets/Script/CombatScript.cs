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
	private float timer;

	private const float ROUND_TICK = 1.5f;
	private const float SOLDIERS_PER_GROUP = 25.0f;
	private const float ATTACK_ROLL_MAX = 20.0f;
	private const float ATTACK_ROLL_MIN = 1.0f;
	private const float DAMAGE_ROLL_MAX = 6.0f;
	private const float DAMAGE_ROLL_MIN = 1.0f;

	private ManagerScript gameManager;

	// Use this for initialization
	void Start () {
		gameManager = ManagerScript.Instance;
		planet = GetComponent<AbstractPlanet> ();
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!planet.isContested)
			return;
		
		timer += Time.deltaTime;
		if (timer >= ROUND_TICK) {
			PlaySoundFx ();
			Combat ();
			timer = 0.0f;
		}

	}

	void PlaySoundFx() {
		gameManager.audioManager.PlaySound ("bassBoom");
		bool playShortBurst = Random.value < 0.5f;
		if (playShortBurst) {
			bool playFirst = Random.value < 0.5f;
			if (playFirst)
				gameManager.audioManager.PlaySound ("shortBurst1");
			else
				gameManager.audioManager.PlaySound ("shortBurst2");
		}
	}
		
	void Combat() {
		int playerDamageRolls = DetermineDamageRolls(gameManager.GetEnemyStats(), planet.playerSoldiers);
		int enemyDamageRolls = DetermineDamageRolls(gameManager.GetPlayerStats(), planet.enemySoldiers);
		DetermineDamage(playerDamageRolls, gameManager.GetPlayerStats(), true);
		DetermineDamage(enemyDamageRolls, gameManager.GetEnemyStats(), false);
	}

	void DetermineDamage(int damageRolls, SoldierStats attackerStats, bool isPlayerAttacking) {
		int totalDamage = 0;
		for (int i = 0; i < damageRolls; i++) {
			int rand = Mathf.CeilToInt(Random.Range (DAMAGE_ROLL_MIN, DAMAGE_ROLL_MAX));
			totalDamage += (rand + attackerStats.attackMod);
		}
		if (isPlayerAttacking)
			planet.EnemyTakeDamage (totalDamage);
		else
			planet.PlayerTakeDamage (totalDamage);

	}

	int DetermineDamageRolls(SoldierStats defenderStats, int attackerUnits) {
		int result = 0;
		int rolls = (int)Mathf.Max(1, attackerUnits / SOLDIERS_PER_GROUP);
		for (int i = 0; i < rolls; i++) {
			int rand = (int)Random.Range (ATTACK_ROLL_MIN, ATTACK_ROLL_MAX);
			if ((defenderStats.defense + defenderStats.defenseMod) <= rand)
				result++;
		}
		return result;
	}
}
