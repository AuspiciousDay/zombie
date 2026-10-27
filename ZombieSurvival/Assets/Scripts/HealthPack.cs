using UnityEngine;
using UnityEngine.Events;

public class HealthPack : MonoBehaviour, IItem
{
    public float heal = 10f;

    public UnityEvent OnUse { get; set; } = new UnityEvent();

    public void Use(GameObject target)
    {
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        playerHealth.Heal(heal);
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

public int heal = 50;

public void Use(GameObject target)
{   
    LivingEntity livingEntity = target.GetComponent<LivingEntity>();
    if(LivingEntity != null)
    {
        livingEntity.AddHealth(heal);
        gameObject.SetActive(false);
        OnUse.Invoke();
    }
}

*/
