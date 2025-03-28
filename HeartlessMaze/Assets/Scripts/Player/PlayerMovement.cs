using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Movement
    public float speed;
    private bool _canMove = true;
    [SerializeField] private Vector2 targetPosition;
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

    void Start()
    {
        targetPosition = transform.position;
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
        if (Input.GetKeyDown(KeyCode.W) && _canGoUp)
        {
            targetPosition += Vector2.up;
            _canMove = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) && _canGoLeft)
        {
            targetPosition += Vector2.left;
            _canMove = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.S) && _canGoDown)
        {
            targetPosition += Vector2.down;
            _canMove = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.D) && _canGoRight)
        {
            targetPosition += Vector2.right;
            _canMove = false;
            return;
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
}