using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] GameObject _gun;
    WeaponStat _weaponStat;
    EnemyStats _enemyStat;
    
    float _gunDamage;
    float _enemyHealth;

    void Start()
    {
        _weaponStat = _gun.GetComponent<WeaponStat>();
        _enemyStat = gameObject.GetComponent<EnemyStats>();
        _enemyHealth = _enemyStat.GetEnemyHealth();
        _gunDamage = _weaponStat.GetWeaponDamage();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ammo"))
        {
            Destroy(collision.gameObject);
            _enemyHealth -= _gunDamage;
            if (_enemyHealth <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                _enemyStat.SetEnemyHealth(_enemyHealth);
            }
        }
    }
}
