using UnityEngine;
using UnityEngine.UI;

public class GasBar : MonoBehaviour
{
    [SerializeField]
    private Image gasBar;

    private float lerpSpeed;
    void Update()
    {
        lerpSpeed = 10f * Time.deltaTime;
        GasBarFill();
    }

    private void GasBarFill()
    {
        gasBar.fillAmount = Mathf.Lerp(gasBar.fillAmount, PoisonGas.instance.t / 1, lerpSpeed);
    }

}
