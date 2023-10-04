using UnityEngine;

public class MagnetScript : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<OrbScript>(out OrbScript Orb))
        {
            if (collision.gameObject.CompareTag("HpOrb") && PlayerScript.instance.currentHealth >= PlayerScript.instance.maxHealth)
            {
                Physics2D.IgnoreCollision(PlayerScript.instance.coll, collision);
                Orb.UnsetTarget();
                return;
            }

            Physics2D.IgnoreCollision(PlayerScript.instance.coll, collision, false);
            Orb.SetTarget(transform.parent.position);
        }
    }

}
