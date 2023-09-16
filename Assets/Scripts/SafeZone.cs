using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private float savedTimer = 0;

    [SerializeField]
    private float healInterval = 1f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if ((Time.time - savedTimer) > healInterval)
            {
                savedTimer = Time.time;
                collision.GetComponent<PlayerScript>().UpdateHealth(1f);
            }
        }
    }

}
