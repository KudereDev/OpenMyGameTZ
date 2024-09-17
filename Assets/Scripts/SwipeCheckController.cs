using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeCheckController : MonoBehaviour
{
    [SerializeField] private Vector2 screenSwipeSesitivity;
    Vector3 startingPosition;
    Vector3 endingPosition;

    private void Awake()
    {
        screenSwipeSesitivity = new Vector2(Screen.width / 4, Screen.height / 4);
    }

    private void Update()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR_WIN
        if (Input.touchCount > 0) 
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) 
            {
                startingPosition = Input.GetTouch(0).position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                endingPosition = Input.GetTouch(0).position;
                var xDelta = endingPosition.x - startingPosition.x;
                var yDelta = endingPosition.y - startingPosition.y;
                var checkSwipeType = PointSwipeType(xDelta, yDelta);

                if (checkSwipeType == SwipeType.Failed) 
                {
                    return;
                }

                GameplayController.Instance.CheckAndMoveBlock(startingPosition, checkSwipeType);
            }
        }
#elif UNITY_EDITOR_WIN
        if (Input.GetMouseButtonDown(0)) 
        {
            startingPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endingPosition = Input.mousePosition;
            var xDelta = endingPosition.x - startingPosition.x;
            var yDelta = endingPosition.y - startingPosition.y;
            var checkSwipeType = PointSwipeType(xDelta, yDelta);

            if (checkSwipeType == SwipeType.Failed) 
            {
                return;
            }

            GameplayController.Instance.CheckAndMoveBlock(startingPosition, checkSwipeType);
        }
#endif
    }

    private SwipeType PointSwipeType(float xDelta, float yDelta)
    {
        if (Mathf.Abs(xDelta) < screenSwipeSesitivity.x && Mathf.Abs(yDelta) < screenSwipeSesitivity.y)
        {
            return SwipeType.Failed;
        }

        if (Mathf.Abs(xDelta) >= Mathf.Abs(yDelta))
        {
            if (xDelta < 0) 
            {
                return SwipeType.Left;
            }
            else
            {
                return SwipeType.Right;
            }

        }
        else 
        {
            if (yDelta < 0)
            {
                return SwipeType.Down;
            }
            else
            {
                return SwipeType.Up;
            }
        }
    }

    public struct SwipeData 
    {
        public Vector3 startMousePosition;
        public SwipeType swipeType;

        public SwipeData(Vector3 position, SwipeType type) 
        {
            this.startMousePosition = position;
            this.swipeType = type;
        }
    }
}

public enum SwipeType
{
    Left,
    Right,
    Up,
    Down,
    Failed
}
