using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
  
    [SerializeField]
    private Image experiencebar;

    private float lerpSpeed;
    void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        HealthBarFiller();
    }

    private void HealthBarFiller()
    {
        experiencebar.fillAmount = Mathf.Lerp(experiencebar.fillAmount, PlayerScript.instance.experience / LevelController.instance.xpNeeded, lerpSpeed);
    }

}
