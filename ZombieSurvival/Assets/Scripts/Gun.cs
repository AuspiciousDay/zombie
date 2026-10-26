using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum State // 열거형, 값형
    {
        Ready,
        Empty,
        Realoading,
    }

    public State CurrentState {get; private set;}

    public Transform fireTransform;

    public ParticleSystem muzzleFlasgEffect;
    public ParticleSystem shellEjectEffect;

    private LineRenderer bulletLineRenderer;

    private AudioSource gunAudioPlayer;

    public GunData gunData;
    private float fireDistance = 50f;

    private int ammoRemain;
    private int magAmmo;

    private float lastFireTime;

    public UIManager uiManager;

    private void Awake()
    {
        bulletLineRenderer = GetComponent<LineRenderer>();
        gunAudioPlayer = GetComponent<AudioSource>();

        bulletLineRenderer.positionCount =2;
        bulletLineRenderer.enabled = false;
    }

    private void Start()
    {
        uiManager.SetAmmoText(magAmmo,ammoRemain);
    }

    private void OnEnable()
    {
        ammoRemain = gunData.startAmmoRemain;
        magAmmo = gunData.magCapacity;
        uiManager.SetAmmoText(magAmmo,ammoRemain);

        CurrentState = State.Ready;
        lastFireTime = 0f;
    }

    public void Fire()
    {
        if(CurrentState == State.Ready && Time.time >= lastFireTime + gunData.timeBetFire)
        {
            lastFireTime = Time.time;
            Shoot();
        }
    }

    public LayerMask targetLayer;

    private void Shoot()
    {
        Vector3 hitPosition;

        Ray ray = new Ray(fireTransform.position, fireTransform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, fireDistance, targetLayer))
        {
            hitPosition = hit.point;
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if(target != null)
            {
                target.OnDamage(gunData.damage, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        StartCoroutine(CoShotEffect(hitPosition));

        magAmmo--;
        uiManager.SetAmmoText(magAmmo,ammoRemain);

        if(magAmmo == 0)
        {
            CurrentState = State.Empty;
        }
    }

    private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        muzzleFlasgEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(gunData.shotClip);

        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.03f);

        bulletLineRenderer.enabled = false;
    }

    public bool Reload() 
    {
        if(CurrentState != State.Realoading && ammoRemain >= 0 && magAmmo != gunData.magCapacity)
        {
            StartCoroutine(CoReload());
            return true;
        }
        return false;
    }
    private IEnumerator CoReload()
    {
        gunAudioPlayer.PlayOneShot(gunData.reloadClip);
        CurrentState = State.Realoading;

        yield return new WaitForSeconds(gunData.reloadTime);

        int ammoToFill = Mathf.Min(gunData.magCapacity - magAmmo, ammoRemain);

        magAmmo += ammoToFill;
        ammoRemain -= ammoToFill;
        
        /* 
        if(magAmmo == 0)
        {
            ammoRemain -= gunData.magCapacity;
            magAmmo += gunData.magCapacity;
        }
        else if(magAmmo < gunData.magCapacity)
        {
            ammoRemain = ammoRemain - (gunData.magCapacity - magAmmo);
            magAmmo = gunData.magCapacity;
        }
        */

        uiManager.SetAmmoText(magAmmo,ammoRemain);
        CurrentState = State.Ready;
    }
}
