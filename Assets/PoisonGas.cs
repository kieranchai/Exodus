using System.Collections;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public static PoisonGas instance { get; private set; }

    [SerializeField]
    private Transform targetLocation;

    private Vector3 targetLocationV3;

    private float timeTaken = 10f;

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
}
