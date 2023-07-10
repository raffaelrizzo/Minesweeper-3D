using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gameOverText;
    
    [SerializeField]
    private TextMeshProUGUI victoryText;

    [SerializeField]
    private Button playAgainButton;
    
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;

        playAgainButton.onClick.AddListener(PlayAgain);

        gameOverText.gameObject.SetActive(false);
        victoryText.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        gameOverText.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(true);
    }

    public void ShowVictoryUI() 
    {
        victoryText.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(true);
    }

    void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
