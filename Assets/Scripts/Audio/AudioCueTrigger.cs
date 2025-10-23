using UnityEngine;

public class AudioCueTrigger : MonoBehaviour
{
    [SerializeField] private AudioCueId cueId;

    public void TriggerCue()
    {
        SoundscapeManager.Instance.PlayVoiceCue(cueId);
    }
}