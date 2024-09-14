using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GameController;

//Simulate gameplay zone, check Touch/Drag imput from player 
//Force block to go for player swipe type with fail safe for touch without dragrging and dragging for bad direction 
//On swipe end and command block to do thing, do specific stuff 

// - On block swapping, swap block and check for 3rd in row 
// - On block swap to empty area make block drop by animation to lowest point in grin for X,Y of block 
// - On all blocks destroyed flag level as win. 
public class GameplayController : MonoBehaviour
{
    [SerializeField] private Vector2 blockSize = Vector2.one;
    [SerializeField] private GameObject blockPref;
    private int[,] gridMap;

    public enum SwipeType
    {
        Left,
        Right,
        Up,
        Down,
        Failed
    }

    private void SetUpGrid(Vector2Int gridSize, List<BlockPlacements> placements)
    {
        var gridMap = ConvertBlockPlacementToIdGrid(gridSize, placements);
        for (int y = 0; y < gridSize.y; y++) 
        {

        }
    }

    private int[,] ConvertBlockPlacementToIdGrid(Vector2Int gridSize, List<BlockPlacements> placements) 
    {
        int[,] idGrid = new int[gridSize.x, gridSize.y];

        foreach (var placement in placements) 
        {
            if (placement.pinpointPosition.Count == 0) 
            {
                Debug.LogError($"No pinpoint positions for blocks in BlockType:{placement.BlockType} placements");
                continue;
            }

            BlockType parsed_enum = (BlockType)System.Enum.Parse(typeof(BlockType), placement.BlockType);

            for(int i = 0; i < placement.pinpointPosition.Count; i++) 
            {
                var position = ParseJsonIntoGridPostion(placement.pinpointPosition[i]);
                idGrid[position.x, position.y] = (int)parsed_enum;
            }
        }
        return idGrid;
    }

    private Vector2Int ParseJsonIntoGridPostion(string pinpointPosition) 
    {
        var coordinates = pinpointPosition.Split('x');
        Vector2Int gridPosition = new Vector2Int(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
        return gridPosition;
    }
 }
