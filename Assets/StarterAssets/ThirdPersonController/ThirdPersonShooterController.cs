using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float aimSensitivity;  // sensitivty for mouse control
    [SerializeField] private float moveSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform bulletStartPosition;

 
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animatorController;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animatorController = GetComponent<Animator>();

    }

 
    private void Update()
    {

        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform aimTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycasthit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycasthit.point;
            aimTransform = raycasthit.transform;
            //Debug.Log(raycasthit.point);
        }
        //targetGun(mouseWorldPosition, starterAssetsInputs.shoot);

        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;  // only make rotate at x-z plain
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            setShootPos();
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);  // make player rotate to aimDirection
        } else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(moveSensitivity);
            thirdPersonController.SetRotateOnMove(true);
        }

        //if (starterAssetsInputs.shoot)
        //{
        //    targetGun(mouseWorldPosition, starterAssetsInputs.shoot);

        //    Vector3 aimDir = (mouseWorldPosition - bulletStartPosition.position).normalized;  // projecile direction to go
        //    Instantiate(pfBulletProjectile, bulletStartPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        //    setShootPos();
        //    starterAssetsInputs.shoot = false; // no continue shoot
        //}

    }

    private void setShootPos()
    {    
        Debug.Log("set shoot animator");
        animatorController.SetTrigger(Animator.StringToHash("Shoot"));   
    }

    private void targetGun(Vector3 mouseWorldPosition, bool shootStatus)
    {
        Transform gunTransform = this.transform.Find("GunHeavy");
        if (gunTransform != null)
        {
            if (shootStatus)
            {
                Debug.Log("shooting pos");
                gunTransform.rotation = Quaternion.Euler(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
            }
        }
    }


}

