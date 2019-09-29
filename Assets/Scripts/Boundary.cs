using UnityEngine;

struct Boundary 
    {
        public float upBound, downBound, leftBound, rightBound;
        public static float visibleWorldHeight
        {
            get
            {
                return 2 * Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, 0)).y;
            }
        }
        public static float visibleWorldWidth
        {
            get
            {
                return 2 * Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, 0)).x;
            }
        }
        public static Vector2 visibleWorldExtents
        {
            get
            {
                return new Vector2(visibleWorldWidth/2, visibleWorldHeight/2);
            }
        }
    public Boundary(float up, float down, float left, float right)
        {
            upBound = up;
            downBound = down;
            leftBound = left;
            rightBound = right;
        }


    // Summary:
    //  Determines playerMovementBoundaries based on screen size and playerSize
    public static Boundary ScreenBoundary(Vector2 playerSize)
        {
            Boundary screenBoundary = new Boundary();

            int screenWidth = Camera.main.pixelWidth;
            int screenHeight = Camera.main.pixelHeight;
            Vector3 screenToWorldSize = Camera.main.ScreenToWorldPoint
                                        (new Vector3(screenWidth, screenHeight, 0));
            Bounds screenBounds = new Bounds(Vector3.zero, screenToWorldSize * 2);

            screenBoundary.upBound = screenBounds.max.y - playerSize.y;
            screenBoundary.downBound = screenBounds.min.y + playerSize.y;
            screenBoundary.leftBound = screenBounds.min.x + playerSize.x;
            screenBoundary.rightBound = screenBounds.max.x - playerSize.x;

            return screenBoundary;
        }
    }
