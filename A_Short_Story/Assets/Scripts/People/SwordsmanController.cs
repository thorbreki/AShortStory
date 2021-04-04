using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanController : SoldierController
{
    [Header("Swordsman specific stuff")]
    [SerializeField] private float damage; // How much damage I do


    /// <summary>
    /// Damage the enemy and potentially kill them, this method is called as an event in the swordsman's attack animation
    /// </summary>
    public override void DamageEnemy()
    {
        EnemyController enemyController = focusedEnemyTransform.GetComponent<EnemyController>();
        if (enemyController != null) // Paranoid check
        {
            enemyController.DamageMe(damage);
        }
    }
}
