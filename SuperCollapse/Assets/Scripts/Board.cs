using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StateMachine { 
    BUSY,
    AVAILABLE
}

public class Board : MonoBehaviour {

    public int width; 
    public int height;

    public GameObject[] pieces; // pieces prefabs

    public GameObject tilePrefab;
    private BackgroundTile[,] tiles;

    public GameObject[,] allPieces; // Pieces in play

    public GameObject explosionPrefab; 

    public StateMachine currentState = StateMachine.AVAILABLE;
    private int linesAdded;

    public const int MIN_MATCH = 3; // Minimum number of pieces for a match
    public const double INITIAL_FILL_PERCENTAGE = 0.5; // Percentage of rows filled at start
    public const int MAX_ROWS = 100; // Maximum number of rows to add
    public const float ROW_INITIAL_DELAY = 5f;
    public const float ROW_DELAY = 1f;


    // Start is called before the first frame update
    void Start() {
        tiles = new BackgroundTile[width, height];
        allPieces = new GameObject[width, height];
        Scoring.ResetScore();
        SetUp();

        InvokeRepeating("addRow", ROW_INITIAL_DELAY, ROW_DELAY);
    }

    private void SetUp() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector2 pos = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, pos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + ", " + j + ")";

                if(!(j > height * INITIAL_FILL_PERCENTAGE)) { 
                    int rand = Random.Range(0, pieces.Length);
                    GameObject piece = Instantiate(pieces[rand], pos, Quaternion.identity); // TODO change to POOL
                    piece.transform.parent = this.transform;
                    piece.name = "(" + i + ", " + j + ")";
                    allPieces[i, j] = piece;
                }
            }
        }
    }

    public void DestroyPieces (HashSet<Piece> set) {
        Scoring.CalculateScore(set);

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allPieces[i,j] != null && set.Contains(allPieces[i,j].GetComponent<Piece>())) {
                    Instantiate(explosionPrefab, allPieces[i, j].transform.position, Quaternion.identity); // TODO change to POOL
                    Destroy(allPieces[i, j]); // TODO change to POOL
                    allPieces[i, j] = null;
                }
            }
        }
        StartCoroutine(collapseColumnCo());
    }

    private IEnumerator collapseColumnCo() { 
        for (int i = 0; i < width; i++) {
            int nullCount = 0; // reset count
            for (int j = 0; j < height; j++) {
                if(allPieces[i,j] == null) 
                    nullCount++;
                else if (nullCount > 0) { // shift down
                    allPieces[i, j].GetComponent<Piece>().row -= nullCount;
                    allPieces[i, j] = null;
                }
            }
        }
        yield return new WaitForSeconds(.1f);
        StartCoroutine(collapseRowCo());
        yield return new WaitForSeconds(.05f);
        currentState = StateMachine.AVAILABLE;
    }

    private IEnumerator collapseRowCo() {
        if (width % 2 == 1) {  // odd width
            throw new System.NotImplementedException(); // TODO not implemented yet
        }
        else {
            int i = width / 2;
            int nullCountR = 0;
            int nullCountL = 0;
            for (int n = 0; n < i ; n++) {
                if (allPieces[i + n, 0] == null) { // columns right of center
                    nullCountR++;
                } else if (nullCountR > 0) { // shift left
                    shiftColumn(nullCountR, i + n, false);
                }
                if (allPieces[i - n - 1, 0] == null) { // columns left of center
                    nullCountL++;
                }
                else if (nullCountL > 0) { // shift right
                    shiftColumn(nullCountL, i - n - 1, true);
                }
            }
        }

        
        yield return new WaitForSeconds(.3f);
        StartCoroutine(centerColumnsCo());
    }

    /**
     * Shifts a column shiftValue columns
     * 
     * @param shiftValue - how many times to shift this column
     * @param i - index of the column to shift
     * @param shiftingRight - true if shifting column to the right, false otherwise
     */
    private void shiftColumn(int shiftValue, int i, bool shiftingRight) {
        if (!shiftingRight)
            shiftValue = -shiftValue;

        for (int j = 0; j < height; j++) {
            if (allPieces[i, j] == null) {
                break;
            }

            allPieces[i, j].GetComponent<Piece>().col += shiftValue;
            allPieces[i, j] = null;
        }
    }

    private IEnumerator centerColumnsCo() {

        int firstCol = -1;
        int lastCol = -1;
        for (int i = 0; i < width; i++) { // Find first and last column
            if (allPieces[i, 0] != null) {
                if (firstCol == -1) {
                    firstCol = i;
                } else {
                    lastCol = i;
                }
            } else if (lastCol != -1) {
                break;
            }    
        }
        // Debug.Log(firstCol + ", " + lastCol);

        int nCols = lastCol - firstCol + 1;
        int center = (firstCol + lastCol) / 2 + (1 - nCols % 2);
        int shiftValue = width / 2 - center;
        // Debug.Log(nCols + ", " + center + ", " + shiftValue);


        // Shift if needed
        if (shiftValue > 0) { 
            for (int i = lastCol; i >= firstCol; i--) { // Start at rightmost column
                shiftColumn(shiftValue, i, true);
            }
        }
        if (shiftValue < 0) {
            for (int i = firstCol; i <= lastCol; i++) { // Start at lefttmost column
                shiftColumn(Mathf.Abs(shiftValue), i, false);
            }
        }
        
        yield return new WaitForSeconds(0.1f);

    }

    private void addRow() {
        currentState = StateMachine.BUSY;
        for (int i = 0; i < width; i++) {
            for (int j = height - 1; j >= 0; j--) {
                if (allPieces[i, j] != null ) {
                    if(j >= height - 1) { // Last row already filled
                        // GAME OVER
                        Scoring.PlayerWins(false);
                        SceneManager.LoadScene("GameOver");
                        break;
                    }

                    allPieces[i, j].GetComponent<Piece>().row += 1; // shift up
                    allPieces[i, j] = null;
                } 
                    
            }

            GameObject piece = Instantiate(pieces[Random.Range(0, pieces.Length)], new Vector2(i, 0), Quaternion.identity); // TODO change to POOL
            piece.transform.parent = this.transform;
            allPieces[i, 0] = piece;
        }
        
        
        if (linesAdded >= MAX_ROWS) {
            // YOU WIN!
            Scoring.PlayerWins(true);
            SceneManager.LoadScene("GameOver");  
        } else {
            linesAdded++;
        }
        currentState = StateMachine.AVAILABLE;
    }
}
