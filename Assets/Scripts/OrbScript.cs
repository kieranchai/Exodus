using System.Collections;
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    public Rigidbody2D rb;
    bool hasTarget;
    public int value;
    Vector3 targetPosition;
    public float moveSpeed;
    public float timer;
    public bool isFlashing = false;
    public SpriteRenderer sr;


    private void Start()
    {
        timer += Random.Range(-2, 2);
    }
    void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 1)
            {
                if (!isFlashing)
                {
                    isFlashing = true;
                    StartCoroutine(FlashOrb());
                }
            }
        }
        else
        {
            Destroy(this.gameObject);
        }

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

    public void UnsetTarget()
    {
        hasTarget = false;
        targetPosition = transform.position;
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

    IEnumerator FlashOrb()
    {
       while (isFlashing)
        {
            Color color = sr.color;
            color.a = 0f;
            sr.color = color;
            yield return new WaitForSeconds(0.1f);
            color.a = 1f;
            sr.color = color;
            yield return new WaitForSeconds(0.1f);
        }
    }


}
