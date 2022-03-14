using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType 
{
    Akimbo,
    Handgun,
    Heavy,
    Infantry,
    Knife,
    Machinegun,
    RocketLauncher
}

public enum AttackType
{
    Shoot,
    Bolt,
    Burst,
    Shotgun,
    Loop
}

public class CharacterPlayerMovement : MonoBehaviour
{
    private const string Walk = "Walk";
    private const string Run = "Run";
    private const string Crouch = "Crouch";
    private const string Reload = "Reload";
    private const string infantry_combat_shoot = "infantry_combat_shoot";
    private const string infantry_combat_shoot_bolt = "infantry_combat_shoot_bolt";
    private const string infantry_combat_shoot_burst = "infantry_combat_shoot_burst";
    private const string infantry_combat_shoot_shotgun = "infantry_combat_shoot_shotgun";
    private const string infantry_crouch_idle = "infantry_crouch_idle";
    private const string infantry_crouch_walk = "infantry_crouch_walk";
    private const string infantry_crouch_reload = "infantry_crouch_reload";
    private const string infantry_combat_reload = "infantry_combat_reload";
    private const string handgun_crouch_reload = "handgun_crouch_reload";
    private const string handgun_combat_reload = "handgun_combat_reload";
    private const string heavy_crouch_reload = "heavy_crouch_reload";
    private const string machinegun_crouch_reload = "machinegun_crouch_reload";
    private const string machinegun_combat_reload = "machinegun_combat_reload";
    private const string rocketlauncher_crouch_reload = "rocketlauncher_crouch_reload";
    private const string rocketlauncher_combat_reload = "rocketlauncher_combat_reload";
    private const string heavy_combat_reload = "heavy_combat_reload";
    private const string _PlayerController = "PlayerController";
    private const string Soldiers_Customizable = "Soldiers_Customizable";
    private const string Terrain = "Terrain";
    private const string Enemy = "Enemy";

    private float AttackCooldownPlaceholder;
    private float AttackCooldownCounter;

    private NavMeshAgent agent = null;
    private Animator animator = null;
    private PlayerController PlayerController = null;
    private Customization customization;
    private CharacterResource characterResource = null;
    private bool _toggleRun = false;
    private bool isMoving = false;
    private float random_y_rotate = 0;

    [HideInInspector]
    public bool CrouchDetected = false;
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool isCrouching = false;
    [HideInInspector]
    public GameObject AttackEnemy = null;

    [Header("Types")]
    public UnitType UnitType = new UnitType();
    public AttackType AttackType = new AttackType();
    [Header("Attack Customization")]
    public float AttackRange = 3.0f;
    public float AttackCooldown = 0.5f;
    public float AttackCooldownSmoother = 0.17f;
    [Header("Movement Speed")]
    public float RunningSpeed = 6;
    public float WalkingSpeed = 4;

    private void Awake()
    {
        characterResource = gameObject.GetComponent<CharacterResource>();
        customization = gameObject.transform.Find(Soldiers_Customizable).GetComponent<Customization>();
        PlayerController = GameObject.Find(_PlayerController).GetComponent<PlayerController>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        animator = gameObject.transform.Find(Soldiers_Customizable).gameObject.GetComponent<Animator>();
        AttackCooldownCounter = 0;
        AttackCooldownPlaceholder = AttackCooldown;
    }

    private void Start()
    {
        GetUnitType();
    }

    private void Update()
    {
        ToggleRun();
        MoveAndAttack();
        CharacterMovementChecker();
    }

    private void ToggleRun() 
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            _toggleRun = !_toggleRun;

            if (_toggleRun)
            {
                agent.speed = RunningSpeed;
            }
            else 
            {
                agent.speed = WalkingSpeed;
            }
        }
    }

    public void TerrainMoveTo(RaycastHit hit) 
    {
        AttackEnemy = null;
        PlayerController.CurrentUnitAgent.stoppingDistance = 0;
        Vector3 target = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        NavMeshPath path = new NavMeshPath();
        PlayerController.CurrentUnitAgent.CalculatePath(target, path);
        if (path.status != NavMeshPathStatus.PathPartial && path.status != NavMeshPathStatus.PathInvalid)
            PlayerController.CurrentUnitAgent.SetDestination(target);
    }

    private void MoveAndAttack() 
    {
        if (AttackEnemy != null)
        {
            agent.stoppingDistance = AttackRange;
            agent.SetDestination(AttackEnemy.transform.position);
            float distance = Vector3.Distance(transform.position, AttackEnemy.transform.position);
            if (distance <= AttackRange)
            {
                Vector3 enemyPos = new Vector3(AttackEnemy.transform.position.x, transform.position.y, AttackEnemy.transform.position.z);
                transform.LookAt(enemyPos);
                if (GetAttackAnimationStatus())
                {
                    if (AttackCooldownCounter <= 0)
                    {
                        if (characterResource.bullets > 0)
                        {
                            if (GetReloadAnimationStatus())
                            {
                                if (isCrouching)
                                {
                                    if (characterResource.bullets > 0)
                                    {
                                        SetCrouch(false, 500);
                                    }
                                }
                                else if (!isCrouching)
                                {
                                    GetAttackType();
                                    AttackCooldownCounter = AttackCooldownPlaceholder;
                                }
                            }
                        }
                        else if (characterResource.bullets <= 0)
                        {
                            if (!GetReloadAnimationStatus())
                            {
                                characterResource.bulletsMax -= characterResource.reloadAmount;
                                characterResource.bullets += characterResource.reloadAmount;
                            }
                            else if (GetReloadAnimationStatus())
                            {
                                animator.SetTrigger(Reload);
                            }
                        }
                    }
                }
                agent.SetDestination(transform.position);
            }
            else if (distance > AttackRange)
            {
                Vector3 enemyPos = new Vector3(AttackEnemy.transform.position.x, transform.position.y, AttackEnemy.transform.position.z);
                transform.LookAt(enemyPos);
            }
        }

        if (AttackCooldownCounter > 0)
        {
            AttackCooldownCounter -= AttackCooldownSmoother * Time.deltaTime;
            if (CrouchDetected)
                SetCrouch(true, 500);
        }
    }
    private void GetUnitType()
    {
        switch (UnitType)
        {
            case UnitType.Akimbo:
                animator.SetBool(UnitType.Akimbo.ToString(), true);
                break;
            case UnitType.Handgun:
                animator.SetBool(UnitType.Handgun.ToString(), true);
                break;
            case UnitType.Heavy:
                animator.SetBool(UnitType.Heavy.ToString(), true);
                break;
            case UnitType.Infantry:
                animator.SetBool(UnitType.Infantry.ToString(), true);
                break;
            case UnitType.Knife:
                animator.SetBool(UnitType.Knife.ToString(), true);
                break;
            case UnitType.Machinegun:
                animator.SetBool(UnitType.Machinegun.ToString(), true);
                break;
            case UnitType.RocketLauncher:
                animator.SetBool(UnitType.RocketLauncher.ToString(), true);
                break;
            default:
                break;
        }
    }

    private async void GetAttackType()
    {
        if (!isMoving) 
        {
            switch (AttackType)
            {
                case AttackType.Shoot:
                    animator.SetTrigger(AttackType.Shoot.ToString());
                    OpenFireOnce();
                    break;
                case AttackType.Bolt:
                    animator.SetTrigger(AttackType.Bolt.ToString());
                    OpenFireOnce();
                    break;
                case AttackType.Burst:
                    animator.SetTrigger(AttackType.Burst.ToString());
                    await OpenFireBurst();
                    break;
                case AttackType.Shotgun:
                    animator.SetTrigger(AttackType.Shotgun.ToString());
                    await OpenFireShotgun();
                    break;
                case AttackType.Loop:
                    animator.SetBool(AttackType.Loop.ToString(), true);
                    await OpenFireLoop();
                    break;
                default:
                    break;
            }
        }
    }

    private async Task GetMuzzleDisable() 
    {
        GameObject getGameobject = customization.selectedMuzzle;
        getGameobject.SetActive(true);
        await Task.Delay(50);
        getGameobject.SetActive(false);
    }

    private void ResetBullets() 
    {
        foreach (BulletEffect be in customization._RBulletEffect)
        {
            be.Impact.SetActive(false);
            be.Bullet.SetActive(true);
            be.BulletCollider.enabled = true;
            be.gameObject.SetActive(false);
        }
    }

    private void ResetBulletSettings(Rigidbody rb)
    {
        rb.transform.SetParent(customization.BulletsPool.transform, true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        rb.transform.localPosition = Vector3.zero;
        rb.transform.localRotation = Quaternion.Euler(0, -90, 0);
        rb.transform.localScale = Vector3.one;
        customization._Bullets.Add(rb.gameObject);

        foreach (BulletEffect be in customization._RBulletEffect)
        {
            be.Impact.SetActive(false);
            be.Bullet.SetActive(true);
            be.BulletCollider.enabled = true;
        }
    }
    private void ResetRandomBulletSettings(Rigidbody rb)
    {
        rb.transform.SetParent(customization.BulletsPool.transform, true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        rb.transform.localPosition = Vector3.zero;
        random_y_rotate = Random.Range(-85, -95);
        rb.transform.localRotation = Quaternion.Euler(0, random_y_rotate, 0);
        rb.transform.localScale = Vector3.one;
        customization._Bullets.Add(rb.gameObject);

        foreach (BulletEffect be in customization._RBulletEffect)
        {
            be.Impact.SetActive(false);
            be.Bullet.SetActive(true);
            be.BulletCollider.enabled = true;
        }
    }

    private void OpenFireOnce() 
    {
        if (characterResource.bullets > 0) 
        {
            if (customization._Bullets.Count > 0)
            {
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (customization._Bullets.Contains(rb.gameObject))
                    {
                        rb.gameObject.SetActive(true);
                        rb.transform.parent = null;
                        rb.AddForce(rb.transform.forward * 2000);
                        customization._Bullets.Remove(rb.gameObject);
                        characterResource.bullets -= 1;
                        break;
                    }
                }
                isAttacking = false;
            }
            else if (customization._Bullets.Count <= 0)
            {
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (!customization._Bullets.Contains(rb.gameObject))
                    {
                        ResetBulletSettings(rb);
                        ResetBullets();
                    }
                }
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (customization._Bullets.Contains(rb.gameObject))
                    {
                        rb.gameObject.SetActive(true);
                        rb.transform.parent = null;
                        rb.AddForce(rb.transform.forward * 2000);
                        customization._Bullets.Remove(rb.gameObject);
                        characterResource.bullets -= 1;
                        break;
                    }
                }
                isAttacking = false;
            }
        }
    }

    private async Task OpenFireBurst()
    {
        if(characterResource.bullets > 0) 
        {
            int interval = 0;
            if (customization._Bullets.Count > 0)
            {
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (customization._Bullets.Contains(rb.gameObject))
                    {
                        await GetMuzzleDisable();
                        rb.gameObject.SetActive(true);
                        rb.transform.parent = null;
                        rb.AddForce(rb.transform.forward * 2000);
                        customization._Bullets.Remove(rb.gameObject);
                        characterResource.bullets -= 1;
                        await Task.Delay(50);
                    }
                }
                isAttacking = false;
            }
            else if (customization._Bullets.Count <= 0)
            {
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (!customization._Bullets.Contains(rb.gameObject))
                    {
                        if (interval <= characterResource.bulletsPerRound)
                        {
                            await GetMuzzleDisable();
                            ResetBulletSettings(rb);
                            rb.transform.parent = null;
                            rb.AddForce(rb.transform.forward * 2000);
                            customization._Bullets.Remove(rb.gameObject);
                            characterResource.bullets -= 1;
                            await Task.Delay(50);
                        }
                        interval++;
                    }
                }
                interval = 0;
                isAttacking = false;
            }
        }
    }

    private async Task OpenFireLoop()
    {
        if (characterResource.bullets > 0)
        {
            int interval = 0;
            if (customization._Bullets.Count > 0)
            {
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (customization._Bullets.Contains(rb.gameObject))
                    {
                        await GetMuzzleDisable();
                        rb.gameObject.SetActive(true);
                        rb.transform.parent = null;
                        rb.AddForce(rb.transform.forward * 2000);
                        customization._Bullets.Remove(rb.gameObject);
                        characterResource.bullets -= 1;
                        await Task.Delay(50);
                    }
                }
                animator.SetBool(AttackType.Loop.ToString(), false);
                isAttacking = false;
            }
            else if (customization._Bullets.Count <= 0)
            {
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (!customization._Bullets.Contains(rb.gameObject))
                    {
                        if (interval <= characterResource.bulletsPerRound)
                        {
                            await GetMuzzleDisable();
                            ResetBulletSettings(rb);
                            rb.transform.parent = null;
                            rb.AddForce(rb.transform.forward * 2000);
                            customization._Bullets.Remove(rb.gameObject);
                            characterResource.bullets -= 1;
                            await Task.Delay(50);
                        }
                        interval++;
                    }
                }
                interval = 0;
                animator.SetBool(AttackType.Loop.ToString(), false);
                isAttacking = false;
            }
        }
    }

    private async Task OpenFireShotgun()
    {
        if (characterResource.bullets > 0) 
        {
            if (customization._Bullets.Count > 0)
            {
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (customization._Bullets.Contains(rb.gameObject))
                    {
                        rb.gameObject.SetActive(true);
                        rb.transform.parent = null;
                        rb.AddForce(rb.transform.forward * 2000);
                        customization._Bullets.Remove(rb.gameObject);
                        characterResource.bullets -= 1;
                        await Task.Delay(10);
                    }
                }
                isAttacking = false;
            }
            else if (customization._Bullets.Count <= 0)
            {
                foreach (Rigidbody rb in customization._RBBullets)
                {
                    isAttacking = true;
                    if (!customization._Bullets.Contains(rb.gameObject))
                    {
                        ResetRandomBulletSettings(rb);
                        rb.transform.parent = null;
                        rb.AddForce(rb.transform.forward * 2000);
                        customization._Bullets.Remove(rb.gameObject);
                        characterResource.bullets -= 1;
                        await Task.Delay(10);
                    }
                }
                isAttacking = false;
            }
        }
    }

    private async void SetCrouch(bool crouchEnabled, int wait)
    {
        animator.SetBool(Crouch, crouchEnabled);
        await Task.Delay(wait);
        isCrouching = crouchEnabled;
    }

    private bool GetAttackAnimationStatus()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(infantry_combat_shoot) && 
            !animator.GetCurrentAnimatorStateInfo(0).IsName(infantry_combat_shoot_bolt) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(infantry_combat_shoot_burst) && 
            !animator.GetCurrentAnimatorStateInfo(0).IsName(infantry_combat_shoot_shotgun))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool GetReloadAnimationStatus() 
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(infantry_combat_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(infantry_crouch_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(handgun_crouch_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(handgun_combat_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(heavy_crouch_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(heavy_combat_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(machinegun_combat_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(machinegun_crouch_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(rocketlauncher_crouch_reload) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName(rocketlauncher_combat_reload))
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    private void CharacterMovementChecker() 
    {
        if (!_toggleRun)
        {
            if (agent.remainingDistance > agent.stoppingDistance && agent.velocity != Vector3.zero)
            {
                animator.SetBool(Walk, true);
                animator.SetBool(Run, false);
                isMoving = true;
            }
            else if (agent.remainingDistance <= agent.stoppingDistance && agent.velocity == Vector3.zero)
            {
                animator.SetBool(Walk, false);
                animator.SetBool(Run, false);
                isMoving = false;
            }
        }
        else 
        {
            if (agent.remainingDistance > agent.stoppingDistance && agent.velocity != Vector3.zero)
            {
                animator.SetBool(Walk, false);
                animator.SetBool(Run, true);
                isMoving = true;
            }
            else if (agent.remainingDistance <= agent.stoppingDistance && agent.velocity == Vector3.zero)
            {
                animator.SetBool(Walk, false);
                animator.SetBool(Run, false);
                isMoving = false;
            }
        }
    }
}
