using UnityEngine;
using System.Collections;

public class BuoyController : MonoBehaviour {

    public  GameObject buoyPrefab;  // prefab de las boyas.
    public float radius;            // radio de generacion de boyas.
    public float angle = 30;        // angulo de separacion entre cada voya.
    public bool randRotation = false;

    private float _buoyCuantity;    // cantidad de boyas.
    private Transform _t;           // transform del landing spot.

	void Start () 
    {
        _t = GetComponent<Transform>();
        _buoyCuantity = Mathf.Round(360 / angle);

        for (int i = 0; i < _buoyCuantity; i++)
        {
            float j = angle * i;
            Vector3 p = _t.position + LengthDir(j, radius);

            Quaternion r;
            if (randRotation)
                r = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(-26, 26)));
            else
                r = Quaternion.identity;
            
			GameObject g = (GameObject)Instantiate(buoyPrefab, p, r, _t);
			TweenPosition tp = g.GetComponent<TweenPosition> ();
			tp.from = g.transform.localPosition - new Vector3 (0f, 0.1f, 0f);
			tp.to = g.transform.localPosition;
			tp.duration = Random.Range (0.8f, 1.2f);
			tp.delay = Random.Range (0f, 1f);
        }

	}

    Vector3 LengthDir(float dir, float mag)
    {
        float xx = mag * (Mathf.Cos((dir * Mathf.PI) / 180));
        float yy = mag * (Mathf.Sin((dir * Mathf.PI) / 180));

        return new Vector3(xx, 0f, yy);
    }
    
}
