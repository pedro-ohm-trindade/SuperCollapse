using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * This piece can't be matched
 * On mouse click, it destroys it's adjacent neighbours (up, down, left, right)
 * and can be chained/matched with any number of BombPiece
 */
public class BombPiece : Piece {

    protected override void OnMouseDown()
    {
        HashSet<Piece> set = this.BombChain(new HashSet<Piece>());
        board.DestroyPieces(set);
    }

    private HashSet<Piece> BombChain(HashSet<Piece> set)
    {
        if (!set.Contains(this))
        {
            GameObject neighbour;
            set.Add(this);
            if (base.row + 1 < board.height) { // UP
                neighbour = board.allPieces[base.col, base.row + 1];
                if (neighbour != null) {
                    Piece piece = neighbour.GetComponent<Piece>();
                    if (piece is BombPiece) {
                        ((BombPiece)piece).BombChain(set);
                    }
                    
                    set.Add(piece);    
                }
            }

            if (base.row - 1 >= 0) { // DOWN
                neighbour = board.allPieces[base.col, base.row - 1];
                if (neighbour != null) {
                    Piece piece = neighbour.GetComponent<Piece>();
                    if (piece is BombPiece) {
                        ((BombPiece)piece).BombChain(set);
                    }
                    
                    set.Add(piece);
                 }
            }

            if (base.col + 1 < board.width) { // RIGHT
                neighbour = board.allPieces[base.col + 1, base.row];
                if (neighbour != null) {
                    Piece piece = neighbour.GetComponent<Piece>();
                    if (piece is BombPiece) {
                        ((BombPiece)piece).BombChain(set);
                    }
                    
                    set.Add(piece);
                }
            }

            if (base.col - 1 >= 0) { // LEFT
                neighbour = board.allPieces[base.col - 1, base.row];
                if (neighbour != null) {
                    Piece piece = neighbour.GetComponent<Piece>();
                    if (piece is BombPiece) {
                        ((BombPiece)piece).BombChain(set);
                    }
                    
                    set.Add(piece);
                }
            }

            
        }
        return set;
    }
}
