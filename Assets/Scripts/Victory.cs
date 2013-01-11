using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {

    private GameDriver gd;
    void Start() {
        gd = gameObject.GetComponent<GameDriver>();
    }

    void OnGUI()
    {
        GUI.skin = gd.skin;
        GUI.Box(new Rect(10, 10, Screen.width - 10, 75), "You escaped!\nA winner is you!");
    }
}
