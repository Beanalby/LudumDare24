using UnityEngine;
using System.Collections;

public class WeaponHandler : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip ShootSound;
    public AudioClip swapSound;

    private Upgrade currentAction;
    private DudeController dc;
    private GameDriver gd;

    private float lastFired = -100;
    private float fireCooldown = 1;

    public Texture[] weaponlistJump;
    public Texture[] weaponlistShoot;
    public Texture[] weaponlistShield;

    void Start()
    {
        currentAction = Upgrade.None;
        dc = gameObject.GetComponent<DudeController>();
        gd = GameObject.Find("GameDriver").GetComponent<GameDriver>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
            handleFireDown();
        if(Input.GetButtonUp("Fire1"))
            handleFireUp();
        handleSwap();
    }
    void handleFireDown()
    {
        if (!Input.GetButtonDown("Fire1"))
            return;

        switch (currentAction)
        {
            case Upgrade.None:
                return;
            case Upgrade.Shoot:
                shoot();
                return;
            case Upgrade.Shield:
                dc.ShieldOn();
                return;
        }
    }

    void handleFireUp()
    {
        switch (currentAction)
        {
            case Upgrade.Shield:
                dc.ShieldOff();
                break;
        }
    }
    void shoot()
    {
        // Debug.Log("Button was pressed, is " + (lastFired + fireCooldown) + " gt " + Time.time);
        if (lastFired + fireCooldown > Time.time)
            return;
        // FIRE!    
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0;
        if (plane.Raycast(ray, out hitdist))
        {
            Vector3 point = ray.GetPoint(hitdist);
            Vector3 bulletDir = point - transform.position;
            Quaternion bulletRotation = Quaternion.LookRotation(bulletDir);
            Instantiate(bullet, transform.position, bulletRotation);
            AudioSource.PlayClipAtPoint(ShootSound, Camera.main.transform.position);
            lastFired = Time.time;
        }
    }

    void handleSwap()
    {
        string[] inputList = {"WeaponJump", "WeaponShoot", "WeaponShield"};
        Upgrade[] upgradeList = {Upgrade.Jump, Upgrade.Shoot, Upgrade.Shield};
        for(int i=0;i<inputList.Length; i++)
        {
            if(!Input.GetButtonDown(inputList[i]))
                continue;
            Debug.Log("GetButtonDown for [" + inputList[i] + " is ok!");
            if(!dc.HasUpgrade(upgradeList[i]))
                continue;
            Debug.Log("HasUpgrade for [" + inputList[i] + " is ok!");
            SetActive(upgradeList[i]);
            AudioSource.PlayClipAtPoint(swapSound, Camera.main.transform.position);
        }
    }

    public void SetActive(Upgrade up)
    {
        Debug.Log("Swapping to " + up);
        currentAction = up;
    }

    public Upgrade GetCurrentAction()
    {
        return currentAction;
    }

    public void OnGUI()
    {
        GUI.skin = gd.skin;
        DrawWeaponItem(weaponlistJump, Upgrade.Jump, 0);
        DrawWeaponItem(weaponlistShoot, Upgrade.Shoot, 1);
        DrawWeaponItem(weaponlistShield, Upgrade.Shield, 2);
    }

    public void DrawWeaponItem(Texture[] texList, Upgrade up, int offset)
    {
        Texture tex;
        if (!dc.HasUpgrade(up))
            return;
        if(currentAction == up)
            tex = texList[0];
        else
            tex = texList[1];

        Rect r = new Rect(offset * tex.width, Screen.height - tex.height, tex.width, tex.height);
        GUI.DrawTexture(r, tex);
    }
}