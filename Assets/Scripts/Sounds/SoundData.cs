using UnityEngine;



[CreateAssetMenu(fileName = "NewSound",  menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    public AudioClip[] clips;

    [Range(0f, 1f)] public float volume = 1f;
    [Range (0f, 1f)] public float pitch = 1f;

    public bool useRandomPitch = true;
    [Range(0.1f, 0.5f)] public float pitchVariance = 0.1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
        {
            return null;
        }
        return clips[Random.Range(0, clips.Length)];
    }
}
