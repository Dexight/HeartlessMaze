using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Movement
    public float speed;
    private bool _canMove = true;
    [ReadOnlyProperty][SerializeField] private Vector2 targetPosition;
    #endregion

    #region checkers
    [SerializeField] Transform leftChecker;
    [SerializeField] Transform upChecker;
    [SerializeField] Transform rightChecker;
    [SerializeField] Transform downChecker;

    [SerializeField] Vector2 checkSizeLeftRight; //размер чекеров
    [SerializeField] Vector2 checkSizeUpDown; //размер чекеров

    [SerializeField] LayerMask wallLayer;

    private bool _canGoLeft;
    private bool _canGoUp;
    private bool _canGoRight;
    private bool _canGoDown;
    #endregion

    Animator anim;

    void Start()
    {
        targetPosition = transform.position;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //движение
        if (_canMove)//нажатия клавиш
            Move();
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);// перемещание персонажа
        if (transform.position.x == targetPosition.x && transform.position.y == targetPosition.y) //если пришёл в точку назначения
            _canMove = true;

        CheckWorld();
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            goUp();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            goLeft();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            goDown();
        }

       if (Input.GetKeyDown(KeyCode.D))
        {
            goRight();
        }
    }

    void CheckWorld()
    {
        _canGoLeft = !Physics2D.OverlapBox(leftChecker.position, checkSizeLeftRight, 0, wallLayer);
        _canGoUp = !Physics2D.OverlapBox(upChecker.position, checkSizeUpDown, 0, wallLayer);
        _canGoRight = !Physics2D.OverlapBox(rightChecker.position, checkSizeLeftRight, 0, wallLayer);
        _canGoDown = !Physics2D.OverlapBox(downChecker.position, checkSizeUpDown, 0, wallLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _canGoLeft ? Color.green : Color.red;
        Gizmos.DrawWireCube(leftChecker.position, checkSizeLeftRight);

        Gizmos.color = _canGoUp ? Color.green : Color.red;
        Gizmos.DrawWireCube(upChecker.position, checkSizeUpDown);

        Gizmos.color = _canGoRight ? Color.green : Color.red;
        Gizmos.DrawWireCube(rightChecker.position, checkSizeLeftRight);

        Gizmos.color = _canGoDown ? Color.green : Color.red;
        Gizmos.DrawWireCube(downChecker.position, checkSizeUpDown);
    }

    public void goUp()
    {
        if (_canGoUp)
        {
            targetPosition += Vector2.up;
            _canMove = false;
            anim.SetTrigger("Up");
            return;
        }
        else
        {
            //TODO
            Debug.Log("Вверх идти нельзя");
        }
    }

    public void goDown()
    {
        if (_canGoDown)
        {
            targetPosition += Vector2.down;
            _canMove = false;
            anim.SetTrigger("Down");
            return;
        }
        else
        {
            //TODO
            Debug.Log("Вниз идти нельзя");
        }
    }

    public void goLeft()
    {
        if (_canGoLeft)
        {
            targetPosition += Vector2.left;
            _canMove = false;
            anim.SetTrigger("Left");
            return;
        }
        else
        {
            //TODO
            Debug.Log("Влево идти нельзя");
        }
    }

    public void goRight()
    {
        if (_canGoRight)
        {
            targetPosition += Vector2.right;
            _canMove = false;
            anim.SetTrigger("Right");
            return;
        }
        else
        {
            //TODO
            Debug.Log("Вправо идти нельзя");
        }
    }
}