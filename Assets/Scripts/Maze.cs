using UnityEngine;

public class Maze
{
    public MazeGeneratorCell[,] cells;
    public Vector2Int finishPosition;
}

public class MazeGeneratorCell
{
    public int X;
    public int Y;

    public bool WallLeft = true;
    public bool WallDown = true;
    public bool Floor = true;
    public bool DeathZone = false;
    public bool Finish = false;

    public bool Visited = false;
    public int DistanceFromStart;
}
