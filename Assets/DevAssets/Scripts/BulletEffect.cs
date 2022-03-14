using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    private const string Enemy = "Enemy";
    private const string Terrain = "Terrain";
    private const string Cover = "Cover";
    private const string Player = "Player";

    public float DetectionRange = 2;
    public float setScaling = 1;

    private WaitForSeconds Wait = new WaitForSeconds(3);
    private WaitForSeconds ImpactDelay = new WaitForSeconds(0.25f);
    public GameObject BulletPool = null;
    public string rootParentTag = "";
    private ParticleSystem ImpactParticleEffect = null;
    private float ImpactParticleDuration = 0;
    private Vector3 resetScalingValue = Vector3.zero;

    public Rigidbody rb = null;
    public CapsuleCollider BulletCollider = null;
    public GameObject Bullet = null;
    public GameObject Impact = null;

    private void Awake()
    {

        if (BulletPool == null) BulletPool = transform.parent.gameObject;
        setScaling = gameObject.transform.localScale.x;
        resetScalingValue = new Vector3(setScaling, setScaling, setScaling);
        ImpactParticleEffect = Impact.GetComponent<ParticleSystem>();
        //float impactPSDuration = ImpactParticleEffect.main.duration;
        //float impactPSStartLifeTime = ImpactParticleEffect.main.startLifetimeMultiplier;
        //ImpactParticleDuration = impactPSDuration + impactPSStartLifeTime;
        ImpactDelay = new WaitForSeconds(ImpactParticleDuration);
    }

    private void Start()
    {
        StartCoroutine(OffLimit());
    }

    private void Update()
    {
        if (BulletPool != null) rootParentTag = BulletPool.transform.root.gameObject.tag;

        isNotMovingAndScallingController();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRange);
        foreach (Collider hitCollider in hitColliders)
        {
            BulletDecider(hitCollider);
        }
    }

    private void BulletDecider(Collider hitCollider)
    {
        string hitColliderTag = hitCollider.gameObject.tag;
        if (rootParentTag.Equals(Player)) 
        {
            switch (hitColliderTag)
            {
                case Terrain:
                    BulletCleaner();
                    break;
                case Cover:
                    BulletCleaner();
                    break;
                case Enemy:
                    BulletCleaner();
                    break;
                default:
                    break;
            }
        }
        else if (rootParentTag.Equals(Enemy))
        {
            switch (hitColliderTag)
            {
                case Terrain:
                    BulletCleaner();
                    break;
                case Cover:
                    BulletCleaner();
                    break;
                case Player:
                    BulletCleaner();
                    break;
                default:
                    break;
            }
        }
    }

    private void BulletCleaner() 
    {
        Bullet.SetActive(false);
        BulletCollider.enabled = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        Impact.SetActive(true);
    }

    private void isNotMovingAndScallingController()
    {
        if (gameObject.transform.localScale.x > setScaling) gameObject.transform.localScale = resetScalingValue;
        if (rb.IsSleeping()) 
        {
            Bullet.SetActive(false);
            BulletCollider.enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
            transform.SetParent(BulletPool.transform, true);
        }
    }

    private IEnumerator OffLimit() 
    {
        yield return Wait;
        BulletCollider.enabled = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        Bullet.SetActive(false);
        Impact.SetActive(true);
        //StartCoroutine(ImpactParticleEffectDelay());
    }
}
