using UnityEngine;
using System.Collections;

/// <summary>
/// Менеджер аудио для управления фоновой музыкой в игре.
/// </summary>
public class AudioManager : MonoBehaviour
{
  #region Поля и свойства
  /// <summary>
  /// Статический экземпляр менеджера аудио для глобального доступа.
  /// </summary>
  public static AudioManager Instance { get; private set; }

  /// <summary>
  /// Компонент AudioSource для воспроизведения музыки.
  /// </summary>
  [SerializeField] private AudioSource audioSource;

  #endregion

  #region Методы

  /// <summary>
  /// Воспроизводит указанную музыку.
  /// </summary>
  public void PlayMusic(AudioClip clip)
  {
    if (clip == null) return;

    audioSource.clip = clip;
    audioSource.Play();
  }

  /// <summary>
  /// Плавно меняет музыку на указанную.
  /// </summary>
  public void ChangeMusic(AudioClip newClip, float fadeDuration = 1.0f)
  {
    StartCoroutine(CrossFadeMusic(newClip, fadeDuration));
  }

  /// <summary>
  /// Останавливает музыку.
  /// </summary>
  public void StopMusic()
  {
    audioSource.Stop();
  }

  /// <summary>
  /// Устанавливает громкость музыки.
  /// </summary>
  public void SetMusicVolume(float volume)
  {
    audioSource.volume = volume;
  }

  /// <summary>
  /// Корутина для плавной смены музыки.
  /// </summary>
  private IEnumerator CrossFadeMusic(AudioClip newClip, float fadeDuration)
  {
    float startVolume = audioSource.volume;
    for (float t = 0; t < fadeDuration; t += Time.deltaTime)
    {
      audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
      yield return null;
    }

    audioSource.Stop();
    audioSource.volume = startVolume;

    PlayMusic(newClip);
  }

  #endregion

  #region Базовый класс

  // <summary>
  /// Инициализация менеджера аудио.
  /// </summary>
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      audioSource = GetComponent<AudioSource>();
      if (audioSource == null)
      {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
      }
    }
    else
    {
      Destroy(gameObject);
      return;
    }
  }

  #endregion
}