using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TestTask;
using System;


public class InfiniteScroll : MonoBehaviour
{
    public event Action PhaseChanged;
    public SpinPhase Phase { get; private set; } = SpinPhase.None;

    [SerializeField] private RectTransform _content;
    [SerializeField] private SlotsFactory _slotsFactory;
    [SerializeField] private InfiniteScrollConfig _config;

    private SlotType _targetSlotType = SlotType.None;

    private List<Slot> _slots = new List<Slot>();

    private int _nextSequenceNumber = 0;
    private int _targetSequenceNumber = -1;

    private float currentSpeed;

    void Awake()
    {
        Init();
    }

    public void StartSpin(SlotType targetSlot = SlotType.None)
    {
        if (Phase != SpinPhase.None) 
            return;

        _targetSlotType = targetSlot;

        StartCoroutine(ProcessSpin());
    }

    public void StopSpin()
    {
        if (Phase != SpinPhase.None)
            SetPhase(SpinPhase.Brake);
    }

    private void Init()
    {
        var requiredSlotsCount = Mathf.CeilToInt(_config.VisibleHeight / _config.SlotHeight) + 1;
        for (int i = 0; i < requiredSlotsCount; i++)
        {
            var newSlot = _slotsFactory.Create(_content, _config.SlotWidth, _config.SlotHeight);
            AddNext(newSlot);
        }
        _content.anchoredPosition += Vector2.down * ((int)(_config.VisibleHeight / 2 / _config.SlotHeight) + 1) * _config.SlotHeight;
    }

    private void SetPhase(SpinPhase phase)
    {
        if (Phase == phase)
            return;
        Phase = phase;
        PhaseChanged?.Invoke();
    }

    private IEnumerator ProcessSpin()
    {
        currentSpeed = 0;

        yield return StartCoroutine(AccelerateRoutine());
        yield return StartCoroutine(SpinRoutine());
        yield return StartCoroutine(BrakeRoutine());
        SetPhase(SpinPhase.None);
    }

    private IEnumerator AccelerateRoutine()
    {
        SetPhase(SpinPhase.Accelerate);

        var accelerationProgress_0_1 = 0f;

        while (accelerationProgress_0_1 < 1)
        {
            var dt = Time.deltaTime;
            accelerationProgress_0_1 += dt / _config.AccelerationTime;
            currentSpeed = Mathf.Lerp(0, _config.MaxSpeed, _config.AcceleratingCurve.Evaluate(accelerationProgress_0_1));
            MoveContenPanek(currentSpeed * dt);
            yield return null;
        }
    }

    private IEnumerator SpinRoutine()
    {
        SetPhase(SpinPhase.Spin);

        while (Phase == SpinPhase.Spin)
        {
            MoveContenPanek(currentSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator BrakeRoutine()
    {
        SetPhase(SpinPhase.Brake);

        var brakingDistance = GetBrakingDistance();
        _targetSequenceNumber = GetTargetSequenceNumberByBrakingDistance(brakingDistance);
        var brakingDistanceToCenterOfTarget = GetBrakeDistanceToCenterOfTarget();

        var distanceToStartBrake = brakingDistanceToCenterOfTarget - brakingDistance;

        while (distanceToStartBrake > 0)
        {
            var moveDelta = currentSpeed * Time.deltaTime;
            if (distanceToStartBrake - moveDelta < 0)
            {
                moveDelta = distanceToStartBrake;
            }
            distanceToStartBrake -= moveDelta;
            MoveContenPanek(moveDelta);
            yield return null;
        }

        var breakingProgress_0_1 = 0f;

        while (breakingProgress_0_1 < 1)
        {
            var dt = Time.deltaTime;
            breakingProgress_0_1 += dt / _config.BrakingTime;
            currentSpeed = Mathf.LerpUnclamped(_config.MaxSpeed, 0, _config.BrakingCurve.Evaluate(breakingProgress_0_1));
            MoveContenPanek(currentSpeed * dt);
            yield return null;
        }
    }

    private void MoveContenPanek(float delta)
    {
        _content.anchoredPosition += Vector2.down * delta;
        RepositionOutOfViewSlots();
    }

    private void RepositionOutOfViewSlots()
    {
        float bottomThreshold = -(_config.VisibleHeight + _config.SlotHeight) / 2;

        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            var slot = _slots[i];
            float localY = slot.RectTransform.anchoredPosition.y + _content.anchoredPosition.y;

            if (localY < bottomThreshold)
            {
                _slots.RemoveAt(i);
                AddNext(slot);
            }
        }
    }

    private void AddNext(Slot slot)
    {
        slot.Setup(GetNextSlotType());
        slot.RectTransform.anchoredPosition = Vector2.up * (_nextSequenceNumber * _config.SlotHeight); 
        _slots.Insert(0, slot);
        _nextSequenceNumber++;
    }

    private SlotType GetNextSlotType()
    {
        if (_nextSequenceNumber == _targetSequenceNumber && _targetSlotType != SlotType.None)
            return _targetSlotType;
        return Utils.GetRandomEnumValue<SlotType>(exceptValues: SlotType.None);
    }

    private int GetTargetSequenceNumberByBrakingDistance(float brakingDistance)
    {
        var totalDistance = -_content.anchoredPosition.y + brakingDistance;
        return (int)((totalDistance + _config.SlotHeight) / _config.SlotHeight);
    }

    private float GetBrakeDistanceToCenterOfTarget()
    {
        var centeredTotalDistance = _targetSequenceNumber * _config.SlotHeight;
        return centeredTotalDistance + _content.anchoredPosition.y;
    }

    private float GetBrakingDistance()
    {
        var timeStep = 0.005f;
        var currTime = 0f;
        var resultDistance = 0f;

        while(currTime < _config.BrakingTime)
        {
            currTime += timeStep;
            var currSpeed = Mathf.LerpUnclamped(_config.MaxSpeed, 0, _config.BrakingCurve.Evaluate(currTime / _config.BrakingTime));
            resultDistance += currSpeed * timeStep;
        }

        return resultDistance;
    }
}

public enum SpinPhase
{
    None,
    Accelerate,
    Spin,
    Brake
}