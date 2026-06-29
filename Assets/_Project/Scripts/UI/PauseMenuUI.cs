using JudgmentOfTheFallenWing.Core.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JudgmentOfTheFallenWing.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        private void OnEnable()
        {
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        private void Update()
        {
            if (GameManager.Instance == null || pausePanel == null) return;
            pausePanel.SetActive(GameManager.Instance.IsPaused);
        }

        public void Resume()
        {
            GameManager.Instance?.SetPaused(false);
        }

        public void SaveGame()
        {
            GameManager.Instance?.SaveGame();
        }

        public void LoadMainMenu()
        {
            GameManager.Instance?.SetPaused(false);
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
            GameManager.Instance?.QuitGame();
        }
    }
}
