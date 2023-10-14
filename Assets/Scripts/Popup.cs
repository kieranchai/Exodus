using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private float destroyTimer = 0.2f;

    private Color textColor;
    private TextMeshPro textMesh;

    private Vector3 RandomizeIntensity = new Vector3(0.3f, 0.1f, 0);

    private bool isHeal = false;

    private void Awake()
    {
        textMesh = gameObject.GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
            Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
            Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z));
    }

    public void SetDamageNumber(float damage)
    {
        textMesh.text = damage.ToString();
        textColor = textMesh.color;
        destroyTimer = 0.2f;
    }

    public void SetCrit(float damage)
    {
        gameObject.GetComponent<Animator>().SetBool("isCrit", true);
        textColor = new Color32(243, 189, 19, 255);
        textMesh.color = textColor;
        textMesh.text = damage.ToString();
        destroyTimer = 0.2f;
    }

    public void SetPlayerDamage(float damage, bool evaded = false, bool blocked = false)
    {
        textColor = new Color32(255, 37, 37, 255);
        textMesh.color = textColor;
        if (evaded)
        {
            textMesh.text = "EVADED";
        }
        else if (blocked)
        {
            textMesh.text = "BLOCKED";
        }
        else
        {
            textMesh.text = damage.ToString();
        }
        destroyTimer = 0.2f;
    }

    public void SetHeal(float heal)
    {
        isHeal = true;
        textColor = new Color32(150, 255, 126, 255);
        textMesh.color = textColor;
        textMesh.text = $"+{heal}";
        destroyTimer = 0.2f;
    }

    private void LateUpdate()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer < 0)
        {
            if (isHeal)
            {
                float moveYSpeed = 1f;
                transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
            }
            float destroySpeed = 2.5f;
            textColor.a -= destroySpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
