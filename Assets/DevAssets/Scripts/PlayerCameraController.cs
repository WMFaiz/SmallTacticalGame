using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    private static string PlayerController = "PlayerController";

    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    private GameObject target;
    private Camera maincamera;
    private PlayerController _playerController;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag(PlayerController).GetComponent<PlayerController>();
        maincamera = gameObject.GetComponent<Camera>();
        target = _playerController.CurrentControledUnit;
    }

    private void Update()
    {
        if(target != _playerController.CurrentControledUnit) 
            target = _playerController.CurrentControledUnit;

        if(target != null) 
        {
            desiredPosition = target.transform.position + offset;
            smoothedPosition = Vector3.MoveTowards(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            transform.LookAt(target.transform.position);
        }

        //desiredPosition = target.transform.position + offset;
        //smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        //transform.position = smoothedPosition;
        //transform.LookAt(target.transform.position);

    }

    private bool IsVisible(Camera camera, GameObject target) 
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        Vector3 point = target.transform.position;

        foreach (var plane in planes) 
        {
            if (plane.GetDistanceToPoint(point) > 0) 
            {   
                return false;
            }
        }
        return true;
    }
}
