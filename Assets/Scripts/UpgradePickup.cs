using UnityEngine;
using System.Collections;

public enum Upgrade { None, Jump, Shoot, Shield }

public class UpgradePickup : MonoBehaviour {

    public AudioClip upgradeSound;

    private GameObject dude;
    private GameDriver gd;    
    public Upgrade thisUpgrade;

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
        //Debug.Log(gameObject.name + " collided with " + col.gameObject.name);
        //Debug.Log("I'm at " + gameObject.transform.position + ", dude=" + col.gameObject.transform.position + ", dist=" + Vector3.Distance(gameObject.transform.position, col.gameObject.transform.position));
        if (col.gameObject != dude)
            return;
        // keeps triggering remotely, no idea why.  Bail if it's too far.
        if (Vector3.Distance(gameObject.transform.position, col.gameObject.transform.position) > 10)
            return;
        Debug.Log("Adding upgrade " + thisUpgrade);
        DudeController dc = dude.GetComponent<DudeController>();
        dc.AddUpgrade(thisUpgrade);
        string upgradeMessage="";
        switch (thisUpgrade)
        {
            case Upgrade.Jump:
                upgradeMessage = "You evolved jumping!\nSelect it with '1' and Left Click to leap!";
                break;
            case Upgrade.Shield:
                upgradeMessage = "You evolved shielding!\nSelect it with '3' and Left Click to become invincible!";
                break;
            case Upgrade.Shoot:
                upgradeMessage = "You evolved shooting!\nSelect with '2' aim with the mouse, Left Click to lob goo!";
                break;
        }
        gd.ShowMessage(upgradeMessage);
        AudioSource.PlayClipAtPoint(upgradeSound, Camera.main.transform.position);
        Destroy(gameObject);
    }
}
