using UnityEngine;
using System.Collections;

public class CrusherHead : MonoBehaviour {

    public AudioClip soundUp;
    public AudioClip soundDown;

    private float interval = 2;
    private float height = 80;
    private float duration = .1f;
    private Vector3 bottom, top, currentBottom;
    private GameObject dude;


    private int lastDir;
	// Use this for initialization
	void Start () {
        dude = GameObject.Find("Dude");
        bottom = transform.position;
        currentBottom = bottom;
        top = new Vector3(bottom.x, bottom.y + height, bottom.z);
        lastDir = 1;
	}
	
	void FixedUpdate () {
        Vector3 from, to;
        float percent = (Time.time % 1) / duration;
        if ((Time.time % interval) > interval / 2)
        {
            from = currentBottom;
            to=top;
            // reset the bottom if we changed it
            if(percent >= 1)
                currentBottom = bottom;
            if (lastDir == 0)
            {
                if (soundUp != null)
                    AudioSource.PlayClipAtPoint(soundUp, Camera.main.transform.position);
                lastDir = 1;
            }
        }
        else
        {
            from = top;
            to = currentBottom;
            if (lastDir == 1)
            {
                if (soundDown != null)
                    AudioSource.PlayClipAtPoint(soundDown, Camera.main.transform.position);
                lastDir = 0;
            }
        }
        transform.position = Vector3.Lerp(from, to, percent);
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject != dude)
            return;
        foreach(ContactPoint cp in col.contacts)
        {
            DudeController dc = dude.gameObject.GetComponent<DudeController>();
            if (dc.IsShielded())
            {
                Debug.Log("HE LIVES!");
                // bottom out just a little above him
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + 1,
                    transform.position.z);
                currentBottom = transform.position;
            }
            else
            {
                Debug.Log("HE DIES!");
                dude.GetComponent<DudeController>().Die();
                Destroy(dude);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(gameObject.name + " onTriggerEnter with " + col.gameObject.name);
        Debug.Log("I'm at " + gameObject.transform.localPosition + ", dude=" + col.gameObject.transform.position + ", dist=" + Vector3.Distance(gameObject.transform.position, col.gameObject.transform.position));
    }
}
