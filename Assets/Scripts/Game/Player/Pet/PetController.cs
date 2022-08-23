using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class PetController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _characterController;

        [Header("Parameters")]
        [SerializeField] private float _followDistance = 5f;
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _teleportDistance = 50f;

        private GameObject _playerObject;

        private void Awake()
        {
            _playerObject = GameManager.Instance.PlayerObject;
        }

        private void Update()
        {
            FollowPlayer();
            LookAtPlayer();
        }

        private void LookAtPlayer()
        {
            transform.LookAt(_playerObject.transform);
        }

        private void FollowPlayer()
        {
            var playerPosition = _playerObject.transform.position;
            var distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

            if (distanceToPlayer > _teleportDistance)
            {
                TeleportToPlayer(playerPosition);
            }
            else if(distanceToPlayer > _followDistance)
            {
                SmoothPlayerFollow(playerPosition);
            }
        }

        private void SmoothPlayerFollow(Vector3 playerPosition)
        {
            var motion = (playerPosition - transform.position).normalized * (_movementSpeed * Time.deltaTime);
            _characterController.Move(motion);
        }

        private void TeleportToPlayer(Vector3 playerPosition)
        {
            transform.position = playerPosition;
        }
    }
}