using UnityEngine;
using System.Collections;

public class AmbientAudioController : MonoBehaviour {

    public AudioClip[] birds;
    AudioSource aSource;

    [Range(5, 15)]
    public int bRange;

    private IEnumerator coroutine;



	// Use this for initialization
    void Start () {
        aSource = GetComponent<AudioSource>();

        //aSource.PlayScheduled(3);
        coroutine = PlayBirdsAudio(4.0f);
        StartCoroutine(coroutine);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator PlayBirdsAudio(float waitTime) {
        while (true) {
            int i = Mathf.RoundToInt(Random.Range(0, birds.Length));
            float j = Mathf.RoundToInt(Random.Range(5, bRange));
            yield return new WaitForSeconds(j);

            aSource.clip = birds[i];
            aSource.Play();
        }
    }
}
