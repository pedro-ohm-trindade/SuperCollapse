using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoring : MonoBehaviour
{
    private static int totalScore;
    private static bool win;


    private static void AddPoints(int points) {
        totalScore += points;
    }

    public static void CalculateScore(HashSet<Piece> set) {
        AddPoints(set.Count * 10); // Basic Score
    }

    public static int getScore() {
        return totalScore;
    }
    public static bool isWin() {
        return win;
    }

    public static void PlayerWins(bool win) { 
        Scoring.win = win;
    }

    public static void ResetScore() {
        totalScore = 0;
        win = false;
    }
}
