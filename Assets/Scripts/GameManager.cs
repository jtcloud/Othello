using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.AssetImporters;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance { get; private set; }
    public Chess.State[,] ChessBoard = new Chess.State[5, 5];

    bool _isBlackPlayersTurn = true;

    public int ChessBoardVer { get; private set; }

    private int BoardSize = UIManager.BoardSize;

    private bool blackNoHands;
    private bool whiteNoHands;

    int _availableCellCount = 0;
    public int _unavailableTimes = 0;

    public int BlackScore { get; private set; }
    public int WhiteScore { get; private set; }

    public bool GameOver { get; private set; }

    private void Awake()
    {
        Instance = this;

    }
    void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                ChessBoard[x, y] = Chess.State.Unavailable;

            }

        }
        ChessBoard[1, 1] = Chess.State.White;
        ChessBoard[1, 2] = Chess.State.Black;
        ChessBoard[2, 1] = Chess.State.Black;
        ChessBoard[2, 2] = Chess.State.White;

        // _isBlackPlayersTurn = true;
        _isBlackPlayersTurn = false;  // UpdateChessBoardState will flip the order for 1st hand

        BlackScore = 0;
        WhiteScore = 0;

        _availableCellCount = 0;
        _unavailableTimes = 0;

        GameOver = false;

        ChessBoardVer = 0;

        UpdateChessBoardState();

        // _isBlackPlayersTurn = !_isBlackPlayersTurn;
    }

    void SetState(Vector2Int coordinate, bool isBlack)
    {
        if (ChessBoard[coordinate.x, coordinate.y] == Chess.State.Black || ChessBoard[coordinate.x, coordinate.y] == Chess.State.White)
        {
            return;
        }

        if (
            JudgeDirection(coordinate, new Vector2Int(1, 0), isBlack) ||
            JudgeDirection(coordinate, new Vector2Int(1, 1), isBlack) ||
            JudgeDirection(coordinate, new Vector2Int(1, -1), isBlack) ||
            JudgeDirection(coordinate, new Vector2Int(0, 1), isBlack) ||
            JudgeDirection(coordinate, new Vector2Int(0, -1), isBlack) ||
            JudgeDirection(coordinate, new Vector2Int(-1, 0), isBlack) ||
            JudgeDirection(coordinate, new Vector2Int(-1, 1), isBlack) ||
            JudgeDirection(coordinate, new Vector2Int(-1, -1), isBlack))
        {
            ChessBoard[coordinate.x, coordinate.y] = Chess.State.Available;
        }

    }

    bool JudgeDirection(Vector2Int coordinate, Vector2Int direction, bool isBlack)
    {
        Vector2Int neighborPos = coordinate + direction;

        if (neighborPos.x < 0 || neighborPos.y < 0 || neighborPos.x >= BoardSize || neighborPos.y >= BoardSize)
        {
            return false;
        }

        if (isBlack && ChessBoard[neighborPos.x, neighborPos.y] != Chess.State.White ||
            !isBlack && ChessBoard[neighborPos.x, neighborPos.y] != Chess.State.Black)
        {
            return false;
        }

        for (Vector2Int currentPos = neighborPos; currentPos.x >= 0 && currentPos.y >= 0 && currentPos.x < BoardSize && currentPos.y < BoardSize; currentPos += direction)
        {
            if (ChessBoard[currentPos.x, currentPos.y] == Chess.State.Available ||
                ChessBoard[currentPos.x, currentPos.y] == Chess.State.Unavailable
            )
            {
                return false;
            }

            if (isBlack && ChessBoard[currentPos.x, currentPos.y] == Chess.State.Black ||
                !isBlack && ChessBoard[currentPos.x, currentPos.y] == Chess.State.White
            )
            {
                return true;
            }
        }

        return false;

    }



    void UpdateChessBoardState()
    {
        _isBlackPlayersTurn = !_isBlackPlayersTurn;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (ChessBoard[x, y] == Chess.State.Available)
                {
                    ChessBoard[x, y] = Chess.State.Unavailable;
                }
            }
        }

        BlackScore = 0;
        WhiteScore = 0;
        _availableCellCount = 0;

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                SetState(new Vector2Int(x, y), _isBlackPlayersTurn);

                if (ChessBoard[x, y] == Chess.State.Available)
                {
                    _availableCellCount++;
                }

                if (ChessBoard[x, y] == Chess.State.Black)
                {
                    BlackScore++;
                }
                if (ChessBoard[x, y] == Chess.State.White)
                {
                    WhiteScore++;
                }
            }
        }

        Debug.Log("availableCount: " + _availableCellCount);

        if (_availableCellCount == 0)
        {
            _unavailableTimes++;
            Debug.Log("_unavailableTimes++ to: " + _unavailableTimes);

        }

        if (_unavailableTimes == 1)
        {
            UpdateChessBoardState();
            if (GameOver == true){
                return;
            }
        }

        if (_unavailableTimes == 2)
        {
            Debug.Log("Game Over! should see the message box.");
            GameOver = true;
            ChessBoardVer++;
            return;
        }

        ChessBoardVer++;
    }

    public void CellClicked(Vector2Int coordinate)
    {
        List<Vector2Int> directionCandidates = new List<Vector2Int>(){
            new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(0,1), new Vector2Int(-1,1),
            new Vector2Int(-1,0), new Vector2Int(-1,-1), new Vector2Int(0,-1), new Vector2Int(1,-1)
        };

        foreach (Vector2Int dir in directionCandidates)
        {
            // Debug.Log($"check on direction {dir.x}");
            System.Diagnostics.Debug.WriteLine($"Clicked ({coordinate.x}, {coordinate.y})");
            System.Diagnostics.Debug.WriteLine($"Check on direction ({dir.x}, {dir.y}) 判断路线是否能覆盖.");

            if (JudgeDirection(coordinate, dir, _isBlackPlayersTurn))
            {
                System.Diagnostics.Debug.WriteLine("可以覆盖.");
                Vector2Int currentPoint = coordinate + dir;
                while (_isBlackPlayersTurn && ChessBoard[currentPoint.x, currentPoint.y] == Chess.State.White ||
                  !_isBlackPlayersTurn && ChessBoard[currentPoint.x, currentPoint.y] == Chess.State.Black)
                {
                    ChessBoard[currentPoint.x, currentPoint.y] = _isBlackPlayersTurn ? Chess.State.Black : Chess.State.White;

                    currentPoint += dir;
                }
            }
        }

        // Debug.Log("Before Click: " + ChessBoard[coordinate.x, coordinate.y]);
        ChessBoard[coordinate.x, coordinate.y] = _isBlackPlayersTurn ? Chess.State.Black : Chess.State.White;
        // Debug.Log("After Click: " + ChessBoard[coordinate.x, coordinate.y]);
        // _isBlackPlayersTurn = !_isBlackPlayersTurn;

        UpdateChessBoardState();
    }

}
