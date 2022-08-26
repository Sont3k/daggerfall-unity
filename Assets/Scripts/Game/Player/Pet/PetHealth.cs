using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    public class PetHealth : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DaggerfallEntityBehaviour _daggerfallEntityBehaviour;
        [SerializeField] private Slider _healthSlider;

        private PlayerPetEntity _petEntity;
        private readonly float _minHealth = 0f;

        private void Start()
        {
            _petEntity = _daggerfallEntityBehaviour.Entity as PlayerPetEntity;
            InitHealthBar();

            IncreaseHealth(50); // for testing purposes
            DecreaseHealth(10);
        }

        private void InitHealthBar()
        {
            _healthSlider.minValue = _minHealth;
            _healthSlider.value = _petEntity.CurrentHealth;
            _healthSlider.maxValue = _petEntity.MaxHealth;
        }

        private void UpdateHealthBar()
        {
            _healthSlider.value = _petEntity.CurrentHealth;
        }

        // Not used for now but can be used in future to increase health with UI update
        public void IncreaseHealth(int amount)
        {
            _petEntity.IncreaseHealth(amount);
            UpdateHealthBar();
        }

        // Not used for now but can be used in future to decrease health with UI update
        public void DecreaseHealth(int amount)
        {
            _petEntity.DecreaseHealth(amount);
            UpdateHealthBar();
        }
    }
}