using UnityEngine;
using System.Collections;

public class RaycastShootComplete : MonoBehaviour
{
    public int gunDamage = 1;
    public float fireRate = 0.25f;
    public float weaponRange = 50f;
    public float hitForce = 100f;
    public Transform gunEnd;
    public GameObject scope;

    private Camera fpsCam;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private AudioSource gunAudio;
    private LineRenderer laserLine;
    private float nextFire;

    private float zoom = 25f; // <-- Set a default zoom value
    private float originalFOV;

    void Start()
    {
        originalFOV = Camera.main.fieldOfView;

        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        fpsCam = GetComponentInParent<Camera>();

        if (scope != null)
            scope.SetActive(false); // Make sure scope starts disabled
    }

    void Update()
    {
        // Scope zoom handling
        if (Input.GetButtonDown("Fire2"))
        {
            ScopeZoom();
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            UnZoom();
        }

        // Shooting logic
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            StartCoroutine(ShotEffect());

            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            laserLine.SetPosition(0, gunEnd.position);

            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                laserLine.SetPosition(1, hit.point);

                // Damage enemy if it has ZombieAI
                ZombieAI enemy = hit.collider.GetComponent<ZombieAI>();
                if (enemy != null)
                {
                    enemy.tackdamge(10);
                }

                // Add force if object has rigidbody
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }
            }
            else
            {
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
            }
        }
    }

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();
        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
    }

    void ScopeZoom()
    {
        if (scope != null)
            scope.SetActive(true);
        Camera.main.fieldOfView = zoom;
    }

    void UnZoom()
    {
        if (scope != null)
            scope.SetActive(false);
        Camera.main.fieldOfView = originalFOV;
    }
}
