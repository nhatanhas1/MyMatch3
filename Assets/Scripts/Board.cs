using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Board : MonoBehaviour
{
    public int width = 7;
    public int height = 7;

    [SerializeField] GameObject tileBG;
    [SerializeField] Gem[] gemTypeList;
    [SerializeField] Gem boomGem;

    public FindMatcher findMatcher;

    public enum BoardState { move, wait };
    public BoardState state;

    private void Awake()
    {
        findMatcher = FindObjectOfType<FindMatcher>();
    }

    //m?ng 2 chi?u l?u toàn b? Gem ?ang có trên màn hình
    public Gem[,] allGems;
    
    int gemType;


    // Start is called before the first frame update
    void Start()
    {
        allGems = new Gem[width, height];
        Setup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Setup()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject gemSlot = Instantiate(tileBG, pos, Quaternion.identity, transform);
                //GameObject gemSlot = Instantiate(tileBG, gemPos, Quaternion.identity);
                //gemSlot.transform.parent = transform;
                gemSlot.name = $"BG - {x} - {y}";

                //Random.Ragnge(min,max): sinh giá tr? ng?u nhiên trong ?o?n min, max;
                gemType = Random.Range(0,gemTypeList.Length);
                Vector2Int gemPos = new Vector2Int(x,y);

                int loopCheck = 0;
                 
                //Kiem tra gem type truoc khi tao. Neu cung loai thi random lai cho den khi ra loai khac
                while (CheckGemTypeBeforeGenerate(gemPos, gemTypeList[gemType]) && loopCheck < 100)
                {
                    gemType = Random.Range(0,gemTypeList.Length);
                    loopCheck++;
                }
                GenerateGem(gemPos, gemTypeList[gemType]);
            }
        }
    }

    bool CheckGemTypeBeforeGenerate(Vector2Int pos, Gem gem)
    {
        bool result = false;
        if (pos.x > 1)
        {
            if (allGems[pos.x - 1, pos.y].type == gem.type && allGems[pos.x - 2, pos.y].type == gem.type)
            {
                result = true;
            }
        }
        if (pos.y > 1)
        {
            if (allGems[pos.x, pos.y - 1].type == gem.type && allGems[pos.x, pos.y - 2].type == gem.type)
            {
                result = true;
            }
        }
        return result;
    }

    void GenerateGem(Vector2Int pos,Gem gem)
    {
        if (Random.Range(0, 100) < 2)
        {
            Debug.Log("Boom");
            gem = boomGem;
        }

        Gem newGem = Instantiate(gem,new Vector3(pos.x,pos.y,0f),Quaternion.identity);
        newGem.transform.parent = transform;
        newGem.name = $"Gem {pos.x},{pos.y}";

        allGems[pos.x, pos.y] = newGem;

        newGem.InitGem(pos,this,findMatcher);
    }

    
    
    void DeleteMatchGem(Vector2Int pos)
    {
        if (allGems[pos.x,pos.y] != null && allGems[pos.x, pos.y].isMatched)
        {

            Instantiate(allGems[pos.x, pos.y].GameObjectEffect, new Vector3(pos.x, pos.y, -1), Quaternion.identity);
            //Instantiate(allGems[pos.x, pos.y].GameObjectEffect,allGems[pos.x,pos.y].transform);

            Destroy(allGems[pos.x, pos.y].gameObject);

            allGems[pos.x,pos.y] = null;
               
            //gemType = Random.Range(0, gemTypeList.Length);
            //GenerateGem(pos, gemTypeList[gemType]);

        }
    }

    public void DeleteAllMatches()
    {
        //StartCoroutine(CheckMatchesGemList());
        for (int i = 0; i < findMatcher.matchesList.Count; i++)
        {
            DeleteMatchGem(findMatcher.matchesList[i].gemPos);

        }

        StartCoroutine(MoveGemDown());
    }

    IEnumerator MoveGemDown()
    {
        yield return new WaitForSeconds(0.5f);
        for (int x = 0; x < width; x++)
        {
            int countNullRow = 0;

            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    countNullRow++;
                }
                else if (countNullRow > 0) 
                {
                    //allGems[x,y].gemPos = new Vector2Int(x,y - countNullRow);
                    allGems[x, y].gemPos.y -= countNullRow;
                    allGems[x, y - countNullRow] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }
        }

        StartCoroutine(FillBoardCo());
    }

    IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(0.5f);
        ReFillBoard();
        yield return new WaitForSeconds(0.2f);
        findMatcher.FindAllMatches();
        if (findMatcher.matchesList.Count > 0)
        {
            yield return new WaitForSeconds(0.2f);
            DeleteAllMatches();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            state = Board.BoardState.move;
        }
    }

    void ReFillBoard()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(allGems[x, y] == null)
                {
                    gemType = Random.Range(0, gemTypeList.Length);
                    GenerateGem(new Vector2Int(x, y), gemTypeList[gemType]);
                }
                
            }
        }

        //findMatcher.FindAllMatches();
    }

    void CheckWrongGem()
    {
        List<Gem> foundGems = new List<Gem>();
        //foundGems chua tat ca Gem dang co trong cua so hierarchy
        foundGems.AddRange(FindObjectsOfType<Gem>());
        for(int x = 0; x < width; x++)
        {
            for(int y =0; y < height; y++)
            {
                if (foundGems.Contains(allGems[x, y]))
                {
                    foundGems.Remove(allGems[x, y]);
                }
            }
        }

        foreach(Gem g in foundGems)
        {
            Destroy(g.gameObject);
        }
    }
}
