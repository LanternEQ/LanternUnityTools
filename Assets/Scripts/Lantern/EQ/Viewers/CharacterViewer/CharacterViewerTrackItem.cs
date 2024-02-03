using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lantern.Legacy.CharacterViewer
{
    public class CharacterViewerTrackItem : MonoBehaviour, IPointerClickHandler
    {
        public string itemID = null;

        [System.Serializable]
        public class TrackItemEvent : UnityEvent<string> { }

        public TrackItemEvent onClick;

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            onClick.Invoke(itemID);
        }
    }
}
