using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MagnetScript : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D Collider) {
        if (Collider.gameObject.TryGetComponent<OrbScript>(out OrbScript Orb)) {
            Orb.SetTarget(transform.parent.position);
        }
    }
    
}
