using TMPro;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    public Equipment loot;

    private GameObject textBox;

    private void Awake()
    {
        textBox = transform.Find("Text Box").gameObject;
    }

    public void Initialise(Equipment equipment)
    {
        loot = equipment;
        textBox.transform.Find("Loot Name").GetComponent<TMP_Text>().text = equipment.equipmentName;
        textBox.transform.Find("Thumbnail").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"UISprites/{equipment.thumbnailPath}");
        string desc = equipment.description.Replace("[value]", equipment.value);
        string desc2 = desc.Replace("[secValue]", equipment.secValue);
        textBox.transform.Find("Loot Desc").GetComponent<TMP_Text>().text = desc2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textBox.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerScript.instance.EquipEquipment(loot);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textBox.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerScript.instance.EquipEquipment(loot);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textBox.gameObject.SetActive(false);
        }
    }

    public void EnableTextBox()
    {
        textBox.gameObject.SetActive(true);
    }

    public void DisableTextBox()
    {
        textBox.gameObject.SetActive(false);
    }
}
