using TMPro;
using UnityEngine;

public class AnnouncementScript : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textMesh;

    private float timer = 0;
    private float duration = 0.8f;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetText(string text)
    {
        this.textMesh.text = text;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            anim.SetBool("Disappear", true);
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}

