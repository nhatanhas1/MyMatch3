using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatcher : MonoBehaviour
{
    Board board;
    Gem gem;
    public List<Gem> matchesList = new List<Gem>();

    int boomRadius = 1;

    //public bool onFindMatchGem;
    private void Awake()
    {
        board = FindObjectOfType<Board>(); 
        //gem = GetComponent<Gem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void FindAllMatches()
    {
        //onFindMatchGem = true;

        //Debug.Log("FindAllMatchesCall");
        //Xóa list cho m?i l?n update
        matchesList.Clear();
        for(int x = 0; x < board.width; x++)
        {
            for(int y = 0; y < board.height; y++)
            {
                Gem currentGem = board.allGems[x, y];

                if (currentGem != null)
                {
                    if(x>0 && x < board.width - 1)
                    {
                        Gem leftGem = board.allGems[x -1, y];
                        Gem rightGem = board.allGems[x + 1, y];
                        if(leftGem != null && rightGem != null)
                        {
                            if(currentGem.type == leftGem.type && currentGem.type == rightGem.type)
                            {
                                currentGem.isMatched = true;
                                leftGem.isMatched = true;
                                rightGem.isMatched = true;

                                matchesList.Add(currentGem);
                                matchesList.Add(leftGem);
                                matchesList.Add(rightGem);
                            }
                        }
                    }

                    if (y > 0 && y < board.height - 1)
                    {
                        Gem aboveGem = board.allGems[x, y + 1];
                        Gem belowGem = board.allGems[x, y - 1];
                        if (aboveGem != null && belowGem != null)
                        {
                            if (currentGem.type == aboveGem.type && currentGem.type == belowGem.type)
                            {
                                currentGem.isMatched = true;
                                aboveGem.isMatched = true;
                                belowGem.isMatched = true;

                                matchesList.Add(currentGem);
                                matchesList.Add(aboveGem);
                                matchesList.Add(belowGem);
                            }
                        }
                    }
                }
            }


        }

        CheckBoom();


        //x? lý lo?i b? nh?ng ph?n t? chùng c?a 2 nhóm gem giao nhau 
        if (matchesList.Count > 0)
        {
            matchesList = matchesList.Distinct().ToList();
            
            //board.DeleteAllMatches();
        }

        CheckBoom();
        //onFindMatchGem = false;



    }


    void CheckBoom()
    {
        if (matchesList.Count > 0)
        {
            for (int i = 0; i < matchesList.Count; i++)
            {
                int x = matchesList[i].gemPos.x;
                int y = matchesList[i].gemPos.y;

                if (x > 0)
                {
                    if(board.allGems[x-1, y] != null)
                    {
                        if (board.allGems[x-1,y].type == Gem.GemType.boom)
                        {
                            MarkBoom(board.allGems[x-1,y].gemPos, board.allGems[x-1,y]);
                        }
                    }
                }

                if (x < board.width -1)
                {
                    if (board.allGems[x + 1, y] != null)
                    {
                        if (board.allGems[x + 1, y].type == Gem.GemType.boom)
                        {
                            MarkBoom(board.allGems[x + 1, y].gemPos, board.allGems[x + 1, y]);
                        }
                    }
                }

                if (y > 0)
                {
                    if (board.allGems[x , y -1] != null)
                    {
                        if (board.allGems[x , y-1].type == Gem.GemType.boom)
                        {
                            MarkBoom(board.allGems[x , y -1].gemPos, board.allGems[x , y -1]);
                        }
                    }
                }


                if (y < board.height - 1)
                {
                    if (board.allGems[x , y +1] != null)
                    {
                        if (board.allGems[x , y +1].type == Gem.GemType.boom)
                        {
                            MarkBoom(board.allGems[x , y +1].gemPos, board.allGems[x , y +1]);
                        }
                    }
                }
            }
        }
    }

    void MarkBoom(Vector2Int thePos, Gem theBomb)
    {
        for(int x = thePos.x - boomRadius; x <= thePos.x + boomRadius; x++)
        {
            for(int y = thePos.y - boomRadius; y <= thePos.y + boomRadius; y++)
            {
                if(x>=0 && x < board.width && y>=0 && y < board.height)
                {
                    if (board.allGems[x,y] != null)
                    {
                        board.allGems[x, y].isMatched = true;
                        matchesList.Add(board.allGems[x, y]);
                        board.allGems[x, y].isMatched = true;  
                    }
                }
            }
        }

        if(matchesList.Count > 0)
        {
            matchesList = matchesList.Distinct().ToList();
        }
    }
}
