using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

//Startup game, load current level, push current level to gameplay controller to create grid and player play zone
//Restrart game after losing/restart, saving current level status to player prefs to be loaded after restarting app

public class GameController : MonoBehaviour
{
    [SerializeField] private Vector2 blockSize = Vector2.one;
    [SerializeField] private GameObject blockPref;
    private void Awake()
    {
        SaveFirstLevelAsJson();
    }

    private void SetUpGrid(Vector2Int gridSize, int[,] blockMap) 
    {

    }

    private void SaveFirstLevelAsJson() 
    {
        List<BlockPlacements> blockPlacements = new List<BlockPlacements>();

        var firstLevelWater = new BlockPlacements();
        firstLevelWater.BlockType = BlockType.Water.ToString();
        firstLevelWater.pinpointPosition.Add("0x0");
        firstLevelWater.pinpointPosition.Add("0x1");
        firstLevelWater.pinpointPosition.Add("2x0");

        blockPlacements.Add(firstLevelWater);

        var firstLevelFire = new BlockPlacements();
        firstLevelFire.BlockType = BlockType.Fire.ToString();
        firstLevelFire.pinpointPosition.Add("2x1");
        firstLevelFire.pinpointPosition.Add("3x0");
        firstLevelFire.pinpointPosition.Add("4x0");

        blockPlacements.Add(firstLevelWater);

        var jsonString = JsonConvert.SerializeObject(blockPlacements, Formatting.Indented);
        Debug.Log(jsonString);

        var objectFromJson = JsonConvert.DeserializeObject<List<BlockPlacements>>(jsonString);
        Debug.Log($"Object from Json data, {objectFromJson[0].BlockType} + {objectFromJson[1].pinpointPosition.Count}");
    }

    [System.Serializable]
    public class BlockPlacements 
    {
        public string BlockType;
        public List<string> pinpointPosition = new List<string>();
    }

    public enum BlockType
    {
        Fire,
        Water,
    }

    public enum SwipeType 
    {
        Left, 
        Right,
        Up,
        Down, 
        Failed
    }
}
