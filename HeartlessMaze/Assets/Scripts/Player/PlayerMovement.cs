using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    #region Movement
    public float speed;
    private bool _canMove = true;
    [ReadOnlyProperty][SerializeField] private Vector2 targetPosition;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private GameObject pauseButton;
    #endregion

    #region checkers
    [SerializeField] Transform leftChecker;
    [SerializeField] Transform upChecker;
    [SerializeField] Transform rightChecker;
    [SerializeField] Transform downChecker;

    [SerializeField] GameObject leftTextHelper;
    [SerializeField] GameObject rightTextHelper;
    [SerializeField] GameObject upTextHelper;
    [SerializeField] GameObject downTextHelper;


    [SerializeField] Vector2 checkSizeLeftRight; //размер чекеров
    [SerializeField] Vector2 checkSizeUpDown; //размер чекеров

    [SerializeField] LayerMask wallLayer;

    private bool _canGoLeft;
    private bool _canGoUp;
    private bool _canGoRight;
    private bool _canGoDown;
    [ReadOnlyProperty] public bool _isPaused = false;
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

        leftTextHelper.SetActive(_canGoLeft);
        rightTextHelper.SetActive(_canGoRight);
        upTextHelper.SetActive(_canGoUp);
        downTextHelper.SetActive(_canGoDown);
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            pressW();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            pressA();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            pressS();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            pressD();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pressEscape();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            pressReturn();
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

    public void pressW()
    {
        if (!_isPaused)
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
        else
        {
            pauseMenu.previousButton();
        }
    }

    public void pressS()
    {
        if (!_isPaused)
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
        else
        {
            pauseMenu.nextButton();
        }
    }

    public void pressA()
    {
        if (!_isPaused)
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
        else
        {
            //nothing?   
        }
    }

    public void pressD()
    {
        if (!_isPaused)
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
        else
        {
            //nothing?
        }
    }

    public void pressEscape()
    {
        if (_isPaused)
        {
            Debug.Log("Not paused");
            pauseMenu.gameObject.SetActive(false);
            pauseButton.SetActive(true);
            _isPaused = false;
        }
        else
        {
            Debug.Log("Paused");
            pauseMenu.gameObject.SetActive(true);
            pauseButton.SetActive(false);
            _isPaused = true;
        }
    }

    public void pressReturn()
    {
        if (_isPaused) 
        {
            pauseMenu.pressButton();
        }
        else
        {
            // nothing?
        }
    }
}