using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] float _enemyHealth = 50f;
    [SerializeField] float _enemyDamage = 10f;

    public float GetEnemyHealth()
    {
        return _enemyHealth;
    }

    public void SetEnemyHealth(float health)
    {
        _enemyHealth = health;
    }

    public float GetEnemyDamage()
    {
        return _enemyDamage;
    }

    public void SetEnemyDamage(float damage)
    {
        _enemyDamage = damage;
    }
}
