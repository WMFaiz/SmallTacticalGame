using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public enum UnitRole 
{
    Assault,
    Heavy,
    Melee,
    Worker
}

public enum UnitMode 
{
    RandomRoaming,
    RoundRobin,
}

public enum UnitAttackType
{
    Shoot,
    Melee,
    Burst,
    Shotgun,
    Loop
}

public class BattleDroidsController : MonoBehaviour
{
    private const string melee_combat_attack_A = "melee_combat_attack_A";
    private const string melee_combat_attack_B = "melee_combat_attack_B";
    private const string assault_combat_shoot = "assault_combat_shoot";
    private const string assault_combat_shoot_burst = "assault_combat_shoot_burst";
    private const string heavy_combat_shoot = "heavy_combat_shoot";
    private const string heavy_combat_shoot_burst = "heavy_combat_shoot_burst";
    private const string heavy_combat_shoot_loop = "heavy_combat_shoot_loop";
    private const string worker_combat_attack_A = "worker_combat_attack_A";
    private const string worker_combat_attack_B = "worker_combat_attack_B";

    private const string Walk = "Walk";
    private const string Run = "Run";
    private const string Assault = "Assault";
    private const string Heavy = "Heavy";
    private const string Melee = "Melee";
    private const string Worker = "Worker";
    private const string Player = "Player";
    private const string Terrain = "Terrain";
    private const string Cover = "Cover";
    private const string Hit = "Hit";
    private const string HitAnimNumber = "HitAnimNumber";

    private Animator animator = null;
    private NavMeshAgent agent;
    private float RoamingCooldownPlaceholder;
    private float RoamingCooldownCounter;
    private Vector3 curPosition;
    private float AttackCooldownPlaceholder;
    private float AttackCooldownCounter;
    private GameObject _target = null;
    private GameObject controlledUnit = null;
    private List<GameObject> UnitPrefeb = new List<GameObject>();
    private GameObject selectedMuzzle = null;
    private GameObject selectedBulletContainer = null;
    private bool isAttacking = false;
    private List<BulletEffect> bulletEffect = new List<BulletEffect>();

    [Header("Purpose")]
    public UnitRole unitRole = new UnitRole();
    public UnitMode unitMode = new UnitMode();
    public UnitAttackType unitAttackType = new UnitAttackType();
    public List<GameObject> points = null;
    [Header("Character Settings")]
    public float AttackRange = 3.0f;
    public float AttackCooldown = 0.5f;
    public float AttackCooldownSmoother = 0.17f;
    public float walkSpeed = 4.0f;
    public float runSpeed = 6.0f;
    [Header("Roaming Settings")]
    public float RoamingRange = 30.0f;
    public float RoamingCooldownMin = 10.0f;
    public float RoamingCooldownMax = 20.0f;
    public float RoamingCooldownSmoother = 0.17f;
    [Header("Agro Settings")]
    public float AgroRange = 10.0f;
    public float AgroRangeFollowUp = 20.0f;
    public float AgroRangeGiveUp = 30.0f;
    [Header("Bullets")]
    public int BulletPerRound = 5;
    public List<GameObject> Muzzle = new List<GameObject>();
    public List<GameObject> BulletsContainers = new List<GameObject>();
    public List<Rigidbody> RBBullets = new List<Rigidbody>();


    private void Awake()
    {
        foreach (Transform child in gameObject.transform)
        {
            UnitPrefeb.Add(child.gameObject);
            if (child.gameObject.activeSelf) controlledUnit = child.gameObject;
        }

        animator = controlledUnit.GetComponent<Animator>();
        curPosition = gameObject.transform.position;
        agent = gameObject.GetComponent<NavMeshAgent>();
        AttackCooldownCounter = 0;
        AttackCooldownPlaceholder = AttackCooldown;
    }

    private void Start()
    {
        AssignRole();
    }

    private void Update()
    {
        Attack();
        MoveChecker();
        RoamMode();
    }

    private void RoamMode()
    {
        switch (unitMode)
        {
            case UnitMode.RandomRoaming:
                RandomRoaming();
                break;
            case UnitMode.RoundRobin:
                RoundRobin();
                break;
            default:
                break;
        }
    }

    private void AssignRole()
    {
        switch (unitRole)
        {
            case UnitRole.Assault:
                GetBlltsRBsSwtchNReset(0);
                animator.SetBool(Assault, true);
                break;
            case UnitRole.Heavy:
                GetBlltsRBsSwtchNReset(1);
                animator.SetBool(Heavy, true);
                break;
            case UnitRole.Melee:
                switchAndResetUnitPrefeb(2);
                animator.SetBool(Melee, true);
                break;
            case UnitRole.Worker:
                switchAndResetUnitPrefeb(3);
                animator.SetBool(Worker, true);
                break;
            default:
                break;
        }
    }

    private void GetBlltsRBsSwtchNReset(int i)
    {
        switchAndResetUnitPrefeb(i);
        selectedMuzzle = Muzzle[i];
        selectedBulletContainer = BulletsContainers[i];
        foreach (Transform child in BulletsContainers[i].transform)
        {
            Rigidbody childRB = child.gameObject.GetComponent<Rigidbody>();
            BulletEffect childBE = childRB.GetComponent<BulletEffect>();
            RBBullets.Add(childRB);
            bulletEffect.Add(childBE);
        }
    }

    private async void GetAttackType()
    {
        switch (unitAttackType)
        {
            case UnitAttackType.Shoot:
                animator.SetTrigger(UnitAttackType.Shoot.ToString());
                //OpenFireOnce();
                break;
            case UnitAttackType.Melee:
                animator.SetTrigger(UnitAttackType.Melee.ToString());
                MeleeAttack();
                break;
            case UnitAttackType.Burst:
                animator.SetTrigger(UnitAttackType.Burst.ToString());
                await OpenFireBurst();
                break;
            case UnitAttackType.Shotgun:
                animator.SetTrigger(UnitAttackType.Shotgun.ToString());
                //OpenFireShotgun();
                break;
            case UnitAttackType.Loop:
                animator.SetBool(UnitAttackType.Loop.ToString(), true);
                //await OpenFireLoop();
                break;
            default:
                break;
        }
    }

    private void switchAndResetUnitPrefeb(int enabledInt)
    {
        UnitPrefeb.ForEach(o =>{ o.SetActive(false); });

        UnitPrefeb[enabledInt].SetActive(true);

        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.activeSelf) controlledUnit = child.gameObject;
        }

        animator = controlledUnit.GetComponent<Animator>();
    }

    private void Attack()
    {
        Aggro();
        if (_target != null)
        {
            agent.stoppingDistance = AttackRange;
            agent.SetDestination(_target.transform.position);
            float distance = Vector3.Distance(transform.position, _target.transform.position);
            if (distance <= agent.stoppingDistance)
            {
                Vector3 lookThis = new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z);
                transform.LookAt(lookThis);
                if (AttackCooldownCounter <= 0)
                {
                    if (GetAttackAnimationStatus())
                    {
                        GetAttackType();
                        AttackCooldownCounter = AttackCooldownPlaceholder;
                    }
                }
            }
            else if (distance > agent.stoppingDistance) 
            {
                if (distance > AgroRangeFollowUp)
                {
                    agent.stoppingDistance = AttackRange;
                    agent.SetDestination(_target.transform.position);
                }

                if (distance > AgroRangeGiveUp)
                {
                    _target = null;

                    agent.stoppingDistance = 0;
                    agent.ResetPath();
                    agent.isStopped = true;
                    agent.destination = transform.position;

                    RoamMode();
                }
            }
        }

        if (AttackCooldownCounter > 0) 
        { 
            AttackCooldownCounter -= AttackCooldownSmoother * Time.deltaTime; 
        }
    }

    private void MoveChecker()
    {
        if (agent.enabled)
        {
            
            if(_target == null) 
            {
                agent.speed = walkSpeed;
                animator.SetBool(Run, false);
                if (agent.remainingDistance > agent.stoppingDistance && agent.velocity != Vector3.zero)
                {
                    animator.SetBool(Walk, true);
                }
                else if (agent.remainingDistance <= agent.stoppingDistance && agent.velocity == Vector3.zero)
                {
                    animator.SetBool(Walk, false);
                }
            }
            else if(_target != null) 
            {
                agent.speed = runSpeed;
                animator.SetBool(Walk, false);
                if (agent.remainingDistance > agent.stoppingDistance && agent.velocity != Vector3.zero)
                {
                    animator.SetBool(Run, true);
                }
                else if (agent.remainingDistance <= agent.stoppingDistance && agent.velocity == Vector3.zero)
                {
                    animator.SetBool(Run, false);
                }
            }
        }
    }

    private void Aggro() 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AgroRange);
        foreach (Collider hitCollider in hitColliders)
        {
            switch (hitCollider.gameObject.tag)
            {
                case Player:
                    if (agent.enabled)
                    {
                        _target = hitCollider.gameObject;
                        agent.stoppingDistance = AttackRange;
                        agent.SetDestination(_target.transform.position);
                    }
                    break;
                case Terrain:
                    break;
                case Cover:
                    break;
                default:
                    break;
            }
        }
    }

    private void RandomRoaming()
    {
        if (_target == null)
        {
            if (agent.enabled)
            {
                if (RoamingCooldownCounter <= 0)
                {
                    float x = Random.Range(transform.position.x - RoamingRange, transform.position.x + RoamingRange);
                    float y = 0;
                    float z = Random.Range(transform.position.z - RoamingRange, transform.position.z + RoamingRange);
                    Vector3 moveTo = new Vector3(x, y, z);
                    float distance = Vector3.Distance(curPosition, moveTo);
                    if (distance <= RoamingRange)
                    {
                        agent.SetDestination(moveTo);
                        RoamingCooldownPlaceholder = Random.Range(RoamingCooldownMin, RoamingCooldownMax);
                        RoamingCooldownCounter = RoamingCooldownPlaceholder;
                    }
                    else if (distance > RoamingRange)
                    {
                        agent.SetDestination(curPosition);
                    }
                }
                else if (RoamingCooldownCounter > 0)
                {
                    RoamingCooldownCounter -= RoamingCooldownSmoother * Time.deltaTime;
                }
            }
        }
        else 
        {
            RoamingCooldownPlaceholder = Random.Range(RoamingCooldownMin, RoamingCooldownMax);
            RoamingCooldownCounter = RoamingCooldownPlaceholder;
        }
    }

    private void RoundRobin()
    {
        if (_target == null) 
        {
            if (points.Count > 0)
            {
                if (agent.enabled)
                {
                    if (points[0] != null)
                    {
                        agent.SetDestination(points[0].transform.position);
                    }
                    float distance = Vector3.Distance(transform.position, points[0].transform.position);
                    if (distance < 0.5f)
                    {
                        GameObject storeGO = points[0];
                        points.Remove(points[0]);
                        points.Add(storeGO);
                    }
                }
            }
            else
            {
                Debug.Log("No Points is detected");
            }
        }
    }

    private bool GetAttackAnimationStatus()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(melee_combat_attack_A) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(melee_combat_attack_B) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(assault_combat_shoot) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(assault_combat_shoot_burst) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(heavy_combat_shoot) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(heavy_combat_shoot_burst) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(heavy_combat_shoot_loop) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(worker_combat_attack_A) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(worker_combat_attack_B))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task GetMuzzleDisable()
    {
        selectedMuzzle.SetActive(true);
        await Task.Delay(50);
        selectedMuzzle.SetActive(false); 
    }

    private void ResetBulletSettings(Rigidbody rb)
    {
        rb.transform.SetParent(selectedBulletContainer.transform, true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        rb.transform.localPosition = Vector3.zero;
        rb.transform.localRotation = Quaternion.Euler(0, 180, 0);
        rb.transform.localScale = Vector3.one;
        RBBullets.Add(rb);

        foreach (BulletEffect be in bulletEffect)
        {
            be.Impact.SetActive(false);
            be.Bullet.SetActive(true);
            be.BulletCollider.enabled = true;
        }
    }

    private void MeleeAttack() 
    {
        int randomNum = Random.Range(0, 11);
        if (randomNum == 5) { randomNum += 1; }
        animator.SetInteger(HitAnimNumber, randomNum);
        animator.SetTrigger(Hit);
    }

    private async Task OpenFireBurst()
    {
        int interval = 0;
        if (RBBullets.Count > 0)
        {
            foreach (Rigidbody rb in RBBullets.ToArray())
            {
                isAttacking = true;
                if (RBBullets.Contains(rb))
                {
                    await GetMuzzleDisable();
                    ResetBulletSettings(rb);
                    rb.gameObject.SetActive(true);
                    rb.transform.parent = null;
                    rb.AddForce(transform.forward * 2000);
                    RBBullets.Remove(rb);
                    await Task.Delay(50);
                }
            }
            isAttacking = false;
        }
        else if (RBBullets.Count <= 0)
        {
            foreach (Rigidbody rb in RBBullets.ToArray())
            {
                isAttacking = true;
                if (!RBBullets.Contains(rb))
                {
                    if (interval <= BulletPerRound)
                    {
                        await GetMuzzleDisable();
                        ResetBulletSettings(rb);
                        rb.transform.parent = null;
                        rb.AddForce(transform.forward * 2000);
                        RBBullets.Remove(rb);
                        await Task.Delay(50);
                    }
                    interval++;
                }
            }
            interval = 0;
            isAttacking = false;
        }
    }
    void OnDrawGizmos()
    {
        if (AgroRange > 0) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AgroRange);
        }

        if(AgroRangeFollowUp > 0) 
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AgroRangeFollowUp);
        }

        if (AgroRangeGiveUp > 0) 
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, AgroRangeGiveUp);
        }

        if (points.Count > 0) 
        {
            points.ForEach(delegate (GameObject go)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, go.transform.position);
            });
        }
    }
}
