using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public enum Player
{
    Nobody,
    PlayerX,
    PlayerO
}


public class ControllerGameplay : MonoBehaviour
{

    private int columns = 3;
    private int rows = 3;

    public ButtonXO bttnPrefab;

    private Player whoseTurn = Player.PlayerX;
    private Player[,] boardData; // all the data of who owns what
    private ButtonXO[,] boardUI; // all the buttons

    public Transform panelGameBoard; // grid of buttons


    void Start()
    {
        BuildBoardUI();
    }

    private void BuildBoardUI()
    {

        boardUI = new ButtonXO[columns, rows]; // instantiating array for buttons

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                ButtonXO bttn = Instantiate(bttnPrefab, panelGameBoard);
                bttn.Init(new GridPOS(x, y), ()=> { ButtonClicked(bttn); });
                boardUI[x, y] = bttn;

            }
        }

    }
    void ButtonClicked(ButtonXO bttn)
    {
        ControllerGameClient.singleton.SendPlayPacket(bttn.pos.X, bttn.pos.Y);
    }

    public void UpdateFromServer(byte gameStatus, byte whoseTurn, byte[] spaces)
    {
        // TODO: update all of the interface to reflect game state:
        // - whose turn
        // - 9 spaces on board
        // - status
        print("update from server: ");

        for (int i = 0; i < spaces.Length; i++)
        {
            byte b = spaces[i];

            int x = i % 3;
            int y = i / 3;


            boardUI[x, y].SetOwner(b);
            print(b);
        }



    }
}
