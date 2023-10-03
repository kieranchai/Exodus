using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorScript : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CashOrb")) {
            PlayerScript.instance.UpdateCash(collision.gameObject.GetComponent<OrbScript>().value);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("HpOrb")) {
            PlayerScript.instance.UpdateHealth(collision.gameObject.GetComponent<OrbScript>().value);
            Destroy(collision.gameObject);
        }
    }
}
