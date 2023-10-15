using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningScript : MonoBehaviour
{
    private CircleCollider2D coll;

    public LayerMask enemyLayer;
    public float damage;

    public GameObject chainLightningEffect;
    public GameObject beenStruck;

    public int amountToChain = 5;

    private GameObject startObject;
    public GameObject endObject;

    private Animator anim;
    public ParticleSystem particleSys;

    public int singleSpawns;

    private AudioSource SFXSource;
    [Header("Lightning Audio Clip")]
    public AudioClip lightning;

    private void Awake()
    {
        SFXSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (amountToChain == 0) Destroy(gameObject);

        SFXSource.PlayOneShot(lightning);
        coll = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        particleSys = GetComponent<ParticleSystem>();

        startObject = gameObject;
        singleSpawns = 1;
        Destroy(gameObject, 0.4f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)) && !collision.GetComponentInChildren<EnemyStruck>())
        {
            if (singleSpawns != 0)
            {
                endObject = collision.gameObject;
                amountToChain -= 1;
                Instantiate(chainLightningEffect, collision.gameObject.transform.position, Quaternion.identity);
                Instantiate(beenStruck, collision.gameObject.transform);

                collision.gameObject.GetComponent<EnemyScript>().TakeDamage(damage, false, false, true);

                anim.StopPlayback();
                coll.enabled = false;

                singleSpawns--;

                particleSys.Play();

                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = startObject.transform.position;
                particleSys.Emit(emitParams, 1);

                emitParams.position = endObject.transform.position;
                particleSys.Emit(emitParams, 1);

                emitParams.position = (startObject.transform.position + endObject.transform.position) / 2;
                particleSys.Emit(emitParams, 1);
            }
        }
    }
}
