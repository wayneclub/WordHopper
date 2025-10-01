using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public static WordManager I;

    [Tooltip("固定 4-letter 單字")]
    public string[] wordBank = { "LOVE", "TIME", "HOME", "PLAY", "GOOD", "MOVE", "GAME", "WALK", "JUMP", "TALK",
  "READ", "COOK", "SING", "SHOP", "LOOK", "STOP", "OPEN", "DOOR", "ROOM", "FOOD",
  "BOOK", "MILK", "RAIN", "WIND", "FIRE", "SNOW", "BIRD", "FISH", "TREE", "STAR" };

    [Tooltip("是否加入 1 個干擾字母；若場景放 5 個 block 建議勾選")]
    public bool includeTrapLetter = true;

    public string CurrentWord { get; private set; }

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    /// <summary>
    /// 依照目前場景方塊數量回傳隨機打亂的字母清單。
    /// </summary>
    public List<char> PrepareLettersForBlocks(int blockCount)
    {
        // 1) 隨機選一個 4-letter 單字
        CurrentWord = wordBank[Random.Range(0, wordBank.Length)].ToUpper();

        // 2) 準備字母清單
        var letters = CurrentWord.ToCharArray().ToList();

        // 3) 如果需要干擾字母
        if (includeTrapLetter && blockCount > letters.Count)
        {
            char trap;
            do { trap = (char)('A' + Random.Range(0, 26)); } while (letters.Contains(trap));
            letters.Add(trap);
        }

        // 4) 打亂順序並回傳 blockCount 個
        return letters.OrderBy(_ => Random.value).Take(blockCount).ToList();
    }
}