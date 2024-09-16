using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;
using static BlocksLocalDB;
using System;

public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance;

    [SerializeField] private Vector2 blockSize = Vector2.one;
    [SerializeField] private BlockView blockPref;
    [SerializeField] private Transform blockHolder;
    private int[,] gridMap;
    [SerializeField] private BlocksLocalDB dataBase;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector2Int gridSize;
    private bool lockSwipes = false;
    private Dictionary<BlockType, BlockData> loadedBlocks = new Dictionary<BlockType, BlockData>();
    private Dictionary<Vector2Int, BlockView> simulatedGrid = new Dictionary<Vector2Int, BlockView>();

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
        this.gridSize = gridSize; 
    }

    public void CheckAndMoveBlock(Vector3 mousePosition, SwipeType swipeType) 
    {
        if (lockSwipes) 
        {
            return;
        }

        var target = GetValidTarget(mousePosition);
        if (target is null) 
        {
            return;
        }

        var blockPosition = target.GetBlockPosition();
        if (!IsSwipeValid(blockPosition, swipeType)) 
        {
            return;
        }

        Vector2Int targetPosition = Vector2Int.zero;

        switch (swipeType) 
        {
            case SwipeType.Left:
                targetPosition = new Vector2Int(blockPosition.x - 1, blockPosition.y);
                break;

            case SwipeType.Right:
                targetPosition = new Vector2Int(blockPosition.x + 1, blockPosition.y);
                break;

            case SwipeType.Down:
                targetPosition = new Vector2Int(blockPosition.x , blockPosition.y - 1);
                break;

            case SwipeType.Up:
                targetPosition = new Vector2Int(blockPosition.x, blockPosition.y + 1);
                break;
        }

        MoveBlock(target, targetPosition, blockPosition);
        bool isPhysicsSimulated = SimulatePhysics();
        if (!isPhysicsSimulated) 
        {
            NormalizeGrid();
        }
    }

    private void ClearGrid() 
    {
        int childCount = blockHolder.childCount;
        for (int i = 0; i < childCount; i++) 
        {
            Destroy(blockHolder.GetChild(i).gameObject);
        }
        simulatedGrid.Clear();
    }

    private void SetUpGrid(Vector2Int gridSize, List<BlockPlacements> placements)
    {
        Debug.Log($"Block placements count:{placements.Count}");
        gridMap = ConvertBlockPlacementToIdGrid(gridSize, placements);
        
        for (int y = 0; y < gridSize.y; y++) 
        {
            for (int x = 0; x < gridSize.x; x++) 
            {
                if (gridMap[x, y] == 0) 
                {
                    continue;
                }

                var blockPosition = new Vector3(blockHolder.position.x + blockSize.x * x, blockHolder.position.x + blockSize.y * y, blockHolder.position.z);
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

                simulatedGrid.Add(new Vector2Int(x, y), block);
                block.UpdateBlockPosition(new Vector2Int(x, y));
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

    private BlockView GetValidTarget(Vector3 mousePosition) 
    {
        var worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Target: " + hit.collider.gameObject.name);

            return hit.collider.gameObject.GetComponent<BlockView>();
        }

        return null;
    }

    private bool IsSwipeValid(Vector2Int position, SwipeType type) 
    {
        switch (type) 
        {
            case SwipeType.Up:
                if (position.y == gridSize.y - 1) 
                {
                    return false;
                } 
                    
                if (gridMap[position.x, position.y + 1] == 0) 
                {
                    return false;
                }
                break;
            case SwipeType.Down:
                if (position.y == 0)
                {
                    return false;
                }
                break;
            case SwipeType.Left:
                if (position.x == 0)
                {
                    return false;
                }
                break;
            case SwipeType.Right:
                if (position.x == gridSize.x - 1)
                {
                    return false;
                }
                break;
        }
        return true;
    }

    private void MoveBlock(BlockView targetBlock, Vector2Int targetPosition, Vector2Int prevPosition) 
    {
        BlockView swapBlock = null;
        if (simulatedGrid.ContainsKey(targetPosition))
        {
            swapBlock = simulatedGrid[targetPosition];
        }

        var tempVariable = gridMap[targetPosition.x, targetPosition.y];
        gridMap[targetPosition.x, targetPosition.y] = gridMap[prevPosition.x, prevPosition.y];
        gridMap[prevPosition.x, prevPosition.y] = tempVariable;

        targetBlock.UpdateBlockPosition(targetPosition);
        targetBlock.transform.position = new Vector3(blockHolder.position.x + blockSize.x * targetPosition.x,
            blockHolder.position.x + blockSize.y * targetPosition.y, blockHolder.position.z);
        targetBlock.name = $"{(BlockType)gridMap[targetPosition.x, targetPosition.y]}_Block[{targetPosition.x},{targetPosition.y}]";
        Debug.Log($"Change target block position to:{targetPosition}");

        if (simulatedGrid.ContainsKey(targetPosition))
        {
            simulatedGrid[targetPosition] = targetBlock;
        }
        else
        {
            simulatedGrid.Add(targetPosition, targetBlock);
        }

        if (swapBlock != null)
        {
            swapBlock.UpdateBlockPosition(prevPosition);
            swapBlock.transform.position = new Vector3(blockHolder.position.x + blockSize.x * prevPosition.x,
                blockHolder.position.x + blockSize.y * prevPosition.y, blockHolder.position.z);

            if (simulatedGrid.ContainsKey(prevPosition))
            {
                simulatedGrid[prevPosition] = swapBlock;
            }
            else 
            {
                simulatedGrid.Add(prevPosition, swapBlock);
            }
            
            swapBlock.name = $"{(BlockType)gridMap[prevPosition.x, prevPosition.y]}_Block[{prevPosition.x},{prevPosition.y}]";
        }
        else
        {
            simulatedGrid.Remove(prevPosition);
        }
    }

    private bool SimulatePhysics()
    {
        List<Vector2Int> blocksForSimulation = new List<Vector2Int>();
        for (int y = 1; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if (gridMap[x, y] == 0)
                {
                    continue;
                }

                var targetBlockPosition = new Vector2Int(x, y - 1);
                if (gridMap[targetBlockPosition.x, targetBlockPosition.y] != 0)
                {
                    continue;
                }

                blocksForSimulation.Add(new Vector2Int(x, y));
                for (int i = 0; i < gridSize.y; i++) 
                {
                    if (y + i >= gridSize.y) 
                    {
                        break;
                    }

                    if (gridMap[x, y + i] != 0) 
                    {
                        blocksForSimulation.Add(new Vector2Int(x, y + i));
                    }
                }
            }
        }

        if (blocksForSimulation.Count == 0) 
        {
            lockSwipes = false;
            CheckWinState();
            return false;
        }

        for (int i = 0; i < blocksForSimulation.Count; i++) 
        {
            int maxAmountOfSteps = 0;
            for (int checkBlockHight = blocksForSimulation[i].y - 1; checkBlockHight >= 0; checkBlockHight--) 
            {
                if (gridMap[blocksForSimulation[i].x, checkBlockHight] == 0)
                {
                    maxAmountOfSteps++;
                }
                else 
                {
                    break;
                }
            }

            var newBlockPosition = new Vector2Int(blocksForSimulation[i].x, blocksForSimulation[i].y - maxAmountOfSteps);
            if (simulatedGrid.ContainsKey(blocksForSimulation[i])) 
            {
                MoveBlock(simulatedGrid[blocksForSimulation[i]], newBlockPosition, blocksForSimulation[i]);
            }
        }
        NormalizeGrid();
        return true;
    }

    private void NormalizeGrid() 
    {
        lockSwipes = true;
        //Check each row and column for 3+ block in row, count what blocks are in row and mark them for destruction 
        //After all grid check destroy all blocks and run Physics simulator 
        List<Vector2Int> destroyList = new List<Vector2Int>();


        //Horisontal check
        for (int y = 0; y < gridSize.y; y++) 
        {
            int checkBlockType = 0;
            int blockInRowNumber = 0;
            List<Vector2Int> tempRowCheck = new List<Vector2Int>();
            for (int x = 0; x < gridSize.x; x++)
            {
                if (gridMap[x, y] == 0)
                {
                    checkBlockType = 0;
                    blockInRowNumber = 0;
                    continue;
                }

                if (gridMap[x, y] != 0 && gridMap[x,y] != checkBlockType) 
                {
                    if (checkBlockType == 0)
                    {
                        checkBlockType = gridMap[x, y];
                    }
                    else 
                    {
                        checkBlockType = gridMap[x, y];
                        blockInRowNumber = 0;
                    }
                }

                if (gridMap[x, y] == checkBlockType && checkBlockType != 0)
                {
                    blockInRowNumber++;
                    //Debug.Log($"Horisontal Check type:{checkBlockType}, blocksInRow:{blockInRowNumber}, full info: [Check Row/Pos:{x},{y}], [GridIdData:{gridMap[x,y]}]");
                    if (blockInRowNumber >= 3) 
                    {
                        for (int i = 0; i < blockInRowNumber; i++)
                        {
                            if (!tempRowCheck.Contains(new Vector2Int(x - i, y)))
                                tempRowCheck.Add(new Vector2Int(x - i, y));
                        }
                    }
                }
            }

            destroyList.AddRange(tempRowCheck);
            
            string debugResult = "";
            foreach (var block in destroyList) 
            {
                debugResult += block.ToString();
            }
            //Debug.Log($"Number block to destroy: {destroyList.Count}, {debugResult}");
        }

        //Vertical Check
        for (int x = 0; x < gridSize.x; x++)
        {
            int checkBlockType = 0;
            int blockInRowNumber = 0;
            List<Vector2Int> tempRowCheck = new List<Vector2Int>();
            for (int y = 0; y < gridSize.y; y++)
            {
                if (gridMap[x, y] == 0)
                {
                    checkBlockType = 0;
                    blockInRowNumber = 0;
                    continue;
                }

                if (gridMap[x, y] != 0 && gridMap[x, y] != checkBlockType)
                {
                    if (checkBlockType == 0)
                    {
                        checkBlockType = gridMap[x, y];
                    }
                    else
                    {
                        checkBlockType = gridMap[x, y];
                        blockInRowNumber = 0;
                    }
                }

                if (gridMap[x, y] == checkBlockType && checkBlockType != 0)
                {
                    blockInRowNumber++;
                    Debug.Log($"Vertical Check type:{checkBlockType}, blocksInRow:{blockInRowNumber}, full info: [Check Row/Pos:{x},{y}], [GridIdData:{gridMap[x, y]}]");
                    if (blockInRowNumber >= 3)
                    {
                        for (int i = 0; i < blockInRowNumber; i++)
                        {
                            if (!tempRowCheck.Contains(new Vector2Int(x, y - i)))
                                tempRowCheck.Add(new Vector2Int(x, y - i));
                        }
                    }
                }
            }

            destroyList.AddRange(tempRowCheck);

            string debugResult = "";
            foreach (var block in destroyList)
            {
                debugResult += block.ToString();
            }
            Debug.Log($"Number block to destroy: {destroyList.Count}, {debugResult}");
        }

        if (destroyList.Count > 0) 
        {
            var blockPosition = destroyList[0];
            DestroyBlock(blockPosition, delegate ()
            {
                SimulatePhysics();
            });

            for (int i = 1; i < destroyList.Count; i++)
            {
                blockPosition = destroyList[i];
                DestroyBlock(blockPosition);
            }
        }

        lockSwipes = false;
        //SimulatePhysics();
    }

    private void DestroyBlock(Vector2Int blockPosition, Action onDestroy = null) 
    {
        gridMap[blockPosition.x, blockPosition.y] = 0;
        if (simulatedGrid.ContainsKey(blockPosition)) 
        {
            var block = simulatedGrid[blockPosition];
            block.DestroyBlock(onDestroy);
            simulatedGrid.Remove(blockPosition);
        }
    }

    private void CheckWinState()
    {
        if (simulatedGrid.Count == 0)
        {
            UIController.Instance.ShowWinScreen();
        }
    }
 }
