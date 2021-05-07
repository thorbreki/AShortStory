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
        if (focusedEnemyTransform == null) { return; } // If the enemy transform doesnt exist anymore than I'm not doing crap!
        EnemyController enemyController = focusedEnemyTransform.GetComponent<EnemyController>();
        if (enemyController != null) // Paranoid check
        {
            enemyController.DamageMe(damage);
        }
    }

    /// <summary>
    /// When soldier gets unselected
    /// </summary>
    //protected override void OnSelect()
    //{
    //    base.OnSelect();
    //}

    /// <summary>
    /// When soldier gets selected
    /// </summary>
    //protected override void WhenSelected()
    //{
    //    base.WhenSelected();
    //}
}
