using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject MessageBox;
    public Text Message;
    public Text BlackScore;
    public Text WhiteScore;
    public Transform ChessParent;
    public Chess ChessPrefab;
    public Transform ReferencePoint;
    public static int BoardSize { get; private set; } = 5;

    private Chess[,] _chesses = new Chess[BoardSize, BoardSize];

    private int _chessBoardVer = 0;

    void Start()
    {
        CreateChesses();
        RefreshChesses();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowMessage("msg");
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SetMessageBoxActive(false);
        }

        if (_chessBoardVer != GameManager.Instance.ChessBoardVer)
        {
            _chessBoardVer = GameManager.Instance.ChessBoardVer;
            // Debug.Log("Current Board Version: " + _chessBoardVer);
            RefreshChesses();

            if (GameManager.Instance.GameOver)
            {
                ShowMessage("游戏结束!黑色: " + BlackScore.text + "白色: " + WhiteScore.text);
            }
            // if (GameManager.Instance._unavailableTimes == 1)
            // {
            //     GameManager.Instance.
            // }
        }
    }

    public void SetMessageBoxActive(bool active)
    {
        MessageBox.SetActive(active);
    }

    public void SetScore(bool setBlack, int score)
    {
        if (setBlack)
        {
            BlackScore.text = score.ToString();
        }
        else
        {
            WhiteScore.text = score.ToString();
        }
    }

    public void ShowMessage(string message)
    {
        Message.text = message;
        SetMessageBoxActive(true);
    }

    void CreateChesses()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                _chesses[x, y] = Instantiate<Chess>(ChessPrefab, ReferencePoint.position + new Vector3(x * 80, y * 80), Quaternion.identity, ChessParent);
                _chesses[x, y].Coordinate = new Vector2Int(x, y);
            }
        }
    }

    public void RefreshChesses()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                _chesses[x, y].ChangeState(GameManager.Instance.ChessBoard[x, y]);
            }
        }

        BlackScore.text = GameManager.Instance.BlackScore.ToString();
        WhiteScore.text = GameManager.Instance.WhiteScore.ToString();
    }

    public void RestartGame()
    {
        GameManager.Instance.InitGame();
        SetMessageBoxActive(false);
    }
}
