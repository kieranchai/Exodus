using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; set; }
    public Transform playerTarget;
    public Transform[] zonesTarget;

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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start() {
        initialZoom = Camera.main.orthographicSize;
        animator = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
        if (GameController.instance.currentState == GameController.GAME_STATE.PANNING) StartCoroutine(StartGame());
        if (GameController.instance.currentState == GameController.GAME_STATE.TUTORIAL)
        {
            target = playerTarget;
            smoothTime = playerSmoothTime;
        }
    }

    private void Update()
    {
        if(GameController.instance.currentState == GameController.GAME_STATE.PANNING)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) SkipPanning();
        }
    }

    private void FixedUpdate()
    {
        if(target)
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
/*            tutorialDialogueBox.SetActive(false);
            yield return new WaitForSeconds(1f);
            tutorialDialogueBox.SetActive(true);
            yield return StartCoroutine(Typewriter("This is the MOTHERSHIP!"));*/
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
        GameController.instance.currentState = GameController.GAME_STATE.PLAYING;
        PlayerScript.instance.playerPanel.gameObject.SetActive(true);
        PoisonGas.instance.StartGas();
        PlayerScript.instance.StartCoroutine(PlayerScript.instance.SpawnFlicker());
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

    IEnumerator Typewriter(string text)
    {
/*        tutorialDialogueText.text = "";*/
        var waitTimer = new WaitForSeconds(.05f);
        foreach (char c in text)
        {
/*            tutorialDialogueText.text += c;*/
            yield return waitTimer;
        }
        yield return new WaitForSeconds(0.5f);
    }
}
