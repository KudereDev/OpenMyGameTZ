using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockDB", menuName = "LocalDB", order = 1)]
public class BlocksLocalDB : ScriptableObject
{
    public List<BlockData> blocks = new List<BlockData>();

    [System.Serializable]
    public class BlockData 
    {
        public string Name;
        public BlockType Type;

        //Add animators and idle block sprite for object
        public RuntimeAnimatorController Animator;
        public Sprite BlockSprite;
    }    
}

public enum BlockType
{
    Fire = 1,
    Water,
}