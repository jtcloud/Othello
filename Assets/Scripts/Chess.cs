using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chess : MonoBehaviour
{
    // Start is called before the first frame update

    public Image ChessImage;
    public Button ChessButton;

    public Sprite Black;
    public Sprite White;
    public Sprite Available;

    public Vector2Int Coordinate;

    public enum State
    {
        Unavailable,
        Available,
        Black,
        White
    }

    public void ChangeState(State state)
    {
        if (state == State.Unavailable)
        {
            ChessButton.interactable = false;
            ChessImage.sprite = null;
            ChessImage.color = new Color(1, 1, 1, 0);
        }
        if (state == State.Available)
        {
            ChessButton.interactable = true;
            ChessImage.sprite = Available;
            ChessImage.color = new Color(1, 1, 1, 1);
        }
        if (state == State.Black)
        {
            ChessButton.interactable = false;
            ChessImage.sprite = Black;
            ChessImage.color = new Color(1, 1, 1, 1);
        }
        if (state == State.White)
        {
            ChessButton.interactable = false;
            ChessImage.sprite = White;
            ChessImage.color = new Color(1, 1, 1, 1);
        }
    }

    public void OnClick()
    {

        Debug.Log("clicked: " + Coordinate.x + ", " + Coordinate.y);

        // Debug.Log("board size: " + UIManager.BoardSize);

        // Coordinate 在UIManager中prefab初始化时赋值的

        GameManager.Instance.CellClicked(Coordinate);

    }
}
