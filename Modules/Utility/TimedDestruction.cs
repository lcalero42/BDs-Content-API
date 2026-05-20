using UnityEngine;

namespace DbsContentApi;

/// <summary>
/// Disables its GameObject after a specified number of rendered frames.
/// </summary>
internal class TimedDestruction : MonoBehaviour
{
    public int frames = 180;

    private int _remaining;

    void Start()
    {
        _remaining = frames;
        if (_remaining <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    void Update()
    {
        _remaining--;
        if (_remaining <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
