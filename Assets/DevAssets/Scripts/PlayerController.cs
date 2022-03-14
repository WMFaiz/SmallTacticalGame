using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private const string Soldiers_Customizable = "Soldiers_Customizable";
    private const string Player = "Player";
    private const string Enemy = "Enemy";
    private const string Terrain = "Terrain";

    public GameObject CurrentControledUnit;
    public NavMeshAgent CurrentUnitAgent = null;
    public Animator CurrentUnitAnimator = null;
    public CharacterPlayerMovement CurrentCharacterPlayerMovement = null;

    private void Start()
    {
        GameObject[] listofPlayer = GameObject.FindGameObjectsWithTag(Player);
        CurrentControledUnit = listofPlayer[0];
        CurrentUnitAgent = CurrentControledUnit.GetComponent<NavMeshAgent>();
        CurrentUnitAnimator = CurrentControledUnit.transform.Find(Soldiers_Customizable).gameObject.GetComponent<Animator>();
        CurrentCharacterPlayerMovement = CurrentControledUnit.GetComponent<CharacterPlayerMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (hit.transform.gameObject.tag)
                {
                    case Player:
                        CurrentControledUnit = hit.transform.gameObject;
                        CurrentUnitAgent = CurrentControledUnit.GetComponent<NavMeshAgent>();
                        CurrentUnitAnimator = CurrentControledUnit.transform.Find(Soldiers_Customizable).gameObject.GetComponent<Animator>();
                        CurrentCharacterPlayerMovement = CurrentControledUnit.GetComponent<CharacterPlayerMovement>();
                        break;
                    default:
                        break;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                switch (hit.transform.gameObject.tag)
                {
                    case Enemy:
                        if (CurrentCharacterPlayerMovement != null)
                        {
                            CurrentCharacterPlayerMovement.AttackEnemy = hit.transform.gameObject;
                        }
                        break;
                    case Terrain:
                        if (CurrentCharacterPlayerMovement != null)
                        {
                            CurrentCharacterPlayerMovement.TerrainMoveTo(hit);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            GameObject[] listofPlayer = GameObject.FindGameObjectsWithTag(Player);
            CurrentControledUnit = listofPlayer[0];
            CurrentUnitAgent = CurrentControledUnit.GetComponent<NavMeshAgent>();
            CurrentUnitAnimator = CurrentControledUnit.transform.Find(Soldiers_Customizable).gameObject.GetComponent<Animator>();
            CurrentCharacterPlayerMovement = CurrentControledUnit.GetComponent<CharacterPlayerMovement>();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject[] listofPlayer = GameObject.FindGameObjectsWithTag(Player);
            CurrentControledUnit = listofPlayer[1];
            CurrentUnitAgent = CurrentControledUnit.GetComponent<NavMeshAgent>();
            CurrentUnitAnimator = CurrentControledUnit.transform.Find(Soldiers_Customizable).gameObject.GetComponent<Animator>();
            CurrentCharacterPlayerMovement = CurrentControledUnit.GetComponent<CharacterPlayerMovement>();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameObject[] listofPlayer = GameObject.FindGameObjectsWithTag(Player);
            CurrentControledUnit = listofPlayer[2];
            CurrentUnitAgent = CurrentControledUnit.GetComponent<NavMeshAgent>();
            CurrentUnitAnimator = CurrentControledUnit.transform.Find(Soldiers_Customizable).gameObject.GetComponent<Animator>();
            CurrentCharacterPlayerMovement = CurrentControledUnit.GetComponent<CharacterPlayerMovement>();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameObject[] listofPlayer = GameObject.FindGameObjectsWithTag(Player);
            CurrentControledUnit = listofPlayer[3];
            CurrentUnitAgent = CurrentControledUnit.GetComponent<NavMeshAgent>();
            CurrentUnitAnimator = CurrentControledUnit.transform.Find(Soldiers_Customizable).gameObject.GetComponent<Animator>();
            CurrentCharacterPlayerMovement = CurrentControledUnit.GetComponent<CharacterPlayerMovement>();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GameObject[] listofPlayer = GameObject.FindGameObjectsWithTag(Player);
            CurrentControledUnit = listofPlayer[4];
            CurrentUnitAgent = CurrentControledUnit.GetComponent<NavMeshAgent>();
            CurrentUnitAnimator = CurrentControledUnit.transform.Find(Soldiers_Customizable).gameObject.GetComponent<Animator>();
            CurrentCharacterPlayerMovement = CurrentControledUnit.GetComponent<CharacterPlayerMovement>();
        }
    }
}
