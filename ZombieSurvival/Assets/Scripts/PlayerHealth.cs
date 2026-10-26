using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public readonly static int IdDie = Animator.StringToHash("Die");

    public Image uiHealth;
    public Image uiMaxHealth;

    public AudioClip hitClip;
    public AudioClip deathClip;
    
    private AudioSource playerAudioSource;
    private Animator playerAnimator;
    
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        playerMovement.enabled = true;
        playerInput.enabled = true;

        uiMaxHealth.gameObject.SetActive(true);
        uiHealth.gameObject.SetActive(true);
        uiHealth.fillAmount = Health / startingHealth;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!IsDeath)
        {
            playerAudioSource.PlayOneShot(hitClip);    
        }
        
        base.OnDamage(damage, hitPoint, hitNormal);
        uiHealth.fillAmount = Health / startingHealth;
    }

    public override void Die()
    {
        base.Die();

        playerAnimator.SetTrigger(IdDie);
        playerAudioSource.PlayOneShot(deathClip);

        playerMovement.enabled = false;
        playerInput.enabled = false;

        uiMaxHealth.gameObject.SetActive(false);
        uiHealth.gameObject.SetActive(false);
    }
}
