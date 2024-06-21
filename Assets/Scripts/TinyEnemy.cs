using UnityEngine;
using System.Collections;

public class TinyEnemy : MonoBehaviour {

    public AudioClip DeathSound;
    public GameObject DeathEffect;

    private float aggroRange = 200;
    private float moveSpeed = 60;
    private float turnSpeed = 3;
    private Light spinLight;

    public Playpen playpen;

    private bool isActive = false;

    private GameObject dude;
    private float originalY;

	void Start () {
        dude = GameObject.Find("Dude");
        originalY = transform.position.y;
        spinLight = GetComponentInChildren<Light>();
        spinLight.enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (dude == null)
            return;
        Vector3 moveAmount = Vector3.zero;
	    // if he's within agro range, turn towards him & move towards him
        Vector3 dudeDir = dude.transform.position - transform.position;
        if (Vector3.Magnitude(dudeDir) <= aggroRange)
        {
            Quaternion targetRot = Quaternion.LookRotation(dudeDir);
            if(isActive)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
            moveAmount += (transform.forward * moveSpeed * Time.deltaTime);
        }
        // kill a move that would put us outside our playpen
        moveAmount = playpen.Contain(transform.position, moveAmount);
        moveAmount.y = originalY - transform.position.y;

        if(isActive)
            transform.position += moveAmount;
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("TinyEnemy collided with " + col.gameObject.name);
        // if this is a bullet, we die
        if (col.gameObject.GetComponent<Bullet>() != null)
        {
            AudioSource.PlayClipAtPoint(DeathSound, Camera.main.transform.position);
            if (DeathEffect)
            {
                Instantiate(DeathEffect, transform.position, transform.rotation);
            }
            Destroy(gameObject);
            Destroy(col.gameObject);
            Destroy(this);
        }
        else
        {
            Debug.Log("Skipping!");
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // let them know an enemy hit (they might not care)
        col.gameObject.SendMessage("EnemyHit", gameObject, SendMessageOptions.DontRequireReceiver);
    }
    void PlayerCollide(GameObject player)
    {
        // let them know that colliding with us was bad!
        player.SendMessage("EnemyHit", gameObject);
    }
    public void SetActive(bool active)
    {
        isActive = active;
        spinLight.enabled = active;
    }
}
