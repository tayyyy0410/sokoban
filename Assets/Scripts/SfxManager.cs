using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager I { get; private set; }

    [Header("Clips")]
    public AudioClip buttonClickClip;
    public AudioClip pushClip;
    public AudioClip vanishClip;

    AudioSource audioSource;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    public static void PlayButton()
    {
        if (I && I.buttonClickClip)
            I.audioSource.PlayOneShot(I.buttonClickClip);
    }

    public static void PlayPush()
    {
        if (I && I.pushClip)
            I.audioSource.PlayOneShot(I.pushClip);
    }

    public static void PlayVanish()
    {
        if (I && I.vanishClip)
            I.audioSource.PlayOneShot(I.vanishClip);
    }
}