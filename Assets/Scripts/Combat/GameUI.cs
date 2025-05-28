using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI redTeamScoreText;
    [SerializeField] private TextMeshProUGUI blueTeamScoreText;
    [SerializeField] private TextMeshProUGUI redTeamStatsText;
    [SerializeField] private TextMeshProUGUI blueTeamStatsText;

    [Header("Game Status")]
    [SerializeField] private TextMeshProUGUI gameStatusText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;

    [Header("Base Health")]
    [SerializeField] private TextMeshProUGUI redBaseHealthText;
    [SerializeField] private TextMeshProUGUI blueBaseHealthText;

    [Header("References")]
    [SerializeField] private Combat combatScript;
    [SerializeField] private BaseHealthController baseHealthController;

    private bool gameEnded = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        UpdateScoreDisplay();
        UpdateBaseHealthDisplay();
        CheckGameStatus();
    }

    private void UpdateScoreDisplay()
    {
        if (combatScript == null) return;

        // Update scores
        float redScore = combatScript.redTeamStats.GetScore();
        float blueScore = combatScript.blueTeamStats.GetScore();

        if (redTeamScoreText != null)
            redTeamScoreText.text = $"Equipo Rojo: {redScore:F1}";
        
        if (blueTeamScoreText != null)
            blueTeamScoreText.text = $"Equipo Azul: {blueScore:F1}";

        // Update detailed stats
        if (redTeamStatsText != null)
        {
            var redStats = combatScript.redTeamStats;
            redTeamStatsText.text = $"Poder: {redStats.power} | Defensa: {redStats.defense} | Velocidad: {redStats.speed}";
        }

        if (blueTeamStatsText != null)
        {
            var blueStats = combatScript.blueTeamStats;
            blueTeamStatsText.text = $"Poder: {blueStats.power} | Defensa: {blueStats.defense} | Velocidad: {blueStats.speed}";
        }
    }

    private void UpdateBaseHealthDisplay()
    {
        if (baseHealthController == null) return;

        if (redBaseHealthText != null)
            redBaseHealthText.text = $"Base Roja: {baseHealthController.RedBaseHealth} HP";
        
        if (blueBaseHealthText != null)
            blueBaseHealthText.text = $"Base Azul: {baseHealthController.BlueBaseHealth} HP";
    }

    private void CheckGameStatus()
    {
        if (gameEnded) return;

        // Check for base destruction
        if (baseHealthController != null)
        {
            if (baseHealthController.RedBaseHealth <= 0)
            {
                ShowGameOver("¡EQUIPO AZUL GANA!", "El equipo azul ha destruido la base roja");
                gameEnded = true;
            }
            else if (baseHealthController.BlueBaseHealth <= 0)
            {
                ShowGameOver("¡EQUIPO ROJO GANA!", "El equipo rojo ha destruido la base azul");
                gameEnded = true;
            }
        }

        // Update status during gameplay
        if (gameStatusText != null && !gameEnded)
        {
            if (combatScript != null)
            {
                float redScore = combatScript.redTeamStats.GetScore();
                float blueScore = combatScript.blueTeamStats.GetScore();
                
                if (redScore > blueScore)
                    gameStatusText.text = "El equipo rojo está dominando";
                else if (blueScore > redScore)
                    gameStatusText.text = "El equipo azul está dominando";
                else
                    gameStatusText.text = "¡Empate!";
            }
        }
    }

    private void ShowGameOver(string winner, string reason)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        if (winnerText != null)
            winnerText.text = $"{winner}\n\n{reason}";
        
        if (gameStatusText != null)
            gameStatusText.text = "JUEGO TERMINADO";
    }

    // Método público para reiniciar el juego
    public void RestartGame()
    {
        gameEnded = false;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        Time.timeScale = 1f; // Asegurar que el tiempo esté corriendo
    }
} 