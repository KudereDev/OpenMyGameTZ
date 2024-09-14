using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockView : MonoBehaviour
{
    [SerializeField] private Vector2Int blockPosition;
    [SerializeField] private Animator animatorController;
    [SerializeField] private SpriteRenderer spriteRenderer;

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
    }

    public void DestroyBlock() 
    {
        animatorController.SetBool("IsDestroyed", true);
    }
}
