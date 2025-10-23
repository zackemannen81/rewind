using UnityEngine;

public class DroneAudioEmitter : MonoBehaviour
{
    private bool isAlerted;

    private void Update()
    {
        // TODO: Implement drone audio logic based on isAlerted state
    }

    public void SetAlertState(bool isAlerted)
    {
        this.isAlerted = isAlerted;
    }
}