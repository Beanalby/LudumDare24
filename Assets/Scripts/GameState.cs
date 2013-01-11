using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

    public ArrayList upgrades;
    public Upgrade currentAction;
    public string previousScene;

	// Use this for initialization
	void Awake()
    {
        upgrades = new ArrayList();
        DontDestroyOnLoad(gameObject);
        previousScene = "gameStart";

    }
}