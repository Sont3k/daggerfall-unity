using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
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

        [Header("Potion Spawn")]
        [SerializeField] private float _potionDropHealthThreshold = 0.25f;
        [SerializeField] private float _potionDropCooldownInitTime = 60f; // in seconds

        private GameObject _playerObject;
        private PlayerEntity _playerEntity;

        private readonly int _potionRecipeIndex = 15;

        private float _potionDropCooldownTimer;
        private bool _isPotionCooldownActive;

        private void Start()
        {
            _playerEntity = GameManager.Instance.PlayerEntity;
            _playerObject = GameManager.Instance.PlayerObject;
        }

        private void Update()
        {
            LookAtPlayer();
            FollowPlayer();
            DropLootWithPotionForPlayer();
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
            else if (distanceToPlayer > _followDistance)
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

        private void DropLootWithPotionForPlayer()
        {
            var isThresholdWasReached =
                _playerEntity.CurrentHealth < _playerEntity.MaxHealth * _potionDropHealthThreshold;
            if (HandlePotionDropCooldown() || !isThresholdWasReached) return;

            SpawnLootWithPotion();
            ActivatePotionDropCooldown();
        }

        private bool HandlePotionDropCooldown()
        {
            if (!_isPotionCooldownActive) return false;

            _potionDropCooldownTimer -= Time.deltaTime;

            if (_potionDropCooldownTimer <= 0)
            {
                _isPotionCooldownActive = false;
            }

            return true;
        }

        private void ActivatePotionDropCooldown()
        {
            _potionDropCooldownTimer = _potionDropCooldownInitTime;
            _isPotionCooldownActive = true;
        }

        private void SpawnLootWithPotion()
        {
            var lootContainer = SpawnLootContainer();
            var recipeKeys = GameManager.Instance.EntityEffectBroker.GetPotionRecipeKeys();
            lootContainer.Items.AddItem(ItemBuilder.CreatePotion(recipeKeys[_potionRecipeIndex]));
        }

        private DaggerfallLoot SpawnLootContainer()
        {
            var iconIndex = Random.Range(0, DaggerfallLootDataTables.randomTreasureIconIndices.Length);
            var iconRecord = DaggerfallLootDataTables.randomTreasureIconIndices[iconIndex];

            var lootContainer = GameObjectHelper.CreateLootContainer(
                LootContainerTypes.RandomTreasure,
                InventoryContainerImages.Chest,
                FindGroundPosition(),
                null,
                DaggerfallLootDataTables.randomTreasureArchive,
                iconRecord);

            return lootContainer;
        }

        private Vector3 FindGroundPosition(float distance = 10)
        {
            var ray = new Ray(transform.position, Vector3.down);
            return Physics.Raycast(ray, out var hit, distance) ? hit.point : transform.position;
        }
    }
}