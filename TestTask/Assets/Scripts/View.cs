using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask
{
    public class View : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private InfiniteScroll _scroll;

        private void Awake()
        {
            PopulateDropdownWithSlotType();
        }

        private void OnEnable()
        {
            _startButton.onClick.AddListener(StartClickHandler);
            _stopButton.onClick.AddListener(StopClickHandler);
            _scroll.PhaseChanged += RefreshButtons;

            RefreshButtons();
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(StartClickHandler);
            _stopButton.onClick.RemoveListener(StopClickHandler);
            _scroll.PhaseChanged -= RefreshButtons;
        }

        private void StartClickHandler()
        {
            _scroll.StartSpin((SlotType)_dropdown.value);
        }

        private void StopClickHandler()
        {
            _scroll.StopSpin();
        }

        private void PopulateDropdownWithSlotType()
        {
            _dropdown.ClearOptions();
            List<string> enumNames = new List<string>(Enum.GetNames(typeof(SlotType)));
            _dropdown.AddOptions(enumNames);
        }

        private void RefreshButtons()
        {
            _startButton.interactable = _scroll.Phase == SpinPhase.None;
            _stopButton.interactable = _scroll.Phase == SpinPhase.Spin;
        }
    }
}