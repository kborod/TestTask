using UnityEngine;

namespace TestTask
{
    public class SlotsFactory : MonoBehaviour
    {
        [SerializeField] private Slot _prefab;
        public Slot Create(Transform parent, float width, float height)
        {
            var slot = Instantiate(_prefab, parent);
            slot.RectTransform.sizeDelta = new Vector2(width, height);
            return slot;
        }
    }
}