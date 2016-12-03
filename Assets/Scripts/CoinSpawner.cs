using UnityEngine;
using System.Collections;

public class CoinSpawner : MonoBehaviour {

    public int coinsNumber;
    public GameObject coinsPrefab;
    Transform t;
    private int[] _coins; // 0 vacio, 1 ocupado
    public float separation;
    private int _slots;
    private int _currentCoins;

	void Start () {
        t = GetComponent<Transform>();

        Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init()
    {
        _currentCoins = 0;
        _slots = coinsNumber + Mathf.RoundToInt(coinsNumber / 2);
        _coins = new int[_slots];

        for (int i = 0; i < _slots; i++)
        {
            _coins[i] = 0;
        }
            
        while (_currentCoins != coinsNumber)
        {
            for (int i = 0; i < _slots; i++)
            {
                int j = Random.Range(0, 10);
                if (((j <= 6) && (_currentCoins != coinsNumber)) && (_coins[i] == 0))
                {
                    _coins[i] = 1;
                    Instantiate(coinsPrefab, new Vector3(this.t.position.x, i*separation, this.t.position.z), Quaternion.identity, this.t);
                    _currentCoins++;
                }
            }
        }


        Debug.Log("listo, monedas spawneadas");
    }
}
