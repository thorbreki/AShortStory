using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnBuildingClick(Vector3 buildingPosition);
    public static event OnBuildingClick onBuildingClick; // When a building is left-clicked by the Player

    public delegate void OnBarrackClick();
    public static event OnBarrackClick onBarrackClick; // When a barrack is left-clicked by the Player

    public delegate void OnSmithyClick();
    public static event OnSmithyClick onSmithyClick; // When a barrack is left-clicked by the Player

    public delegate void OnSelect();
    public static event OnSelect onSelect; // When the Player selects/left-clicks any object in the scene

    public delegate void OnMove(float targetX);
    public static event OnMove onMove; // When the player right-clicks anywhere on the screen, to tell all selected soldiers to move

    public delegate void OnAttack(Transform targetTransform);
    public static event OnAttack onAttack; // When the player tells selected soldiers to attack

    public delegate void OnConstructMe(Transform buildingTransform, BuildingController buildingController);
    public static event OnConstructMe onConstructMe; // When the player tells selected builders to build

    public delegate void OnPlayerModeChanged(Constants.PlayerMode newPlayerMode);
    public static event OnPlayerModeChanged onPlayerModeChanged; // When the game changes from one player mode to another

    public delegate void OnEnemyDamagedByPlayer();
    public static event OnEnemyDamagedByPlayer onEnemyDamagedByPlayer; // When the player damages an enemy directly

    public delegate void OnOreClicked(Transform oreTransform, OreController oreController);
    public static event OnOreClicked onOreClicked; // When the player left-clicks on an ore

    public delegate void OnFindNearestSmithy(OreController oreController); // When the builder calls out for the nearest Smithy
    public static event OnFindNearestSmithy onFindNearestSmithy;

    public delegate void OnPlacingBuildingCancel(); // When the Player cancels wanting to place a new building
    public static event OnPlacingBuildingCancel onPlacingBuildingCancel;

    //public delegate void OnBuilderSelected();
    //public static event OnBuilderSelected onBuilderSelected; // When the player selects a builder

    /// <summary>
    /// Should be raised when a building is left-clicked by the Player
    /// </summary>
    /// <param name="buildingPosition"></param>
    public static void RaiseOnBuildingClick(Vector3 buildingPosition)
    {
        if (onBuildingClick != null)
        {
            onBuildingClick(buildingPosition);
        }
    }

    /// <summary>This event should be raised when the Player clicks on a Barrack</summary>
    public static void RaiseOnBarrackClick()
    {
        if (onBarrackClick != null)
        {
            onBarrackClick();
        }
    }

    /// <summary>
    /// Raises the OnSmithyClick event, should be called when player left-clicks on a Smithy
    /// </summary>
    /// <param name="smithyPosition">The position of the left-clicked Smithy</param>
    public static void RaiseOnSmithyClick()
    {
        if (onSmithyClick != null)
        {
            onSmithyClick();
        }
    }

    /// <summary>This event should be raised when the Player selects anything whatsoever</summary>
    public static void RaiseOnSelected()
    {
        if (onSelect != null)
        {
            onSelect();
        }
    }

    public static void RaiseOnMove(float targetX)
    {
        if (onMove != null)
        {
            onMove(targetX);
        }
    }

    public static void RaiseOnAttack(Transform targetTransform)
    {
        if (onAttack != null)
        {
            onAttack(targetTransform);
        }
    }

    public static void RaiseOnConstructMe(Transform buildingTransform, BuildingController buildingController)
    {
        if (onConstructMe != null)
        {
            onConstructMe(buildingTransform, buildingController);
        }
    }

    public static void RaiseOnPlayerModeChanged(Constants.PlayerMode newPlayerMode)
    {
        if (onPlayerModeChanged != null)
        {
            onPlayerModeChanged(newPlayerMode);
        }
    }

    public static void RaiseOnEnemyDamagedByPlayer()
    {
        if (onEnemyDamagedByPlayer != null)
        {
            onEnemyDamagedByPlayer();
        }
    }

    public static void RaiseOnOreClicked(Transform oreTransform, OreController oreController)
    {
        if (onOreClicked != null)
        {
            onOreClicked(oreTransform, oreController);
        }
    }

    public static void RaiseOnFindNearestSmithy(OreController oreController)
    {
        if (onFindNearestSmithy != null)
        {
            onFindNearestSmithy(oreController);
        }
    }

    public void RaiseOnPlacingBuildingCancel()
    {
        if (onPlacingBuildingCancel != null)
        {
            onPlacingBuildingCancel();
        }
    }

    //public static void RaiseOnBuilderSelected()
    //{
    //    if (onBuilderSelected != null)
    //    {
    //        onBuilderSelected();
    //    }
    //}
}
