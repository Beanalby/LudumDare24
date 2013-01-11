using UnityEngine;
using System.Collections;

public class Playpen : MonoBehaviour {

    public int width=100;
    public int  length=100;
    private BoxCollider bc;
    private TinyEnemy[] enemies;
    private AudioSource source;

    private GameObject dude;
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        bc.size = new Vector3(width, 100, length);
        dude = GameObject.Find("Dude");
        enemies = gameObject.GetComponentsInChildren<TinyEnemy>();
        source = GetComponent<AudioSource>();
    }
    //make sure 
    public Vector3 Contain(Vector3 pos, Vector3 move) {
        if (move.x > 0 && pos.x + move.x > transform.position.x + width / 2)
            move.x = 0;
        if (move.x < 0 && pos.x + move.x < transform.position.x - width / 2)
            move.x = 0;
        if (move.z > 0 && pos.z + move.z > (transform.position.z + (length / 2)))
            move.z = 0;
        if (move.z < 0 && pos.z + move.z < transform.position.z - length / 2)
            move.z = 0;
        return move;
	}

    void OnDrawGizmos()
    {
        Vector3 bottomLeft = transform.position;
        bottomLeft += new Vector3(-width / 2, 0, -length / 2);
        Vector3 topLeft = transform.position;
        topLeft += new Vector3(-width / 2, 0, length / 2);
        Vector3 bottomRight = transform.position;
        bottomRight += new Vector3(width / 2, 0, -length / 2);
        Vector3 topRight = transform.position;
        topRight += new Vector3(width / 2, 0, length / 2);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(bottomRight, topRight);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == dude)
        {
            bool hasEnemy = false;
            foreach (TinyEnemy te in enemies)
                if (te != null)
                {
                    te.SetActive(true);
                    hasEnemy = true;
                }
            if (hasEnemy)
                source.Play();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject == dude)
        {
            foreach (TinyEnemy te in enemies)
                if(te != null)
                    te.SetActive(false);
            source.Stop();
        }
    }
}
