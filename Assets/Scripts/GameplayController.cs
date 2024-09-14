using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static GameController;
using static BlocksLocalDB;

//Simulate gameplay zone, check Touch/Drag imput from player 
//Force block to go for player swipe type with fail safe for touch without dragrging and dragging for bad direction 
//On swipe end and command block to do thing, do specific stuff 

// - On block swapping, swap block and check for 3rd in row 
// - On block swap to empty area make block drop by animation to lowest point in grin for X,Y of block 
// - On all blocks destroyed flag level as win. 
public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance;

    [SerializeField] private Vector2 blockSize = Vector2.one;
    [SerializeField] private BlockView blockPref;
    [SerializeField] private Transform blockHolder;
    private int[,] gridMap;
    [SerializeField] private BlocksLocalDB dataBase;
    private Dictionary<BlockType, BlockData> loadedBlocks = new Dictionary<BlockType, BlockData>();

    public enum SwipeType
    {
        Left,
        Right,
        Up,
        Down,
        Failed
    }

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    public void SetUp(Vector2Int gridSize, List<BlockPlacements> placements) 
    {
        ClearGrid();
        SetUpGrid(gridSize, placements);
    }

    private void ClearGrid() 
    {
        int childCount = blockHolder.childCount;
        for (int i = 0; i < childCount; i++) 
        {
            Destroy(blockHolder.GetChild(i).gameObject);
        }
    }

    private void SetUpGrid(Vector2Int gridSize, List<BlockPlacements> placements)
    {
        Debug.Log($"Block placements count:{placements.Count}");
        var gridMap = ConvertBlockPlacementToIdGrid(gridSize, placements);
        
        for (int y = 0; y < gridSize.y; y++) 
        {
            for (int x = 0; x < gridSize.x; x++) 
            {
                if (gridMap[x, y] == 0) 
                {
                    continue;
                }

                var blockPosition = new Vector3(blockHolder.position.x + blockSize.x * x, blockHolder.position.x + blockSize.y * y, blockHolder.position.z) ;
                var block = Instantiate(blockPref, blockHolder);
                block.transform.position = blockPosition;


                var currentBlockType = (BlockType) gridMap[x, y];
                if (!loadedBlocks.ContainsKey(currentBlockType)) 
                {
                    var dbData = dataBase.blocks.Find(d => d.Type == currentBlockType);
                    loadedBlocks.Add(currentBlockType, dbData);
                }

                var blockData = loadedBlocks[currentBlockType];
                block.SetupBlock(blockData.BlockSprite, x + y, gridMap[x, y], blockData.Animator);
                block.name = $"{(BlockType)gridMap[x, y]}_Block[{x},{y}]";
            }
        }
    }

    private int[,] ConvertBlockPlacementToIdGrid(Vector2Int gridSize, List<BlockPlacements> placements) 
    {
        int[,] idGrid = new int[gridSize.x, gridSize.y];

        for (int i = 0; i < placements.Count; i++) 
        {
            if (placements[i].pinpointPosition.Count == 0) 
            {
                Debug.LogError($"No pinpoint positions for blocks in BlockType:{placements[i].BlockType} placements");
                continue;
            }

            BlockType parsed_enum = (BlockType)System.Enum.Parse(typeof(BlockType), placements[i].BlockType);
            Debug.Log($"Check block type:{parsed_enum} ");

            for(int j = 0; j < placements[i].pinpointPosition.Count; j++) 
            {
                var position = ParseJsonIntoGridPostion(placements[i].pinpointPosition[j]);
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
