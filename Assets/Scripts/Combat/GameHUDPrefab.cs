using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUDPrefab : MonoBehaviour
{
    [ContextMenu("Create Game HUD")]
    public void CreateGameHUD()
    {
        // Buscar o crear Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Game Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Crear panel principal del HUD
        GameObject hudPanel = CreateUIPanel(canvas.transform, "Game HUD Panel");
        RectTransform hudRect = hudPanel.GetComponent<RectTransform>();
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.offsetMin = Vector2.zero;
        hudRect.offsetMax = Vector2.zero;

        // Área de scores (parte superior)
        GameObject scoresPanel = CreateUIPanel(hudPanel.transform, "Scores Panel");
        RectTransform scoresRect = scoresPanel.GetComponent<RectTransform>();
        scoresRect.anchorMin = new Vector2(0, 0.8f);
        scoresRect.anchorMax = new Vector2(1, 1);
        scoresRect.offsetMin = new Vector2(20, -20);
        scoresRect.offsetMax = new Vector2(-20, -20);

        // Score del equipo rojo (izquierda)
        CreateScoreText(scoresPanel.transform, "Red Team Score", "Equipo Rojo: 0.0", Color.red, new Vector2(0, 0.5f), new Vector2(0.5f, 1));

        // Score del equipo azul (derecha)
        CreateScoreText(scoresPanel.transform, "Blue Team Score", "Equipo Azul: 0.0", Color.blue, new Vector2(0.5f, 0.5f), new Vector2(1, 1));

        // Stats detalladas (parte superior-media)
        GameObject statsPanel = CreateUIPanel(hudPanel.transform, "Stats Panel");
        RectTransform statsRect = statsPanel.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0, 0.65f);
        statsRect.anchorMax = new Vector2(1, 0.8f);
        statsRect.offsetMin = new Vector2(20, 0);
        statsRect.offsetMax = new Vector2(-20, 0);

        // Stats del equipo rojo
        CreateStatsText(statsPanel.transform, "Red Team Stats", "Poder: 0 | Defensa: 0 | Velocidad: 0", Color.red, new Vector2(0, 0), new Vector2(0.5f, 1));

        // Stats del equipo azul
        CreateStatsText(statsPanel.transform, "Blue Team Stats", "Poder: 0 | Defensa: 0 | Velocidad: 0", Color.blue, new Vector2(0.5f, 0), new Vector2(1, 1));

        // Estado del juego (centro)
        CreateStatusText(hudPanel.transform, "Game Status", "¡Empieza el juego!", Color.white, new Vector2(0.2f, 0.45f), new Vector2(0.8f, 0.55f));

        // Salud de las bases (parte inferior)
        GameObject healthPanel = CreateUIPanel(hudPanel.transform, "Health Panel");
        RectTransform healthRect = healthPanel.GetComponent<RectTransform>();
        healthRect.anchorMin = new Vector2(0, 0.1f);
        healthRect.anchorMax = new Vector2(1, 0.25f);
        healthRect.offsetMin = new Vector2(20, 0);
        healthRect.offsetMax = new Vector2(-20, 0);

        // Salud base roja
        CreateHealthText(healthPanel.transform, "Red Base Health", "Base Roja: 100 HP", Color.red, new Vector2(0, 0), new Vector2(0.5f, 1));

        // Salud base azul
        CreateHealthText(healthPanel.transform, "Blue Base Health", "Base Azul: 100 HP", Color.blue, new Vector2(0.5f, 0), new Vector2(1, 1));

        // Panel de Game Over (inicialmente oculto)
        CreateGameOverPanel(hudPanel.transform);

        Debug.Log("Game HUD creado exitosamente!");
    }

    private GameObject CreateUIPanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        panel.AddComponent<RectTransform>();
        
        // Panel transparente
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0);
        
        return panel;
    }

    private void CreateScoreText(Transform parent, string name, string text, Color color, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        RectTransform rect = textGO.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI text_component = textGO.AddComponent<TextMeshProUGUI>();
        text_component.text = text;
        text_component.fontSize = 24;
        text_component.color = color;
        text_component.fontStyle = FontStyles.Bold;
        text_component.alignment = TextAlignmentOptions.Center;
    }

    private void CreateStatsText(Transform parent, string name, string text, Color color, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        RectTransform rect = textGO.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI text_component = textGO.AddComponent<TextMeshProUGUI>();
        text_component.text = text;
        text_component.fontSize = 16;
        text_component.color = color;
        text_component.alignment = TextAlignmentOptions.Center;
    }

    private void CreateStatusText(Transform parent, string name, string text, Color color, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        RectTransform rect = textGO.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI text_component = textGO.AddComponent<TextMeshProUGUI>();
        text_component.text = text;
        text_component.fontSize = 20;
        text_component.color = color;
        text_component.fontStyle = FontStyles.Bold;
        text_component.alignment = TextAlignmentOptions.Center;
    }

    private void CreateHealthText(Transform parent, string name, string text, Color color, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        RectTransform rect = textGO.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI text_component = textGO.AddComponent<TextMeshProUGUI>();
        text_component.text = text;
        text_component.fontSize = 18;
        text_component.color = color;
        text_component.fontStyle = FontStyles.Bold;
        text_component.alignment = TextAlignmentOptions.Center;
    }

    private void CreateGameOverPanel(Transform parent)
    {
        GameObject panel = new GameObject("Game Over Panel");
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Fondo semi-transparente
        Image bgImage = panel.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);

        // Texto de ganador
        GameObject winnerTextGO = new GameObject("Winner Text");
        winnerTextGO.transform.SetParent(panel.transform, false);
        
        RectTransform winnerRect = winnerTextGO.AddComponent<RectTransform>();
        winnerRect.anchorMin = new Vector2(0.1f, 0.3f);
        winnerRect.anchorMax = new Vector2(0.9f, 0.7f);
        winnerRect.offsetMin = Vector2.zero;
        winnerRect.offsetMax = Vector2.zero;

        TextMeshProUGUI winnerText = winnerTextGO.AddComponent<TextMeshProUGUI>();
        winnerText.text = "¡JUEGO TERMINADO!";
        winnerText.fontSize = 36;
        winnerText.color = Color.white;
        winnerText.fontStyle = FontStyles.Bold;
        winnerText.alignment = TextAlignmentOptions.Center;

        // Inicialmente oculto
        panel.SetActive(false);
    }
} 