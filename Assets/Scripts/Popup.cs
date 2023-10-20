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

    public void SetExplosion(float damage)
    {
        textColor = new Color32(255, 138, 0, 255);
        textMesh.color = textColor;
        textMesh.text = damage.ToString();
        destroyTimer = 0.2f;
    }

    public void SetLightning(float damage)
    {
        textColor = new Color32(255, 52, 249, 255);
        textMesh.color = textColor;
        textMesh.text = damage.ToString();
        destroyTimer = 0.2f;
    }

    public void SetBleed(float damage)
    {
        gameObject.GetComponent<Animator>().SetBool("isBleed", true);
        textColor = new Color32(100, 149, 237, 255);
        textMesh.color = textColor;
        textMesh.text = damage.ToString();
        destroyTimer = 0.2f;
    }

    public void SetCrit(float damage)
    {
        gameObject.GetComponent<Animator>().SetBool("isCrit", true);
        textColor = new Color32(254, 148, 79, 255);
        textMesh.color = textColor;
        textMesh.text = damage.ToString();
        destroyTimer = 0.2f;
    }

    public void SetPlayerDamage(float damage, bool evaded = false, bool blocked = false)
    {
        textColor = new Color32(255, 37, 37, 255);
        textMesh.color = textColor;
        if (blocked)
        {
            textMesh.text = "BLOCKED";
        }
        else if (evaded)
        {
            textMesh.text = "EVADED";
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
        textMesh.text = $"+{Mathf.Round(heal * 10.0f) * 0.1f}";
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
