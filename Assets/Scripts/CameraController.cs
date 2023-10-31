using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; set; }
    public Transform playerTarget;
    public Transform[] zonesTarget;
    public Transform tutorialPan;

    Vector3 velocity = Vector3.zero;

    private float smoothTime;

    public Vector3 positionOffset;

    public Vector2 xLimit;
    public Vector2 yLimit;
    public Animator animator;

    private float playerSmoothTime = 0.1f;
    private float introSmoothTime = 1f;

    private Transform target;

    private float initialZoom;

    private bool hasStarted = false;

    private void Awake()
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
        initialZoom = Camera.main.orthographicSize;
        animator = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
        if (GameController.instance.currentState == GameController.GAME_STATE.PANNING) StartCoroutine(StartGame());
    }

    private void Update()
    {
        if (GameController.instance.currentState == GameController.GAME_STATE.PANNING)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !hasStarted) SkipPanning();
        }
    }

    private void FixedUpdate()
    {
        if (target)
        {
            Vector3 targetPosition = target.position + positionOffset;
            targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, xLimit.x, xLimit.y), Mathf.Clamp(targetPosition.y, yLimit.x, yLimit.y), -10);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    IEnumerator StartGame()
    {
        smoothTime = introSmoothTime;
        Camera.main.orthographicSize = 9;
        PlayerScript.instance.playerPanel.SetActive(false);
        for (int i = 0; i < zonesTarget.Length; i++)
        {
            target = zonesTarget[i];
            yield return new WaitForSeconds(2.5f);
        }

        target = playerTarget;
        yield return new WaitForSeconds(2.5f);
        yield return ZoomInCamera();
        smoothTime = playerSmoothTime;
        StartRealGame();
    }

    private void SkipPanning()
    {
        StopAllCoroutines();
        smoothTime = playerSmoothTime;
        target = playerTarget;
        Camera.main.orthographicSize = initialZoom;
        StartRealGame();
    }

    private void StartRealGame()
    {
        PlayerScript.instance.rb.bodyType = RigidbodyType2D.Dynamic;
        GameController.instance.currentState = GameController.GAME_STATE.PLAYING;
        if (!hasStarted)
        {
            PlayerScript.instance.playerPanel.gameObject.SetActive(true);
            PlayerScript.instance.StartCoroutine(PlayerScript.instance.SpawnFlicker());
            GameController.instance.StartTutorial();
        }
        hasStarted = true;
    }

    public IEnumerator PanToBoss()
    {
        PoisonGas.instance.StopAllCoroutines();
        GameController.instance.currentState = GameController.GAME_STATE.PANNING;
        PlayerScript.instance.rb.bodyType = RigidbodyType2D.Static;
        smoothTime = introSmoothTime;
        target = zonesTarget[0];
        yield return new WaitForSeconds(2.5f);
        target = playerTarget;
        yield return new WaitForSeconds(0.5f);
        smoothTime = playerSmoothTime;
        StartRealGame();
    }

    public IEnumerator PanToCrabs()
    {
        PoisonGas.instance.StopAllCoroutines();
        GameController.instance.currentState = GameController.GAME_STATE.PANNING;
        PlayerScript.instance.rb.bodyType = RigidbodyType2D.Static;
        smoothTime = introSmoothTime;
        target = tutorialPan;
        yield return new WaitForSeconds(2.5f);
        target = playerTarget;
        yield return new WaitForSeconds(0.5f);
        smoothTime = playerSmoothTime;
        PlayerScript.instance.rb.bodyType = RigidbodyType2D.Dynamic;
        GameController.instance.currentState = GameController.GAME_STATE.PLAYING;
    }

    public IEnumerator PanToBossDeath()
    {
        PlayerScript.instance.rb.bodyType = RigidbodyType2D.Static;
        smoothTime = introSmoothTime;
        target = zonesTarget[0];
        yield return null;
    }

    IEnumerator ZoomInCamera()
    {
        float elapsed = 0;
        float currentZoom = Camera.main.orthographicSize;
        while (elapsed <= 1)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 1);
            Camera.main.orthographicSize = Mathf.Lerp(currentZoom, initialZoom, t);
            yield return null;
        }
    }
}
