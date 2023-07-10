using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton instance

    public AudioSource soundSource; // Reference to the audio source component

    // Sound clips
    public AudioClip mineExplosionSound;
    public AudioClip numberRevealSound;
    public AudioClip toggleFlagSound;
    public AudioClip removeFlagSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Get the audio source component on this object
        soundSource = GetComponent<AudioSource>();
    }

    // Method to play a sound clip by name
    public void PlaySound(string soundName)
    {
        Debug.Log("Attempting to play sound: " + soundName); // Add this line
        AudioClip soundClip = null;

        // Assign the appropriate sound clip based on the name
        switch (soundName)
        {
            case "MineExplosion":
                soundClip = mineExplosionSound;
                break;
            case "NumberReveal":
                soundClip = numberRevealSound;
                break;
            case "ToggleFlag":
                soundClip = toggleFlagSound;
                break;
            case "RemoveFlag":
                soundClip = removeFlagSound;
                break;
        }

        if (soundClip != null)
        {
            soundSource.PlayOneShot(soundClip);
        }
        else
        {
            Debug.LogWarning("Sound clip not found: " + soundName);
        }
    }
}
