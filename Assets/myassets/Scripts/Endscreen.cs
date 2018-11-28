using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class Endscreen : MonoBehaviour {

    public Animator Star1Anim;
    public Animator Star2Anim;
    public Animator Star3Anim;
    public Animator EndScreenBGAnim;
    public Text coinsCollectedText;
    public AudioSource Star1Sound;
    public AudioSource Star2Sound;
    public AudioSource Star3Sound;

    private ItemCounter _itemCounter;
    private int _coinsToFind = 0;

    private const float _MAXTIMERSTAR1 = 0.5f;
    private const float _MAXTIMERSTAR2 = 1f;
    private const float _MAXTIMERSTAR3 = 1.5f;

    private float _timerStar1 = 0;
    private float _timerStar2 = 0;
    private float _timerStar3 = 0;

    private bool _timerstar1Active = false;
    private bool _timerstar2Active = false;
    private bool _timerstar3Active = false;

    // Use this for initialization
    void Awake () {
        _itemCounter = FindObjectOfType<ItemCounter>();
        _coinsToFind = CountCollectableCoins();
	}
	
    void Start()
    {
        if (FindObjectOfType<Ziel>() == null)
            gameObject.SetActive(false);
    }
    private int CountCollectableCoins()
    {
        int coins = 0;
        Collectable[] collectables = FindObjectsOfType<Collectable>();
        foreach(Collectable collectable in collectables)
        {
            if (collectable.ItemType == "coin")
            {
                coins++;
            }
        }

        ItemDropper[] droppers = FindObjectsOfType<ItemDropper>();
        foreach(ItemDropper dropper in droppers)
        {
            if (dropper.HoldItem != null)
            {
                Collectable collectable = dropper.HoldItem.GetComponent<Collectable>();
                if (collectable != null)
                {
                    if (collectable.ItemType == "coin")
                    {
                        coins++;
                    }
                }
            }
        }
        return coins;
    }


    void FixedUpdate()
    {
        if (_timerstar1Active)
        {
            _timerStar1 -= Time.deltaTime;
            if (_timerStar1 <= 0)
            {
                Star1Anim.SetTrigger("place");
                _timerstar1Active = false;
                Star1Sound.Play();
            }
        }

        if (_timerstar2Active)
        {
            _timerStar2 -= Time.deltaTime;
            if (_timerStar2 <= 0)
            {
                Star2Anim.SetTrigger("place");
                _timerstar2Active = false;
                Star2Sound.Play();
            }
        }

        if (_timerstar3Active)
        {
            _timerStar3 -= Time.deltaTime;
            if (_timerStar3 <= 0)
            {
                Star3Anim.SetTrigger("place");
                _timerstar3Active = false;
                Star3Sound.Play();
            }
        }
    }

	public void ShowEndScreen()
    {
        Cursor.visible = true;
        gameObject.SetActive(true);
        EndScreenBGAnim.SetTrigger("show");

        coinsCollectedText.text = _itemCounter.CoinsCollected + " / " + _coinsToFind;

        float coinsFoundRatio = (float)_itemCounter.CoinsCollected / _coinsToFind;

        _timerstar1Active = true;
        _timerStar1 = _MAXTIMERSTAR1;//Ersten Stern immer bei Levelabschluss geben.

        if (coinsFoundRatio >= 0.5f)
        {
            _timerstar2Active = true;
            _timerStar2 = _MAXTIMERSTAR2;//2. Stern bei mindestens 50% geben.
        }

        if (coinsFoundRatio >= 0.8f)
        {
            _timerstar3Active = true;
            _timerStar3 = _MAXTIMERSTAR3;//3. Stern bei mindestens 80% geben
        }

        //Analytics
        Dictionary<string, object> args = new Dictionary<string, object>();
        args.Add("level", SceneManager.GetActiveScene().name);
        args.Add("coins_collected", _itemCounter.CoinsCollected);
        args.Add("coins_collectable", _coinsToFind);
        Analytics.CustomEvent("LevelComplete", args);
    }
}
