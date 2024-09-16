using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

//Startup game, load current level, push current level to gameplay controller to create grid and player play zone
//Restrart game after losing/restart, saving current level status to player prefs to be loaded after restarting app

public class GameController : MonoBehaviour
{

    private void Start()
    {
        var testLevel = SaveThirdLevelAsJson();

        GameplayController.Instance.SetUp(new Vector2Int(5, 8), testLevel);
        UIController.Instance.SetupUI(ReloadLevel);
    }

    private void ReloadLevel() 
    {
        var testLevel = SaveSecondLevelAsJson();

        GameplayController.Instance.SetUp(new Vector2Int(5, 8), testLevel);
    }

    private List<BlockPlacements> SaveFirstLevelAsJson() 
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

        blockPlacements.Add(firstLevelFire);

        var jsonString = JsonConvert.SerializeObject(blockPlacements, Formatting.Indented);
        Debug.Log(jsonString);

        var objectFromJson = JsonConvert.DeserializeObject<List<BlockPlacements>>(jsonString);
        Debug.Log($"Object from Json data, {objectFromJson[0].BlockType} + {objectFromJson[1].pinpointPosition.Count}");

        return objectFromJson;
    }

    private List<BlockPlacements> SaveSecondLevelAsJson() 
    {
        List<BlockPlacements> blockPlacements = new List<BlockPlacements>();

        var firstLevelWater = new BlockPlacements();
        firstLevelWater.BlockType = BlockType.Water.ToString();
        firstLevelWater.pinpointPosition.Add("0x0");
        firstLevelWater.pinpointPosition.Add("2x0");
        firstLevelWater.pinpointPosition.Add("3x0");
        firstLevelWater.pinpointPosition.Add("0x1");
        firstLevelWater.pinpointPosition.Add("2x1");
        firstLevelWater.pinpointPosition.Add("3x1");
        firstLevelWater.pinpointPosition.Add("1x2");
        firstLevelWater.pinpointPosition.Add("0x3");
        firstLevelWater.pinpointPosition.Add("2x3");
        firstLevelWater.pinpointPosition.Add("3x3");
        firstLevelWater.pinpointPosition.Add("0x4");
        firstLevelWater.pinpointPosition.Add("1x4");

        blockPlacements.Add(firstLevelWater);

        var firstLevelFire = new BlockPlacements();
        firstLevelFire.BlockType = BlockType.Fire.ToString();
        firstLevelFire.pinpointPosition.Add("1x0");
        firstLevelFire.pinpointPosition.Add("1x1");
        firstLevelFire.pinpointPosition.Add("0x2");
        firstLevelFire.pinpointPosition.Add("2x2");
        firstLevelFire.pinpointPosition.Add("3x2");
        firstLevelFire.pinpointPosition.Add("1x3");

        blockPlacements.Add(firstLevelFire);

        var jsonString = JsonConvert.SerializeObject(blockPlacements, Formatting.Indented);
        Debug.Log(jsonString);

        var objectFromJson = JsonConvert.DeserializeObject<List<BlockPlacements>>(jsonString);
        Debug.Log($"Object from Json data, {objectFromJson[0].BlockType} + {objectFromJson[1].pinpointPosition.Count}");

        return objectFromJson;
    }

    private List<BlockPlacements> SaveThirdLevelAsJson()
    {
        List<BlockPlacements> blockPlacements = new List<BlockPlacements>();

        var firstLevelWater = new BlockPlacements();
        firstLevelWater.BlockType = BlockType.Water.ToString();
        firstLevelWater.pinpointPosition.Add("0x0");
        firstLevelWater.pinpointPosition.Add("2x0");
        firstLevelWater.pinpointPosition.Add("3x0");
        firstLevelWater.pinpointPosition.Add("0x1");
        firstLevelWater.pinpointPosition.Add("2x1");
        firstLevelWater.pinpointPosition.Add("3x1");
        firstLevelWater.pinpointPosition.Add("1x2");
        firstLevelWater.pinpointPosition.Add("0x3");
        firstLevelWater.pinpointPosition.Add("1x3");
        firstLevelWater.pinpointPosition.Add("3x3");
        firstLevelWater.pinpointPosition.Add("0x4");
        firstLevelWater.pinpointPosition.Add("1x5");

        blockPlacements.Add(firstLevelWater);

        var firstLevelFire = new BlockPlacements();
        firstLevelFire.BlockType = BlockType.Fire.ToString();
        firstLevelFire.pinpointPosition.Add("1x0");
        firstLevelFire.pinpointPosition.Add("1x1");
        firstLevelFire.pinpointPosition.Add("0x2");
        firstLevelFire.pinpointPosition.Add("2x2");
        firstLevelFire.pinpointPosition.Add("3x2");
        firstLevelFire.pinpointPosition.Add("1x4");

        blockPlacements.Add(firstLevelFire);

        var jsonString = JsonConvert.SerializeObject(blockPlacements, Formatting.Indented);
        Debug.Log(jsonString);

        var objectFromJson = JsonConvert.DeserializeObject<List<BlockPlacements>>(jsonString);
        Debug.Log($"Object from Json data, {objectFromJson[0].BlockType} + {objectFromJson[1].pinpointPosition.Count}");

        return objectFromJson;
    }

    [System.Serializable]
    public class BlockPlacements 
    {
        public string BlockType;
        public List<string> pinpointPosition = new List<string>();
    }
}
