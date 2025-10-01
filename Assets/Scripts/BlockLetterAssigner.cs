using System.Collections.Generic;
using UnityEngine;

public class BlockLetterAssigner : MonoBehaviour
{
    void Start()
    {
        // 找出場景所有 LetterBlock
        var blocks = FindObjectsByType<LetterBlock>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (blocks == null || blocks.Length == 0)
        {
            Debug.LogWarning("[BlockLetterAssigner] 場景中找不到 LetterBlock。");
            return;
        }

        if (WordManager.I == null)
        {
            Debug.LogError("[BlockLetterAssigner] 場景中找不到 WordManager。");
            return;
        }

        // 向 WordManager 取得要分配的字母
        List<char> letters = WordManager.I.PrepareLettersForBlocks(blocks.Length);

        // 分配字母給每個方塊（若字母少於方塊數，就只配前幾個）
        for (int i = 0; i < blocks.Length && i < letters.Count; i++)
        {
            blocks[i].SetLetter(letters[i]);
        }

        Debug.Log($"[BlockLetterAssigner] Word: {WordManager.I.CurrentWord}, Letters: {string.Join(",", letters)}");
    }
}