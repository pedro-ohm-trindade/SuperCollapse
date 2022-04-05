using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Abstract class for a game piece
 */
[RequireComponent(typeof(Collider2D))]
public abstract class Piece : MonoBehaviour {
    public int row;
    public int col;

    protected Board board;
    protected Vector2 mousePosition;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        board = FindObjectOfType<Board>();
        col = (int)transform.position.x;
        row = (int)transform.position.y;
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        // confirm current position
        if (Mathf.Abs(col - transform.position.x) > .1)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(col, transform.position.y), .5f);
            if (board.allPieces[col, row] != this.gameObject)
                board.allPieces[col, row] = this.gameObject;
        }
        else
        {
            transform.position = new Vector2(col, transform.position.y);
            board.allPieces[col, row] = this.gameObject;
        }


        if (Mathf.Abs(row - transform.position.y) > .1)
        {
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, row), .5f);
            if (board.allPieces[col, row] != this.gameObject)
                board.allPieces[col, row] = this.gameObject;
        }
        else
        {
            transform.position = new Vector2(transform.position.x, row);
            board.allPieces[col, row] = this.gameObject;
        }
    }

    protected virtual void OnMouseDown()
    {
        if (board.currentState == StateMachine.AVAILABLE)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Debug.Log(mousePosition);

            // Check matches
            GameObject pieceObj = board.allPieces[col, row]; //Debug.Log(this.col + ", " + this.row);
            if (pieceObj != null) { 
                Piece piece = pieceObj.GetComponent<Piece>();
                HashSet<Piece> matchingPieces = GetMatches(piece, piece.tag, new HashSet<Piece>());
                //Debug.Log(matchingPieces.Count);

                if (matchingPieces.Count >= Board.MIN_MATCH)
                {
                    board.currentState = StateMachine.BUSY;
                    board.DestroyPieces(matchingPieces);
                }
            }
        }
    }

    public virtual HashSet<Piece> GetMatches(Piece p, string tag, HashSet<Piece> set) {
        return set;
    }
}
