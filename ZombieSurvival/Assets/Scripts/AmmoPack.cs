using UnityEngine;
using UnityEngine.Events;

public class AmmoPack : MonoBehaviour, IItem
{
    public int ammoPack = 80;

    public UnityEvent OnUse { get; set; } = new UnityEvent();

    public void Use(GameObject target)
    {
        PlayerShooter shooter = target.GetComponent<PlayerShooter>();
        shooter.gun.AddAmmoRemain(ammoPack);
        gameObject.SetActive(false);
        OnUse.Invoke();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Use(other.gameObject);
        }
    }
}
/*
public int ammo = 50;

public void Use(GameObject target)
{
    PlayerShooter shooter = target.GetComponent<PlayerShooter>();
    if(shooter == null || shooter.gun == null)
    {
        return;
    }
    shooter.gun.AddAmmo(ammo);
    gameObject.SetActive(false);
    OnUse.Invoke();
}

*/