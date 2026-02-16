using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TestTask
{
    [CreateAssetMenu(fileName = "InfiniteScrollConfig", menuName = "Configs/InfiniteScrollConfig", order = 1)]
    public class InfiniteScrollConfig: ScriptableObject
    {
        [field: Header("При небольшом времени торможения\nили небольшой скорости выбор целевого\nконечного слота работать не будет, т.к.\nне хватит расстояния торможения\nна генерацию целевого слота")]
        [field: Header("Слоты")]
        [field: SerializeField] public float VisibleHeight { get; private set; } = 800;
        [field: SerializeField] public float SlotWidth { get; private set; } = 300;
        [field: SerializeField] public float SlotHeight { get; private set; } = 300;
        [field: Range(1, 5000)]
        [field: SerializeField] public float MaxSpeed { get; private set; } = 2500;
        [field: Header("Разгон, сек.")]
        [field: Range(0.1f, 5)]
        [field: SerializeField] public float AccelerationTime { get; private set; } = 0.6f;
        [field: SerializeField] public AnimationCurve AcceleratingCurve { get; private set; }


        [field: Header("Торможение, сек.")]
        [field: Range(0.1f, 5)]
        [field: SerializeField] public float BrakingTime { get; private set; } = 1f;
        [field: SerializeField] public AnimationCurve BrakingCurve { get; private set; }

        [SerializeField] private List<SlotData> _slotData;

        public Sprite GetSlotSprite(SlotType slotType)
        {
            return _slotData.First(s => s.Type == slotType).Sprite;
        }

        //По хорошему ассеты через адрессеблы грузить
        [Serializable]
        public class SlotData
        {
            [field: SerializeField] public SlotType Type; 
            [field: SerializeField] public Sprite Sprite; 
        }

    }
}