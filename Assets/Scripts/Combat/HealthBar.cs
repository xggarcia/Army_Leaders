using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public BaseHealthController baseHealthController;
    public string team;

    void Update()
    {
        if (baseHealthController != null)
        {
            float percent = baseHealthController.GetHealthPercent(team);
            fillImage.fillAmount = percent;
        }
    }
}
