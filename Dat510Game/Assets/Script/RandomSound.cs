
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public AudioClip[] soundClips;
    public AudioSource audioSource;
    private float nextPlayTime;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ScheduleNextSound();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextPlayTime)
        {
            PlayRandomSound();
            ScheduleNextSound();
        }
    }

    void PlayRandomSound()
    {
        if (soundClips.Length > 0)
        {
            int randomIndex = Random.Range(0, soundClips.Length);
            Debug.Log("Playing sound: " + soundClips[randomIndex].name);
            audioSource.PlayOneShot(soundClips[randomIndex]);
        }
    }

    void ScheduleNextSound()
    {
        float randomInterval = Random.Range(50f, 100f);
        nextPlayTime = Time.time + randomInterval;
    }
}
