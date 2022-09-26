using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreUi : MonoBehaviour
{
    [SerializeField] private RowUi rowUi;
    ScoreManager scoreManager;

    private void displayRows()
    {
        var arrangedScores = scoreManager.GetHighScores();
        for (int index = 0; index < arrangedScores.Length; index++)
        {
            var row = Instantiate(rowUi, transform).GetComponent<RowUi>();
            row.displayRankUserScore(index + 1, arrangedScores[index]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = ScoreManager.instance;
        displayRows();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
