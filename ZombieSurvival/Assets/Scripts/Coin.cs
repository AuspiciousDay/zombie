using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class Coin : MonoBehaviour, IItem
{
    private GameManager gameManager;
    private int score = 100;

    public UnityEvent OnUse { get; set; } = new UnityEvent();

    private void Start()
    {
        GameObject findGo = GameObject.FindWithTag("GameController");
        gameManager = findGo.GetComponent<GameManager>();
    }

    public void Use(GameObject target)
    {
        gameManager.AddScore(score);
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

public int score = 10;

public void Use(GameObject target)
{
    Debug.Log(Coin);
    gameObject.SetActive(false);
    OnUse.Invoke();
}

*/
