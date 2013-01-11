using UnityEngine;
using System.Collections;

public class DudeController : MonoBehaviour {

    public AudioClip jumpSound;
    public AudioClip shieldSound;

    private float turnSpeed = 5f;
    private float moveSpeed = .5f;
    private float gravity = 10f;
    private float jumpDist = 4f;

    private float cameraDistance = 140f;
    private float cameraDeadzone = 30f;
    private float cameraSpeed = .5f;

    public GameObject target;

    private bool isShielded;

    private CharacterController cont;
    private Animation anim;
    private float jumpClipLength, jumpTime;
    private ArrayList upgrades;
    private WeaponHandler wh;

    private Material normalMaterial;
    public Material shieldMaterial;
    private SkinnedMeshRenderer smr;

	// Use this for initialization
	void Start () {
        jumpTime = -1;
        target = GameObject.Find("Target");
        cont = GetComponent<CharacterController>();
        wh = GetComponent<WeaponHandler>();
        isShielded = false;
        upgrades = new ArrayList();
        anim = GetComponentInChildren<Animation>();
        jumpClipLength = anim.GetClip("Jump").length;
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        normalMaterial = smr.materials[0];
	}
	
	// Update is called once per frame
    void Update()
    {
        // fix the jump animation
        if (jumpTime != -1 && jumpTime + jumpClipLength < Time.time)
        {
            Debug.Log("Post-jump going to idle at " + Time.time);
            anim.Play("Idle");
            jumpTime = -1;
        }
        moveCamera();
    }

    void FixedUpdate()
    {
        moveSelf();
    }

    void moveSelf()
    {
        bool didJump = false;
        Vector3 lookDir = Vector3.zero;
        Vector3 thisMove = Vector3.zero;
        // don't allow them to move if they're shielding
        if (!isShielded)
        {
            lookDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            thisMove = lookDir;
            thisMove *= moveSpeed;
            // jump if the press the jump button (space), or fire when jump is selected
            if (cont.isGrounded && (Input.GetButtonDown("Jump") ||
                    (Input.GetButtonDown("Fire1") && wh.GetCurrentAction() == Upgrade.Jump)))
            {
                if (HasUpgrade(Upgrade.Jump))
                {
                    thisMove.y += jumpDist;
                    anim.Play("Jump");
                    AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position);
                    jumpTime = Time.time;
                    didJump = true;
                }
            }
        }
        if (cont.isGrounded && !didJump && !isShielded)
        {
            if (Vector3.Magnitude(thisMove) != 0)
            {
                anim.Play("Walk");
            }
            else
            {
                anim.Play("Idle");
            }
        }
        float yOffset = (cont.velocity.y - gravity);
        thisMove.y += yOffset * Time.deltaTime;
        cont.Move(thisMove);
        if (Vector3.Magnitude(lookDir) != 0)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }
    }

    void moveCamera()
    {
        /* figure out how far the camera is away from the guy's plane */
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
        float hitdist = 0.0f;
        if (plane.Raycast(ray, out hitdist))
        {
            Vector3 point = ray.GetPoint(hitdist);
            if(target != null)
                target.transform.position = point;
            Vector3 dest;
            // move the camera back or forward along its ray enough to give the distance that we want
            dest = ray.GetPoint(hitdist - cameraDistance);

            // move the camrea in the X/Z plane to keep the target on screen
            Vector3 xzDiff = transform.position - point;
            // if it's farther than our deadzone, move the camera in that direction
            if (Vector3.Magnitude(xzDiff) > cameraDeadzone)
            {
                dest += xzDiff;
                // Figure out where on the edge of the deadzone we're trying to get to
                //Ray moveRay = new Ray(point, xzDiff);
                //Vector3 xzTarget = moveRay.GetPoint(cameraDeadzone);
                //// add the xz differences between the camera & the deadzone target to our planned move
                //xzTarget.y = point.y;
                //// Debug.Log("Adding xz offset " + xzTarget);
                //dest += xzTarget - point;
            }
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, dest, cameraSpeed * Time.deltaTime);
        }
    }

    public void ShieldOn()
    {
        isShielded = true;
        Debug.Log("SHIELD ON!");
        smr.materials[0] = shieldMaterial;
        Material[] m = smr.materials;
        m[0] = shieldMaterial;
        smr.materials = m;
        anim.Stop();
        AudioSource.PlayClipAtPoint(shieldSound, Camera.main.transform.position);
    }
    public void ShieldOff()
    {
        isShielded = false;
        Debug.Log("Shield off.");
        Material[] m = smr.materials;
        m[0] = normalMaterial;
        smr.materials = m;
        anim.Play("Idle");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Dude collided with " + collision.gameObject.name);
    }

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.name == "Terrain")
            return;
        obj.SendMessage("PlayerCollide", gameObject, SendMessageOptions.DontRequireReceiver);
    }

    void EnemyHit(GameObject enemy)
    {
        if (!isShielded)
        {
            Debug.Log("Dead at the hand of " + enemy.name);
            Die();
        }
    }

    public void Die()
    {
        GameObject.Find("GameDriver").GetComponent<GameDriver>().ReloadScene();
        Destroy(gameObject);
    }

    public void AddUpgrade(Upgrade up)
    {
        upgrades.Add(up);
        Debug.Log("Added upgrade " + up + ", now there are " + upgrades.Count);
        if (wh.GetCurrentAction() == Upgrade.None)
            wh.SetActive(up);
    }

    public bool HasUpgrade(Upgrade up)
    {
        return upgrades.Contains(up);
    }

    public bool IsShielded() {
        return isShielded;
    }

    public ArrayList GetUpgrades()
    {
        return upgrades;
    }
}
