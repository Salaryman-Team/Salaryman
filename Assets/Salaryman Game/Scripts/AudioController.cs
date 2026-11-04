using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource testAudioSource;
    [SerializeField] private bool audioPlaying;
    void Start()
    {
        
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Test Level" && audioPlaying == false)
        {
            testAudioSource.Play();
            audioPlaying = true;
        }
    }
}
