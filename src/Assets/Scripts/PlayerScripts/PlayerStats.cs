using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] float _playerHealth = 100f;
    [SerializeField] float _meleeDamage = 10f;
    [SerializeField] float _playerMoveSpeed = 1f; 

    public float GetPlayerHealth()
    {
        return _playerHealth;
    }

    public void SetPlayerHealth(float health)
    {
        _playerHealth = health;
    }

    public float GetPlayerMeleeDamage()
    {
        return _meleeDamage;
    }

    public void SetPlayerMeleeDamage(float damage)
    {
        _meleeDamage = damage;
    }

    public float GetPlayerMoveSpeed()
    {
        return _playerMoveSpeed;
    }

    public void SetPlayerMoveSpeed(float speed)
    {
        _playerMoveSpeed = speed;
    }
}
