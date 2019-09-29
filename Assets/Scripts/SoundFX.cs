using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public AudioClip explosionSound;
    private static AudioClip explosionSFX;
    private static AudioSource SFXPlayer;
    private static AudioSource revolveSound;
    private void Awake() {
        SFXPlayer = GetComponents<AudioSource>()[0];
        revolveSound = GetComponents<AudioSource>()[1];

        explosionSFX = explosionSound;

        PlayRevolveSound();

        DampenRevolveSound();
    }

    public static void PlayExplosionSound()
    {
        SFXPlayer.PlayOneShot(explosionSFX);
    }

    public static void PlaySound(AudioClip sound)
    {
        SFXPlayer.PlayOneShot(sound);
    }

    public static void PlayRevolveSound()
    {
        revolveSound.Play();
    }

    public static void StopRevolveSound()
    {
        revolveSound.Stop();
    }

    public static void DampenRevolveSound(){
        revolveSound.spatialBlend = 0.5f;
    }

    public static void ReviveRevolveSound(){
        revolveSound.spatialBlend = 0.0f;
    }
}
