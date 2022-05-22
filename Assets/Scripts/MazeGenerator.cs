using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator
{
    public int Width = 10;
    public int Height = 10;

    public Maze GenerateMaze()
    {
        MazeGeneratorCell[,] cells = new MazeGeneratorCell[Width, Height];

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                cells[x, y] = new MazeGeneratorCell {X = x, Y = y};
            }
        }

        RemoveWallsWithBacktracker(cells);
        RemoveRoundWalls(cells, Width, Height);
        RemovewWallsRandomly(cells,30);
        PlaceDeathZones(cells, 30);
        cells[8, 8].Finish = true;
        cells[8, 8].DeathZone = false;

        Maze maze = new Maze
        {
            cells = cells,
            finishPosition = new Vector2Int(8, 8)
        };

        return maze;
    }

    private void RemoveRoundWalls(MazeGeneratorCell[,] cells, int Width, int Height)
    {

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            cells[x, Height - 1].WallLeft = false;
            cells[x, Height - 1].Floor = false;
        }

        for (int y = 0; y < cells.GetLength(1); y++)
        {
            cells[Width - 1, y].WallDown = false;
            cells[Width - 1, y].Floor = false;
        }
    }
    private void RemovewWallsRandomly(MazeGeneratorCell[,] maze, int count)
    {
        bool curr = true;
        for (int i = 0; i < count; i++)
        {
            int x = Random(1, 9);
            int y = Random(1, 9);
            if (curr && !maze[x,y].WallLeft)
                maze[x, y].WallDown = false;
            else
                maze[x, y].WallLeft = false;
            curr = !curr;
        }
    }
    private void PlaceDeathZones(MazeGeneratorCell[,] maze, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int x = Random(1, 9);
            int y = Random(1, 9);
            maze[x, y].DeathZone = true;
        }
    }
    private int Random(int x, int y)
    {
        return UnityEngine.Random.Range(x, y);
    }
    private void RemoveWallsWithBacktracker(MazeGeneratorCell[,] maze)
    {
        MazeGeneratorCell current = maze[0, 0];
        current.Visited = true;
        current.DistanceFromStart = 0;

        Stack<MazeGeneratorCell> stack = new Stack<MazeGeneratorCell>();
        do
        {
            List<MazeGeneratorCell> unvisitedNeighbours = new List<MazeGeneratorCell>();

            int x = current.X;
            int y = current.Y;

            if (x > 0 && !maze[x - 1, y].Visited) unvisitedNeighbours.Add(maze[x - 1, y]);
            if (y > 0 && !maze[x, y - 1].Visited) unvisitedNeighbours.Add(maze[x, y - 1]);
            if (x < Width - 2 && !maze[x + 1, y].Visited) unvisitedNeighbours.Add(maze[x + 1, y]);
            if (y < Height - 2 && !maze[x, y + 1].Visited) unvisitedNeighbours.Add(maze[x, y + 1]);

            if (unvisitedNeighbours.Count > 0)
            {
                MazeGeneratorCell chosen = unvisitedNeighbours[Random(0, unvisitedNeighbours.Count)];
                RemoveWall(current, chosen);

                chosen.Visited = true;
                stack.Push(chosen);
                chosen.DistanceFromStart = current.DistanceFromStart + 1;
                current = chosen;
            }
            else
            {
                current = stack.Pop();
            }
        } while (stack.Count > 0);
    }

    private void RemoveWall(MazeGeneratorCell a, MazeGeneratorCell b)
    {
        if (a.X == b.X)
        {
            if (a.Y > b.Y) a.WallDown = false;
            else b.WallDown = false;
        }
        else
        {
            if (a.X > b.X) a.WallLeft = false;
            else b.WallLeft = false;
        }
    }
}