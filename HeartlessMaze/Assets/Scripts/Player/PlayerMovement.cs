using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    #region Movement
    public float speed;
    private bool _canMove = true;
    [ReadOnlyProperty][SerializeField] private Vector2 targetPosition;
    [SerializeField] private GameObject pauseMenu;
    #endregion

    #region checkers
    [SerializeField] Transform leftChecker;
    [SerializeField] Transform upChecker;
    [SerializeField] Transform rightChecker;
    [SerializeField] Transform downChecker;

    [SerializeField] Vector2 checkSizeLeftRight; //������ �������
    [SerializeField] Vector2 checkSizeUpDown; //������ �������

    [SerializeField] LayerMask wallLayer;

    private bool _canGoLeft;
    private bool _canGoUp;
    private bool _canGoRight;
    private bool _canGoDown;
    private bool _isPaused = false;
    #endregion

    Animator anim;
    
    void Start()
    {
        pauseMenu.SetActive(false);
        targetPosition = transform.position;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //��������
        if (_canMove)//������� ������
            Move();
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);// ����������� ���������
        if (transform.position.x == targetPosition.x && transform.position.y == targetPosition.y) //���� ������ � ����� ����������
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z pressed");
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
            Debug.Log("����� ���� ������");
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
            Debug.Log("���� ���� ������");
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
            Debug.Log("����� ���� ������");
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
            Debug.Log("������ ���� ������");
        }
    }

    public void pressPause()
    {
        if(_isPaused)
        {
            Debug.Log("Not paused");
            pauseMenu.SetActive(false);
            _isPaused = false;
        }
        else
        {
            Debug.Log("Paused");
            pauseMenu.SetActive(true);
            _isPaused = true;
        }
    }

    public void pressZ()
    {
        StartCoroutine(PressZCoroutine());
        Debug.Log("FunctionZWOrkedLog");
    }

    IEnumerator PressZCoroutine()
    {
        var kb = InputSystem.GetDevice<Keyboard>();
        InputSystem.QueueStateEvent(kb, new KeyboardState(Key.Z));
        yield return null;
        InputSystem.QueueStateEvent(kb, new KeyboardState());
    }
}