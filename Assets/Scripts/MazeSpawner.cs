using UnityEngine;

public class MazeSpawner : MonoBehaviour
{
    public Cell CellPrefab;
    public Vector3 CellSize = new Vector3(1,1,0);
    public HintRenderer HintRenderer;

    public static Maze maze;

    private void Start()
    {
        Generate(CellPrefab, CellSize);
        HintRenderer.DrawPath();
    }

    public static void Generate(Cell cell, Vector3 vector)
    {
        MazeGenerator generator = new MazeGenerator();
        maze = generator.GenerateMaze();
        Material deathZone = Resources.Load("Materials/DeathZone", typeof(Material)) as Material;
        Material finish = Resources.Load("Materials/Finish", typeof(Material)) as Material;

        for (int x = 0; x < maze.cells.GetLength(0); x++)
        {
            for (int y = 0; y < maze.cells.GetLength(1); y++)
            {
                Cell c = Instantiate(cell, new Vector3(x * vector.x, y * vector.y, y * vector.z), Quaternion.identity);

                c.WallLeft.SetActive(maze.cells[x, y].WallLeft);
                c.WallDown.SetActive(maze.cells[x, y].WallDown);
                c.Floor.SetActive(maze.cells[x, y].Floor);
                if (maze.cells[x, y].DeathZone)
                    c.Floor.GetComponent<Renderer>().material = deathZone;
                if (maze.cells[x, y].Finish)
                    c.Floor.GetComponent<Renderer>().material = finish;
            }
        }
        Player.m = maze;
    }
}