using UnityEngine;
using UnityEngine.AI;
using System.Threading;

namespace SwordFighter
{


    public class enemyController : MonoBehaviour
    {
        private bool playerDetectionResult = false;
        public Transform eyeTransform;

        public Transform playerTransform;
        public LayerMask playerLayer;
        public float visionDistance, chaseDistance, attackDistance = 1.2f;

        private Animator _animator;
        private bool _hasAnimator;
        private bool attackCooling = false;
        private Vector3 original_forward;
        private Vector3 original_position;

        private NavMeshAgent agent;

        // animation IDs
        private int _animIDRunForward;
        private int _animIDRunBackward;
        private int _animIDJump;
        private int _animIDWalkForward;
        private int _animIDWalkBackward;

        private bool isLive = true;
        [SerializeField] private Transform vfxRed;
        [SerializeField] private float harm = 20.0f;
        [SerializeField] private float InitialHealth = 100.0f;
        private float healthvalue;
        private float bloodInterval = 500.0f;
        private float bloodStatus = 0.0f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            if (playerDetectionResult)
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(eyeTransform.position, visionDistance);  // from eyeTransform.position as center to draw sphere
        }

        private void SelfRecover()
        {
            if (isLive && ReturnHealthValue() < InitialHealth)
            {
                healthvalue += 10.0f;
                // Debug.Log("recover a little bit   :" + healthvalue.ToString());
            }
            bloodStatus = 0.0f;
        }

        public float ReturnHealthValue()
        {
            return healthvalue;
        }

        public void CheckHealth()
        {
            if (healthvalue <= 0.0f)
            {
                SetDie();
            }
            else
            {
                // Debug.Log("health value: " + ReturnHealthValue());
            }

        }

        private void CountBloodInterval()
        {
            // count interval between two health recovery
            if (bloodStatus < bloodInterval)
            {
                bloodStatus += 1;
            }
        }

        public void SetDie()
        {
            _animator.SetBool(_animIDWalkForward, false);
            _animator.SetBool(_animIDWalkBackward, false);
            _animator.SetBool(_animIDRunForward, false);
            _animator.SetBool(_animIDRunBackward, false);

            _animator.SetTrigger("Die");
            //Thread.Sleep(500);
            isLive = false;
            //transform.gameObject.SetActive(false);

        }

        public void DoTheHarm(Vector3 bulletPos)
        {
            _animator.SetTrigger("Take Damage");
            if (ReturnHealthValue() >= 20.0f)
            {
                // show bleeding 
                Instantiate(vfxRed, bulletPos, Quaternion.identity);
            }
            healthvalue -= harm;
        }

        private void AssignAnimationIDs()
        {
            _animIDJump = Animator.StringToHash("Jump");
            _animIDRunForward = Animator.StringToHash("Run Forward");
            _animIDRunBackward = Animator.StringToHash("Run Backward");
            _animIDWalkForward = Animator.StringToHash("Walk Forward");
            _animIDWalkBackward = Animator.StringToHash("Walk Backward");
        }

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            AssignAnimationIDs();
            // for backToIdle function
            original_position = transform.position;
            original_forward = transform.forward;
            agent = GetComponent<NavMeshAgent>();
            healthvalue = InitialHealth;
        }

        private void Update()
        {
            if (isLive && _hasAnimator)
            {
                CheckHealth();
                playerDetectionResult = DetectPlayer();
                if (playerDetectionResult)
                {
                    Vector3 directionToPlayer = playerTransform.position - transform.position;
                    TurnToPlayer(directionToPlayer);

                    if (Physics.CheckSphere(transform.position, chaseDistance, playerLayer)) ChasePlayer();

                    if (Physics.CheckSphere(transform.position, attackDistance, playerLayer)) AttackPlayer();

                }
                else
                {
                    Patroling();
                }
                CountBloodInterval();
                if (bloodStatus == bloodInterval)
                {
                    SelfRecover();
                }
            }

        }

        private void Patroling()
        {
            float walkingPointRange = 2.0f;
            float randomZ = Random.Range(-walkingPointRange, walkingPointRange);
            float randomX = Random.Range(-walkingPointRange, walkingPointRange);
            Vector3 walkPoint = new Vector3(original_position.x + randomX, original_position.y, original_position.z + randomZ);
            if (agent.isActiveAndEnabled)
            {
                agent.SetDestination(walkPoint);
            }
            _animator.SetBool(_animIDWalkForward, true);
        }

        private void TurnToPlayer(Vector3 directionToPlayer)
        {
            // make turn
            float relativeAngle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);
            //Debug.Log("turn angle:" + relativeAngle.ToString());
            _animator.SetBool(_animIDWalkForward, false);
            if (relativeAngle < -10.0f || relativeAngle > 10.0f)
            {
                //transform.rotation = Quaternion.Euler(0.0f, relativeAngle, 0.0f);
                if (relativeAngle < -10.0f)
                {
                    _animator.SetTrigger("Turn Left");
                }
                if (relativeAngle > 10.0f)
                {
                    _animator.SetTrigger("Turn Right");
                }
                transform.LookAt(playerTransform);
            }
        }

        private void ChasePlayer()
        {
            float distanceToPlayer = attackDistance / Mathf.Sqrt(2);
            float randomZ = Random.Range(-distanceToPlayer, distanceToPlayer);
            float randomX = Random.Range(-distanceToPlayer, distanceToPlayer);
            Vector3 SampledDestinationPoint = new Vector3(playerTransform.position.x + randomX, playerTransform.position.y, playerTransform.position.z + randomZ);
            if (agent.isActiveAndEnabled)
            {
                agent.SetDestination(SampledDestinationPoint);
            }
            _animator.SetBool(_animIDRunForward, true);
        }

        private void AttackPlayer()
        {
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDWalkForward, false);
                _animator.SetBool(_animIDRunForward, false);

                _animator.SetTrigger("Smash Attack");
            }
        }


        private bool DetectPlayer()
        {
            Collider[] hitColloders = Physics.OverlapSphere(eyeTransform.position, visionDistance, playerLayer);
            foreach (var collider in hitColloders)
            {
                if (collider.transform.gameObject.name == "Player")
                {
                    //Debug.Log("detect player in range");
                    playerTransform = collider.transform;
                    return true;
                }
            }
            return false;
        }
    }
}
