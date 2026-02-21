using UnityEngine;

public class MicInputManager : MonoBehaviour
{
    public string device;
    private AudioClip _clipRecord;
    private int _sampleWindow = 128;

    void Start()
    {
        // ??????????????????????????????????
        if (device == null && Microphone.devices.Length > 0)
        {
            device = Microphone.devices[0];
        }
        _clipRecord = Microphone.Start(device, true, 999, 44100);
    }

    public float GetLoudnessFromMic()
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(device), _clipRecord);
    }

    float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - _sampleWindow;
        if (startPosition < 0) return 0;

        float[] waveData = new float[_sampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0;
        for (int i = 0; i < _sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }
        return totalLoudness / _sampleWindow;
    }
}