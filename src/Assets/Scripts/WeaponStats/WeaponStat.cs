using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStat : MonoBehaviour
{
    [SerializeField] float _ammoSpeed = 10f;  // Speed of the ammo
    [SerializeField] float _fireRate = 1.0f; // Time in seconds between each shot
    [SerializeField] float _weaponDamage = 10f;

    // Getter and setter
    public float GetAmmoSpeed()
    {
        return _ammoSpeed;
    }

    public float GetWeaponFireRate()
    {
        return _fireRate;
    }

    public float GetWeaponDamage()
    {
        return _weaponDamage;
    }

    public void SetAmmoSpeed(float newSpeed)
    {
        _ammoSpeed = newSpeed;
    }

    public void SetWeaponFireRate(float newRate)
    {
        _fireRate = newRate;
    }

    public void SetWeaponDamage(float newWeaponDamage)
    {
        _weaponDamage = newWeaponDamage;
    }
}
