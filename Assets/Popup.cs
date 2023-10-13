using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private float destroyTimer = 1f;

    private Color textColor;
    private TextMeshPro textMesh;

    private Vector3 RandomizeIntensity = new Vector3(0.3f, 0.1f, 0);

    private void Start()
    {
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
            Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
            Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z));
    }

    public void SetDamageNumber(float damage)
    {
        textMesh = gameObject.GetComponent<TextMeshPro>();
        textMesh.text = damage.ToString();
        textColor = textMesh.color;
    }

    public void SetCrit(float damage)
    {
        gameObject.GetComponent<Animator>().SetBool("isCrit", true);
        textMesh = gameObject.GetComponent<TextMeshPro>();
        textMesh.text = damage.ToString();

    }

    private void Update()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer < 0)
        {
            float destroySpeed = 3f;
            textColor.a -= destroySpeed * Time.deltaTime;
            gameObject.transform.localScale /= 2f;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
