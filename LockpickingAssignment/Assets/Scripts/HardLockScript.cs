using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardLockScript : MonoBehaviour
{
    //Lock states 
    enum Mode { easy, medium, hard, insane }

    //Lock and lockpick sensitivities 
    [SerializeField] float lockpickSensitivity = 50;
    [SerializeField] float lockSensitivity = 35;

    [SerializeField] float lockTime = 0.25f;
    float stepAngle = 10;

    [SerializeField] Mode lockMode;
    int easyMode = 30;
    int mediumMode = 15;
    int hardMode = 7;
    int insaneMode = 2;

    [SerializeField] Transform lockpick;
    [SerializeField] Transform lockpickPivot;
    [SerializeField] Transform keyhole;
    [SerializeField] Transform keyholePivot;

    float keyholeRotate, lockpickRotate, min, max, stepMinA, stepMaxA, stepMinB, stepMaxB, lockLimit, keyholeTime;
    int targetAngle, offsetAngle;
    bool isUnlocked, stopped;
    Vector3 defaultAngles;
    float shakePower = 20;

    public bool IsUnlocked { get { return isUnlocked; } }

    //On awake setting pivot to their parents / calculating angles 
    private void Awake()
    {
        lockpick.SetParent(lockpickPivot);
        keyhole.SetParent(keyholePivot);
        defaultAngles = lockpickPivot.eulerAngles;
        CalcAngles();
    }

    private void CalcAngles()
    {   //Randomising target angle 
        targetAngle = UnityEngine.Random.Range(-90, 90);

        switch (lockMode)
        {   //If state is easy - set angle offset to easy 
            case Mode.easy:
                offsetAngle = easyMode;
                break;
            // --//--
            case Mode.medium:
                offsetAngle = mediumMode;
                break;
            // --//--
            case Mode.hard:
                offsetAngle = hardMode;
                break;
            // --//--
            case Mode.insane:
                offsetAngle = insaneMode;
                break;
        }
        //If target angle is more than 0 
        if (targetAngle > 0) //Update offset 
            offsetAngle = targetAngle - offsetAngle;
        else //If target angle is less than 0 - update offset
            offsetAngle = targetAngle + offsetAngle;
        //Set min and max 
        min = Mathf.Min(offsetAngle, targetAngle);
        max = Mathf.Max(offsetAngle, targetAngle);

        if (max > 0)
        {
            stepMinA = min - (stepAngle / 2);
            stepMaxA = min;
            stepMinB = stepMinA - stepAngle;
            stepMaxB = stepMinA;
        }
        else
        {
            stepMaxA = max + (stepAngle / 2);
            stepMinA = max;
            stepMaxB = stepMaxA + stepAngle;
            stepMinB = stepMaxA;
        }
    }

    void ShakeLockpick()
    {
        if (isUnlocked) return;
        stopped = true;
        Vector3 tnd = UnityEngine.Random.insideUnitSphere * shakePower;
        lockpickPivot.eulerAngles = defaultAngles + new Vector3(tnd.x, tnd.y, lockpickPivot.eulerAngles.z);
    }

    void ResetShake()
    {
        stopped = false;
        lockpickPivot.eulerAngles = new Vector3(defaultAngles.x, defaultAngles.y, lockpickPivot.eulerAngles.z);
    }

    void LockControl()
    {
        if (CheckRange() && Input.GetAxis("Horizontal") < 0)
        {
            if (!stopped) keyholeRotate += lockSensitivity * Time.deltaTime;

            if (keyholeRotate >= 90)
            {
                stopped = true;
                isUnlocked = true;
                UnityEngine.SceneManagement.SceneManager.LoadScene("InsaneLockScene");
            }
            else if (keyholeRotate >= lockLimit)
                ShakeLockpick();
        }
        else if (!CheckRange() && Input.GetAxis("Horizontal") < 0)
        {
            keyholeTime += Time.deltaTime;

            if (keyholeTime < lockTime)
                keyholeRotate += lockSensitivity * Time.deltaTime;

            ShakeLockpick();
        }
        else
        {
            keyholeTime = 0;
            lockLimit = 90;
            ResetShake();
            keyholeRotate -= lockSensitivity * Time.deltaTime;
        }

        keyholeRotate = Mathf.Clamp(keyholeRotate, 0, lockLimit);
        keyholePivot.eulerAngles = new Vector3(keyholePivot.eulerAngles.x, keyholePivot.eulerAngles.y, -keyholeRotate);
    }

    void LockpickControl()
    {
        if (Input.GetAxis("Mouse X") < 0)
            lockpickRotate += lockpickSensitivity * Time.deltaTime;
        else if (Input.GetAxis("Mouse X") > 0)
            lockpickRotate -= lockpickSensitivity * Time.deltaTime;

        lockpickRotate = Mathf.Clamp(lockpickRotate, -90, 90);
        lockpickPivot.eulerAngles = new Vector3(lockpickPivot.eulerAngles.x, lockpickPivot.eulerAngles.y, lockpickRotate);
    }

    bool CheckRange()
    {
        if (stopped) return false;

        if (lockpickRotate < stepMaxB && lockpickRotate > stepMinB)
        {
            lockLimit = Math.Abs(stepMinB);
            return true;
        }
        else if (lockpickRotate < stepMaxA && lockpickRotate > stepMinA)
        {
            lockLimit = Math.Abs(stepMinA);
            return true;
        }
        else if (lockpickRotate < max && lockpickRotate > min)
        {
            lockLimit = 90;
            return true;
        }

        return false;

    }

    void CheckTimer()
    {
        float time = TimerScript.Instance.time;
        if (time <= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameoverScene");
        }
    }

    private void LateUpdate()
    {
        if (isUnlocked)
            enabled = false;

        CheckTimer();
        

        LockControl();
        LockpickControl();
    }

}
