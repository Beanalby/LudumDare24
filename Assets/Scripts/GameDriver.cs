using UnityEngine;
using System.Collections;

public class GameDriver : MonoBehaviour {

    public GUISkin skin;
    public float reloadDelay = 3;

    private GameState state;

    private float guiMessageDuration = 5;
    private string guiMessage;
    private float guiMessageStart;
    public GameObject gameStateDefault;
    // some other stuff

    private float reloadAt;
    void Start()
    {
        guiMessageStart = -1;
        reloadAt = -1;

        GameObject stateObj = GameObject.Find("GameState");
        if (stateObj != null)
        {
            state = stateObj.GetComponent<GameState>();
        }
        else
        {
            stateObj = new GameObject("GameState");
            state = stateObj.AddComponent<GameState>();
        }

        Transform spawnTransform = null;
        SpawnPoint[] points = (SpawnPoint[])GameObject.FindObjectsOfType(typeof(SpawnPoint));
        foreach(SpawnPoint sp in points)
        {
            if(sp.fromScene == state.previousScene)
            {
                spawnTransform = sp.transform;
                break;
            }
        }
        if(!spawnTransform)
        {
            Debug.Log("Couldn't find spawn point coming from scene [" + state.previousScene + "]");
            Debug.Break();
        }

        // set the dude to his proper spawn point, upgrades, & active
        GameObject dude = GameObject.Find("Dude");
        if(dude == null)
        {
            Debug.Log("No dude in scene!");
            return;
        }
        if (dude.transform == null)
        {
            Debug.Log("Dude.transform is null!  That's weird!");
            return;
        }
        dude.transform.position = spawnTransform.position;

        UpgradePickup[] pickups = (UpgradePickup[])GameObject.FindObjectsOfType(typeof(UpgradePickup));
        DudeController dc = GameObject.Find("Dude").GetComponent<DudeController>();
        foreach (Upgrade up in state.upgrades)
        {
            // also remove any upgrades from the scene that we already have
            dc.AddUpgrade(up);
            foreach(UpgradePickup pickup in pickups)
                if(pickup.thisUpgrade == up)
                    Destroy(pickup.transform.parent.gameObject);
        }
        dude.GetComponent<WeaponHandler>().SetActive(state.currentAction);
        if (state.previousScene == "gameStart")
            ShowMessage("You're Squishy, and must escape!\nUse [W] [A] [S] [D] to move!");
	}

    public void LoadScene(string name)
    {
        //grab the list of upgrades we currently have
        GameObject dude = GameObject.Find("Dude");
        if (dude != null)
        {
            DudeController dc = GameObject.Find("Dude").GetComponent<DudeController>();
            state.upgrades = dc.GetUpgrades();
            Debug.Log("Saved upgrades:");
            foreach (Upgrade u in state.upgrades)
            {
                Debug.Log("Have upgrade " + u);
            }
            state.currentAction = dude.GetComponent<WeaponHandler>().GetCurrentAction();
        }
        state.previousScene = Application.loadedLevelName;
        Debug.Log("Previous=" + state.previousScene);
        Application.LoadLevel(name);
    }

    public void ReloadScene()
    {
        // reloads the scene a few seconds after they die.
        // give them a little time to see the effects & mourn.
        reloadAt = Time.time;
    }
    public void ShowMessage(string message) {
        guiMessage = message;
        guiMessageStart = Time.time;
    }

    void Update()
    {
        if (reloadAt != -1 && Time.time > reloadAt + reloadDelay)
            Application.LoadLevel(Application.loadedLevel);
    }

    void OnGUI() {
        GUI.skin = skin;
        guiShowMessage();
    }
    void guiShowMessage()
    {
        // we just needed a comment!
        if (guiMessageStart != -1)
        {
            if (Time.time > guiMessageStart + guiMessageDuration)
            {
                guiMessageStart = -1;
                return;
            }
            GUI.Box(new Rect(10, 10, Screen.width - 10, 75), guiMessage);
        }
    }
}
