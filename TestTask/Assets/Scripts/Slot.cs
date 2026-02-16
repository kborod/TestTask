using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask
{
    public class Slot : MonoBehaviour
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        [SerializeField] private Image _image;
        [SerializeField] private InfiniteScrollConfig _config;

        public void Setup(SlotType type)
        {
            _image.sprite = _config.GetSlotSprite(type);
        }
    }
}