using TMPro;
using UnityEngine;

namespace Lantern.Legacy.CharacterViewer
{
    public class CharacterViewerUI : MonoBehaviour
    {
        [SerializeField]
        private OrbitCamera cameraNew;

        [SerializeField]
        private TextMeshProUGUI _text;
        void Update()
        {
            _text.text = cameraNew.CurrentDistance.ToString("0.00");
        }
    }
}
