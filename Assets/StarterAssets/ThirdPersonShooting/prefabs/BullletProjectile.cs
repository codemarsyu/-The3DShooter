using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SwordFighter;

public class BullletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigitbody;
    public float projectileSpeed = 40f;
    //[SerializeField] private Transform vfxGreen;

    private void Awake()
    {
        bulletRigitbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bulletRigitbody.velocity = transform.forward * projectileSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == 3)
        {
            //hit target
            if (other.transform.parent.GetComponent<enemyController>() != null)
            {
                enemyController _targetController = other.transform.parent.GetComponent<enemyController>();
                _targetController.DoTheHarm(transform.position);
            }

        } else
        {
            // hit something else
            //Instantiate(vfxGreen, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}

