using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer = 10f;
    private float gamePlayingTimerMax = 10f;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;

        state = State.WaitingToStart;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f)
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;
            case State.GameOver:
                break;
        }
    }

    public bool IsCountdownToStartActive()
    {
        return state == State.CountdownToStart;
    }

    /// <summary>
    /// 判断游戏是否在进行中
    /// </summary>
    /// <returns></returns>
    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    /// <summary>
    /// 判断游戏是否结束
    /// </summary>
    /// <returns></returns>
    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    /// <summary>
    /// 得到开始倒计时的剩余时间
    /// </summary>
    /// <returns></returns>
    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    /// <summary>
    /// 得到剩余事件的百分比
    /// </summary>
    /// <returns></returns>
    public float GetGamePlayingTimerNormalized()
    {
        return 1 - gamePlayingTimer / gamePlayingTimerMax;
    }
}