using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The default game piece
 * Finds matches through tag
 */
public class BasePiece : Piece {
    
    public override HashSet<Piece> GetMatches(Piece p, string tag, HashSet<Piece> set) {
        if (p.tag.Equals(tag) && !set.Contains(p)) {
           set.Add(p);

            // Check Neighbours
            GameObject neighbour;
            if (p.row + 1 < board.height) { // UP
                neighbour = board.allPieces[p.col, p.row + 1];
                if (neighbour != null)
                    set.UnionWith(GetMatches(neighbour.GetComponent<Piece>(), tag, set));
            }

            if (p.row - 1 >= 0) { // DOWN
                neighbour = board.allPieces[p.col, p.row - 1];
                if (neighbour != null)
                    set.UnionWith(GetMatches(neighbour.GetComponent<Piece>(), tag, set));
            }

            if (p.col + 1 < board.width) { // RIGHT
                neighbour = board.allPieces[p.col + 1, p.row];
                if (neighbour != null)
                    set.UnionWith(GetMatches(neighbour.GetComponent<Piece>(), tag, set));
            }

            if (p.col - 1 >= 0) { // LEFT
                neighbour = board.allPieces[p.col - 1, p.row];
                if (neighbour != null)
                    set.UnionWith(GetMatches(neighbour.GetComponent<Piece>(), tag, set));
            }
        }

        return set;
    }
}
