
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    public Rigidbody2D rb;
    bool hasTarget;
    public int value;
    Vector3 targetPosition;
    public float moveSpeed;

    void FixedUpdate()
    {
        if (PlayerInSight())
        {
            if (hasTarget)
            {
                Vector2 targetdirection = (targetPosition - transform.position).normalized;
                rb.velocity = new Vector2(targetdirection.x, targetdirection.y) * moveSpeed;
            }
        }

    }
    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public bool PlayerInSight()
    {
        int mask1 = 1 << LayerMask.NameToLayer("Tilemap Collider");
        RaycastHit2D hit = Physics2D.Linecast(transform.position, PlayerScript.instance.transform.position, mask1);
        if (hit.collider != null)
        {
            return false;
        }
        return true;
    }
}
