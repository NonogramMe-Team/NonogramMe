using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
using TMPro;
using System.Linq;
using System.Text;
using System.IO;

public class GameManager : MonoBehaviour
{   
    public TextMeshProUGUI row1Text;
    public TextMeshProUGUI row2Text;
    public TextMeshProUGUI row3Text;
    public TextMeshProUGUI row4Text;
    public TextMeshProUGUI row5Text;
    public TextMeshProUGUI row6Text;
    public TextMeshProUGUI row7Text;
    public TextMeshProUGUI row8Text;
    public TextMeshProUGUI row9Text;
    public TextMeshProUGUI row10Text;

    public TextMeshProUGUI col1Text;
    public TextMeshProUGUI col2Text;
    public TextMeshProUGUI col3Text;
    public TextMeshProUGUI col4Text;
    public TextMeshProUGUI col5Text;
    public TextMeshProUGUI col6Text;
    public TextMeshProUGUI col7Text;
    public TextMeshProUGUI col8Text;
    public TextMeshProUGUI col9Text;
    public TextMeshProUGUI col10Text;

    public GameObject winScreen;
    public TextMeshProUGUI winText;

    public GameObject loseScreen;
    public TextMeshProUGUI loseText;

    public GameObject resetBtn;
    public UnityEngine.UI.Button resetB;

    public GameObject menuBtn;
    public UnityEngine.UI.Button menuB;

    public GameObject menuBtn1;
    public UnityEngine.UI.Button menuB1;

    public GameObject nextBtn;
    public UnityEngine.UI.Button nextB;

    public GameObject menuBtn2;
    public UnityEngine.UI.Button menuB2;


    bool [,] solution0 = new bool[10,10] { {false,true,true,true,true,true,true,true,false,false},
                                              {false,true,true,true,false,false,true,false,false,false},
                                              {false,true,true,true,false,false,true,false,false,false},
                                              {false,true,true,true,false,false,true,false,false,false},
                                              {false,true,true,true,false,false,true,false,false,false},
                                              {true,true,true,true,true,true,true,true,true,false},
                                              {true,true,true,true,true,true,true,true,true,true},
                                              {true,true,true,true,true,true,true,true,false,true},
                                              {true,true,true,true,true,true,true,true,true,true},
                                              {false,true,true,false,false,false,false,true,true,false}
                                            };

    bool [,] puzzle = new bool[10,10];
    bool [,] solution = new bool[10,10];
    int puzzleNo;
    public TextMeshProUGUI strikesText;
    private int strikes = 0;
    public Tilemap tilemap;
    private static GameManager _instance;
    static bool _isGameOver;
    public GridLayout gridLayout;
    Vector3Int tilePos;
    Vector3Int tilePos2;
    public static GameManager Instance{
        get{
            if(_instance == null)
                Debug.LogError("Game Manager is NULL!");

                return _instance;
        }
    }



    // Awake is called before the first frame update
    private void Awake()
    {
        _instance = this;
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        winText.text = "";
        loseText.text = "";
        resetBtn.SetActive(false);
        SelectPuzzle();

    }

    // Update is called once per frame
    private void Update()
    {
        CheckMove();
    }

    private void SelectPuzzle(){
        Reset();

        var allLines = File.ReadAllLines("Assets/Gallery/Index.txt");
        int puzzles = int.Parse(allLines[0]);

        if(puzzles!=0){
            int index = 1;
            string line = allLines[index];
            puzzleNo = Random.Range(1,puzzles+1);
            while(index<allLines.Length&&char.GetNumericValue(line[0])!=puzzleNo){
                index++;
                line = allLines[index];
            }

            for(int i = 0; i<10;i++){
                for(int j = 0; j<10; j++){
                    if(char.GetNumericValue(line[(i*10)+j+2])==1){
                        solution[j,i] = true;
                    } else {
                        solution[j,i] = false;
                    }
                }
            }
        } else {
            puzzleNo = 0;
            for(int i = 0; i<10; i++){
                for(int j = 0; j<10; j++){
                    solution[i,j] = solution0[i,j];
                }
            }
        }        
        LoadPuzzle();
    }

    private void LoadPuzzle(){
        int count = 0;
        string [] assignRow = new string[10];
        for(int i = 0; i<10; i++){
            var builder = new StringBuilder();
            for(int j = 0; j<10;j++){
                if(solution[i,j]==true){
                    count++;
                } else {
                    if(count>0){
                        builder.Append(count);
                        builder.Append(' ');
                        count = 0;
                    }
                }
            }
            if(count > 0){
                builder.Append(count);
            }
            assignRow[i] = builder.ToString();
            count = 0;
        }

        //count trues for columns
        count = 0;
        string [] assignCol = new string[10];
        for(int i = 0; i<10; i++){
            var builder1 = new StringBuilder();
            for(int j = 0; j<10;j++){
                if(solution[j,i]==true){
                    count++;
                } else {
                    if(count>0){
                        builder1.Append(count);
                        builder1.Append('\n');
                        count = 0;
                    }
                }
            }
            if(count > 0){
                builder1.Append(count);
            }
            assignCol[i] = builder1.ToString();
            count = 0;
            strikesText.text = " ";
        } 

        //assign values to text

        row1Text.text = assignRow[0];
        row2Text.text = assignRow[1];
        row3Text.text = assignRow[2];
        row4Text.text = assignRow[3];
        row5Text.text = assignRow[4];
        row6Text.text = assignRow[5];
        row7Text.text = assignRow[6];
        row8Text.text = assignRow[7];
        row9Text.text = assignRow[8];
        row10Text.text = assignRow[9];

        col1Text.text = assignCol[0];
        col2Text.text = assignCol[1];
        col3Text.text = assignCol[2];
        col4Text.text = assignCol[3];
        col5Text.text = assignCol[4];
        col6Text.text = assignCol[5];
        col7Text.text = assignCol[6];
        col8Text.text = assignCol[7];
        col9Text.text = assignCol[8];
        col10Text.text = assignCol[9];
    }

    private bool CheckSolution(){
        for(int i = 0; i < 10; i++){
            for(int j = 0; j < 10; j++){
                if(puzzle[i,j]!=solution[i,j]){
                    return false;
                }
            }
        }
        //solution is correct initiate win sequence
        win();
        return true;
        }
    
    private bool CheckCol(Vector3Int tilePos){
        for(int i = 0; i < 10; i++){
            if(puzzle[i,(int)(tilePos.x+5)]!=solution[i,(int)(tilePos.x+5)]){
                return false;
            }
        }
        //gray out column numbers
        switch (tilePos.x+5){
            case 0:
                col1Text.color = Color.gray;
            break;
            case 1:
                col2Text.color = Color.gray;
            break;
            case 2:
                col3Text.color = Color.gray;
            break;
            case 3:
                col4Text.color = Color.gray;
            break;
            case 4:
                col5Text.color = Color.gray;
            break;
            case 5:
                col6Text.color = Color.gray;
            break;
            case 6:
                col7Text.color = Color.gray;
            break;
            case 7:
                col8Text.color = Color.gray;
            break;
            case 8:
                col9Text.color = Color.gray;
            break;
            case 9:
                col10Text.color = Color.gray;
            break;
        }
        return true;
    }

    private bool CheckRow(Vector3Int tilePos){
        for(int i = 0; i < 10; i++){
            if(puzzle[(int)(4-tilePos.y),i]!=solution[(int)(4-tilePos.y),i]){
                return false;
            }
        }
        //gray out row numbers
        switch (4-tilePos.y){
            case 0:
                row1Text.color = Color.gray;
            break;
            case 1:
                row2Text.color = Color.gray;
            break;
            case 2:
                row3Text.color = Color.gray;
            break;
            case 3:
                row4Text.color = Color.gray;
            break;
            case 4:
                row5Text.color = Color.gray;
            break;
            case 5:
                row6Text.color = Color.gray;
            break;
            case 6:
                row7Text.color = Color.gray;
            break;
            case 7:
                row8Text.color = Color.gray;
            break;
            case 8:
                row9Text.color = Color.gray;
            break;
            case 9:
                row10Text.color = Color.gray;
            break;
        }
        return true;
    }

    private Vector3Int GetMouse(){
        Vector3Int tilePos = new Vector3Int();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tileComponents = gridLayout.WorldToCell(worldPos);
        tilePos.Set(tileComponents.x,tileComponents.y,tileComponents.z);
        //Debug.Log("mouse data x: "+tileComponents.x+" y: "+tileComponents.y+" z: "+tileComponents.z);
        return tilePos;
    }

    private void CheckMove(){
        if(Input.GetMouseButtonUp(0)){
            tilePos2 = GetMouse();
            //Debug.Log("difference of x: "+(tilePos2.y-tilePos.y)+" y: "+(tilePos2.x-tilePos.x));
            if(tilePos2.y>tilePos.y&&tilePos2.x==tilePos.x){//bottom to top
                for(int i = tilePos.y; i<=tilePos2.y; i++){
                    tilePos.y = i;
                    CheckTile(tilePos);
                }
            }
            if(tilePos2.y<tilePos.y&&tilePos2.x==tilePos.x){//top to bottom
                for(int i = tilePos2.y; i<=tilePos.y; i++){
                    tilePos2.y = i;
                    CheckTile(tilePos2);
                }
            }
            if(tilePos2.y==tilePos.y&&tilePos2.x>tilePos.x){//left to right
                for(int i = tilePos.x; i<=tilePos2.x; i++){
                    tilePos.x = i;
                    CheckTile(tilePos);
                }    
            }
            if(tilePos2.y==tilePos.y&&tilePos2.x<tilePos.x){//right to left
               for(int i = tilePos2.x; i<=tilePos.x; i++){
                    tilePos2.x = i;
                    CheckTile(tilePos2);
               }
            }
        }
        if(Input.GetMouseButtonDown(0)){
            tilePos = GetMouse();
            CheckTile(tilePos);
        }
    }

    void CheckTile(Vector3Int tpos){
        bool rowCorrect = false;
        bool colCorrect = false;
         if(solution[(int)(4-tpos.y),(int)(tpos.x+5)]){
              //Debug.Log("pressed down on x: "+(4-tpos.y)+" y: "+(tpos.x+5));
              //tilemap.SetTileFlags(tilePos,TileFlags.LockColor);
              tilemap.SetColor(tpos,Color.black);
              puzzle[(int)(4-tpos.y),(int)(tpos.x+5)] = true;
              colCorrect = CheckCol(tpos);
              rowCorrect = CheckRow(tpos);
              if(colCorrect&&rowCorrect){ //row is right and col is right
                     CheckSolution();
               }
            } else {
                strikes++;
                tilemap.SetColor(tilePos,Color.white);
                UpdateStrikes(strikes);
                GameOver(strikes);
            }        
    }

    private void win(){

        menuBtn.SetActive(false);

        col1Text.text = "";
        col2Text.text = "";
        col3Text.text = "";
        col4Text.text = "";
        col5Text.text = "";
        col6Text.text = "";
        col7Text.text = "";
        col8Text.text = "";
        col9Text.text = "";
        col10Text.text = "";

        row1Text.text = "";
        row2Text.text = "";
        row3Text.text = "";
        row4Text.text = "";
        row5Text.text = "";
        row6Text.text = "";
        row7Text.text = "";
        row8Text.text = "";
        row9Text.text = "";
        row10Text.text = "";
        
        winScreen.SetActive(true);
        winText.text = "You Win!";
        resetBtn.SetActive(true);
        resetB.onClick.AddListener(Reset);
        nextBtn.SetActive(true);
        nextB.onClick.AddListener(SelectPuzzle);
        
    }

    private void lose(){

        menuBtn.SetActive(false);

        col1Text.text = "";
        col2Text.text = "";
        col3Text.text = "";
        col4Text.text = "";
        col5Text.text = "";
        col6Text.text = "";
        col7Text.text = "";
        col8Text.text = "";
        col9Text.text = "";
        col10Text.text = "";

        row1Text.text = "";
        row2Text.text = "";
        row3Text.text = "";
        row4Text.text = "";
        row5Text.text = "";
        row6Text.text = "";
        row7Text.text = "";
        row8Text.text = "";
        row9Text.text = "";
        row10Text.text = "";

        loseScreen.SetActive(true);
        loseText.text = "You Lose";
        resetBtn.SetActive(true);
        resetB.onClick.AddListener(Reset);
    }

    private void UpdateStrikes(int strikes){
        var builder = new StringBuilder();
        for(int i = 0; i<strikes; i++){
            builder.Append('X');
        }
        strikesText.text = builder.ToString();
    }
    
    public void GameOver(int strikes){
        if(strikes == 5){
            _isGameOver = true;
            lose();
        } else {
            _isGameOver = false;
        }
    }

    public bool IsGameOver(){ //returns if gameOver state
        return _isGameOver;
    }

    public void Reset(){
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        resetBtn.SetActive(false);
        menuBtn.SetActive(true);
        winText.text = "";
        loseText.text = "";
        strikes = 0; //reset strikes
        GameOver(strikes); //reset gameover state
        strikesText.text = "";
        for(int i = 0; i < 10; i++){ //reset puzzle map
            for(int j = 0; j < 10; j++){
                puzzle[i,j] = false;
            }
        }
        foreach (var position in tilemap.cellBounds.allPositionsWithin) { //reset tilemap
            if (!tilemap.HasTile(position)) {
                continue;
            }
            tilemap.SetTileFlags(position,TileFlags.None);
            tilemap.SetColor(position, Color.white);
        }

        col1Text.color = Color.white;
        col2Text.color = Color.white;
        col3Text.color = Color.white;
        col4Text.color = Color.white;
        col5Text.color = Color.white;
        col6Text.color = Color.white;
        col7Text.color = Color.white;
        col8Text.color = Color.white;
        col9Text.color = Color.white;
        col10Text.color = Color.white;

        row1Text.color = Color.white;
        row2Text.color = Color.white;
        row3Text.color = Color.white;
        row4Text.color = Color.white;
        row5Text.color = Color.white;
        row6Text.color = Color.white;
        row7Text.color = Color.white;
        row8Text.color = Color.white;
        row9Text.color = Color.white;
        row10Text.color = Color.white;

        LoadPuzzle();
    }

}