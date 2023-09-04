using System.Collections;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public static PoisonGas instance { get; private set; }

    [SerializeField]
    private Transform targetLocation;

    private Vector3 targetLocationV3;

    [SerializeField]
    private float timeTaken = 30f;

    private float savedTimer = 0;

    [SerializeField]
    private float damageInterval = 2f;

    [SerializeField]
    private float damage = 5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        this.targetLocationV3 = new Vector3(targetLocation.position.x, targetLocation.position.y + (gameObject.GetComponent<Collider2D>().bounds.size.y / 2));
        StartCoroutine(SpreadToPosition(transform, this.targetLocationV3, this.timeTaken));
    }

    public IEnumerator SpreadToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if ((Time.time - savedTimer) > damageInterval)
            {
                savedTimer = Time.time;
                collision.GetComponent<PlayerScript>().TakeDamage(damage, true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Safe Zone"))
        {
            // LOSE
        }
    }
}
