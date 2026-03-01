using UnityEngine;

public class NarratorManager : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] lines; // Put your voice files here in order

    public void PlayLine(int index)
    {
        if(!source.isPlaying)
        {
            source.clip = lines[index];
            source.Play();
        }
    }
}
