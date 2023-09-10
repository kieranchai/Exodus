using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummyScript : MonoBehaviour
{
    [SerializeField]
    private Transform damagePopupPrefab;

    public void TakeDamage(float damage)
    {
        Transform damagePopupTransform = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);
    }

}
