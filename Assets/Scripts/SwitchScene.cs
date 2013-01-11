using UnityEngine;
using System.Collections;

public class SwitchScene : MonoBehaviour {

    public string targetScene;
    private GameObject dude;
    private GameDriver gd;

	// Use this for initialization
	void Start () {
        dude = GameObject.Find("Dude");
        gd = GameObject.Find("GameDriver").GetComponent<GameDriver>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject != dude)
            return;
        Debug.Log(gameObject.name + " onTriggerEnter by " + col.gameObject.name);
        gd.LoadScene(targetScene);
    }
}
