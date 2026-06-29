using JudgmentOfTheFallenWing.Core.Events;
using JudgmentOfTheFallenWing.Core.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JudgmentOfTheFallenWing.Core.Managers
{
    /// <summary>
    /// Singletonsor principal del juego. Persiste entre escenas.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private bool pauseOnStart;

        public bool IsPaused { get; private set; }
        public SaveData CurrentSave { get; private set; } = new SaveData();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (pauseOnStart)
                SetPaused(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetPaused(!IsPaused);
            }
        }

        public void SetPaused(bool paused)
        {
            if (IsPaused == paused) return;

            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;

            if (paused)
                GameEvents.RaiseGamePaused();
            else
                GameEvents.RaiseGameResumed();
        }

        public void LoadGame()
        {
            CurrentSave = SaveSystem.Load();
        }

        public void SaveGame()
        {
            SaveSystem.Save(CurrentSave);
            GameEvents.RaiseGameSaved();
        }

        public void NewGame()
        {
            CurrentSave = new SaveData();
            SaveSystem.Save(CurrentSave);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
