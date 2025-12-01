using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips de Audio")]
    public AudioClip backgroundMusic;
    public AudioClip clickSound;
    public AudioClip shootSound;
    public AudioClip correctTrashSound; // Sonido al embocar bien
    public AudioClip wrongTrashSound;   // Opcional: si se equivoca
    public AudioClip scoreSound;
    //public AudioClip ;

    private void Awake()
    {
        // Esto asegura que solo haya un AudioManager y no se destruya al cambiar de escena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Iniciar música de fondo automáticamente
        PlayMusic(backgroundMusic);
    }

    // Método para reproducir música (bucle)
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return; // Si ya suena esa música, no reiniciarla

        musicSource.clip = clip;
        musicSource.Play();
    }

    // Método para reproducir efectos
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip); // PlayOneShot permite solapar sonidos
        }
    }
}