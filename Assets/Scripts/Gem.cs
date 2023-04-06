using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class Gem : MonoBehaviour
{

    FindMatcher findMatcher;

    // Start is called before the first frame update
    public Vector2Int gemPos;
    [SerializeField] Vector2Int gemPreviousPos;

    [HideInInspector] //Không hi?n th? trên inspector
    public Board board;

    [SerializeField] Gem otherGem;

    Vector2 firstPosition, lastPosition;

    bool mousePressed = false;

    bool changeGem = false;

    float swipeAngle;

    public enum GemType { blue,green,red,purple,yellow,boom};
    public GemType type;

    public bool isMatched;

    public GameObject GameObjectEffect;

    // Update is called once per frame
    void Update()
    {   
        MoveGemAnimation();
    }

    public void InitGem(Vector2Int gemSpawnPosition, Board gemBoard, FindMatcher matcher)
    {
        gemPos = gemSpawnPosition;
        board = gemBoard;
        findMatcher = matcher;
    }

    private void OnMouseDown()
    {
        firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log($"FirstPosition: {firstPosition}");
        //mousePressed = true;
    }
    private void OnMouseUp()
    {
        lastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log($"LastPosition: {lastPosition}");
        //mousePressed = false;

        if(board.state == Board.BoardState.move)
        {
            
            CalculateAngle();
        }
        
    }

    void CalculateAngle()
    {
        if(Vector2.Distance(firstPosition,lastPosition) > 0.5f)
        {
            //Tính góc c?a 2 c?nh trong tâm giác v?i công th?c l??ng giác Tan 
            swipeAngle = Mathf.Atan2(lastPosition.y - firstPosition.y, lastPosition.x - firstPosition.x);

            //chuyen tu radian qua do 
            swipeAngle = swipeAngle * 180 / Mathf.PI;
            
            //Debug.Log(swipeAngle);
            changeGem = false;
            ChangeGemPosition();
        }
       
    }

    void ChangeGemPosition()
    {
        board.state = Board.BoardState.wait;
        
        gemPreviousPos = gemPos;

        if (swipeAngle < 45 && swipeAngle > -45 && gemPos.x < board.width - 1)
        {
            otherGem = board.allGems[gemPos.x + 1, gemPos.y]; // lay otherGem tu Board Gem Array
            //Swap vi tri cua 2 gem
            otherGem.gemPos.x--;            
            gemPos.x++;
            changeGem = true;
            
        }
        else if (swipeAngle < 135 && swipeAngle >= 45 && gemPos.y < board.height - 1)
        {
            otherGem = board.allGems[gemPos.x, gemPos.y + 1];
            otherGem.gemPos.y--;            
            gemPos.y++;            
            changeGem = true;
        }
        else if (swipeAngle <= -45 && swipeAngle > -135 && gemPos.y > 0)
        {
            otherGem = board.allGems[gemPos.x, gemPos.y - 1];
            otherGem.gemPos.y++;
            gemPos.y--;            
            changeGem = true;
        }
        else if ((swipeAngle < -135 || swipeAngle > 135) && gemPos.x > 0)
        {
            otherGem = board.allGems[gemPos.x - 1, gemPos.y];
            otherGem.gemPos.x++;
            gemPos.x--;            
            changeGem = true;
        }

        //Update lai board Gems array
        if (changeGem && otherGem != null)
        {
            changeGem = false;
            board.allGems[gemPos.x, gemPos.y] = this;
            board.allGems[otherGem.gemPos.x, otherGem.gemPos.y] = otherGem;

            //otherGem.transform.position = new Vector3(otherGem.gemPos.x, otherGem.gemPos.y, 0f);
            //transform.position = new Vector3(gemPos.x, gemPos.y, 0);

           
            //gemPos = new Vector2Int(gemPos.x, gemPos.y);

            //otherGem.transform.position = Vector2.Lerp(otherGem.transform.position, this.transform.position, 7f * Time.deltaTime);
            //transform.position = Vector2.Lerp(transform.position, gemPos, 7f * Time.deltaTime);

            board.findMatcher.FindAllMatches();

            StartCoroutine(ReturnGemPositionIfNotMatches());

        }
    }

    IEnumerator ReturnGemPositionIfNotMatches()
    {
        //Debug.Log("Truoc Yield ");
        board.state = Board.BoardState.wait;

        yield return new WaitForSeconds(0.5f);

        //Debug.Log("Sau Yield");
        
        if(!isMatched && !otherGem.isMatched)
        {
            otherGem.gemPos = gemPos;
            gemPos = gemPreviousPos;

            board.allGems[gemPos.x, gemPos.y] = this;
            board.allGems[otherGem.gemPos.x, otherGem.gemPos.y] = otherGem;

            //otherGem.transform.position = new Vector3(otherGem.gemPos.x, otherGem.gemPos.y, 0f);
            //transform.position = new Vector3(gemPreviousPos.x, gemPreviousPos.y, 0);

            board.state = Board.BoardState.move;
        }
        else
        {
            board.DeleteAllMatches();
        }

    }

    void MoveGemAnimation()
    {
        if(Vector2.Distance(transform.position, gemPos) > 0.01)
        {
            transform.position = Vector2.Lerp(transform.position, gemPos, 10f * Time.deltaTime);
        }
        
    }

}
