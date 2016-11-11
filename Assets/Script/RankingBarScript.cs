using UnityEngine;
using System.Collections;


public struct RankUnit
{
    public GameObject sprite;
    public Vector3 location;
    public bool isLit;
}
public class RankingBarScript : MonoBehaviour {

    private RankUnit[] ranks = new RankUnit[10]; // maximum 10 ranks
    private int count = 0;
    private int _currentRank = 5;

    public int maxRank = 5;
    public int currentRank {
        get { return _currentRank; }
        set { _currentRank = value; UpdateRank(); }
    }
    public bool hideShadow = false;
    public GameObject unitSprite;
    public Color activeColor = Color.yellow;
    public Color shadowColor = Color.white;



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
        UpdateRank();
	}

    void UpdateRank()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject rankUnit = ranks[i].sprite.gameObject;
            SpriteRenderer renderer = rankUnit.GetComponent<SpriteRenderer>();
            if (i < _currentRank)
            {
                if (hideShadow)
                {
                    rankUnit.SetActive(true);
                }
                renderer.color = activeColor;
            } else
            {
                if (hideShadow)
                {
                    rankUnit.SetActive(false);
                }
                else
                {
                    renderer.color = shadowColor;
                }

            }
        }
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
}
