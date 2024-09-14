using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksLocalDB : ScriptableObject
{

    public class BlockData 
    {
        public string Name;
        public BlockType Type;

        //Add animators and idle block sprite for object
    }    
}

public enum BlockType
{
    Fire,
    Water,
}