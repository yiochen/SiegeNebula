using UnityEngine;
using System.Collections;


public struct RankUnit
{
    public GameObject sprite;
    public Vector3 location;
    public bool isLit;
}
public class RankingBarScript : MonoBehaviour {
    public int maxRank = 5;
    public int currentRank = 5;
    public bool showShadow = false;
    public GameObject unitSprite;
    public Color color = Color.yellow;
    public Color shadowColor = Color.white;

    private RankUnit[] ranks = new RankUnit[10]; // maximum 10 ranks
    private int count = 0;

	// Use this for initialization
	void Start () {
        RankUnit rank;
        bool isOdd = (maxRank % 2 == 1);
        for (int i = 0; i < maxRank; i++)
        {
            rank = AddRank();
            rank.sprite.transform.parent = gameObject.transform;
            rank.sprite.transform.localPosition = new Vector3(i - (maxRank -1.0f) / 2.0f, -0.5f, 0.0f);
            rank.sprite.transform.localRotation = Quaternion.identity;
        }
        SetRank(currentRank);
	}

    public void SetRank(int rank)
    {

    }

    RankUnit AddRank()
    {
        GameObject sprite = Instantiate(unitSprite, Vector3.zero, Quaternion.identity) as GameObject;
        RankUnit rank = new RankUnit();
        rank.sprite = sprite;
        rank.isLit = false;
        rank.location = Vector3.zero;
        ranks[count] = rank;
        count++;
        return rank;
    }

    void Refresh()
    {

    }
}
