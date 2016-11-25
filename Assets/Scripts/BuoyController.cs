using UnityEngine;
using System.Collections;

public class BuoyController : MonoBehaviour {

    public  GameObject buoyPrefab;  // prefab de las boyas.
    public float radius;            // radio de generacion de boyas.
    public float angle = 30;        // angulo de separacion entre cada voya.

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
            Instantiate(buoyPrefab, p, Quaternion.identity, _t);
        }

	}

    Vector3 LengthDir(float dir, float mag)
    {
        float xx = mag * (Mathf.Cos((dir * Mathf.PI) / 180));
        float yy = mag * (Mathf.Sin((dir * Mathf.PI) / 180));

        return new Vector3(xx, 0f, yy);
    }
    
}
