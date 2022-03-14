using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEnviromentDetector : MonoBehaviour
{
    private const string Crouch = "Crouch";
    private const string Cover = "Cover";
    private Animator animator = null;
    private CharacterPlayerMovement characterPlayerMovement = null;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        characterPlayerMovement = transform.parent.gameObject.GetComponent<CharacterPlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case Cover:
                animator.SetBool(Crouch, true);
                characterPlayerMovement.isCrouching = true;
                characterPlayerMovement.CrouchDetected = true;
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case Cover:
                animator.SetBool(Crouch, false);
                characterPlayerMovement.isCrouching = true;
                characterPlayerMovement.CrouchDetected = false;
                break;
            default:
                break;
        }
    }
}
