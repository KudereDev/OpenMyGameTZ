using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockView : MonoBehaviour
{
    [SerializeField] private Vector2Int blockPosition;
    [SerializeField] private Animator animatorController;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private event Action OnBlockDestroy = null;

    public void SetupBlock(Sprite startingSprite, int orderInLayer, int blockType, RuntimeAnimatorController blockAnimations)
    {
        spriteRenderer.sprite = startingSprite;
        spriteRenderer.sortingOrder = orderInLayer;
        animatorController.runtimeAnimatorController = blockAnimations;
        animatorController.SetInteger("Type", blockType);
    }

    public void UpdateBlockPosition(Vector2Int position) 
    {
        blockPosition = position;
        spriteRenderer.sortingOrder = position.x + position.y;
    }

    public void DestroyBlock(Action onBlockDestroy = null) 
    {
        animatorController.SetBool("IsDestroyed", true);
        OnBlockDestroy += onBlockDestroy;
    }

    public void DisableBlock() 
    {
        animatorController.enabled = false;
        OnBlockDestroy?.Invoke();
        Destroy(gameObject);
    }

    public Vector2Int GetBlockPosition() 
    {
        return blockPosition;
    }
}
