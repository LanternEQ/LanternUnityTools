using System.Collections.Generic;
using Lantern.EQ.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lantern.Legacy.CharacterViewer
{
    public class CharacterViewerTracks : MonoBehaviour
    {
        public GameObject Content = null;
        public GameObject ItemTemplate = null;
        public delegate void PlayOneShot(AnimationType animationType);

        private RectTransform _contentRectTransform;
        private RectTransform _templateRectTransform;
        private Image _templateImage;

        Dictionary<string, string> _items = new Dictionary<string, string>();
        List<GameObject> _goItems = new List<GameObject>();

        void Start()
        {
            _contentRectTransform = Content.GetComponent<RectTransform>();
            _templateRectTransform = ItemTemplate.GetComponent<RectTransform>();
            _templateImage = ItemTemplate.GetComponentInChildren<Image>();
        }

        public void HighlightItem(string selectedItem)
        {
            foreach (GameObject goItem in _goItems)
            {
                Text text = goItem.GetComponentInChildren<Text>();
                Image image = goItem.GetComponentInChildren<Image>();
                CharacterViewerTrackItem ti = goItem.GetComponentInChildren<CharacterViewerTrackItem>();

                image.color = Color.clear;
                if (ti.itemID == selectedItem)
                    image.color = _templateImage.color;
            }
        }

        public void ClearList()
        {
            foreach (GameObject go in _goItems)
            {
                Destroy(go);
            }

            _goItems.Clear();
        }

        public void RefreshList(Dictionary<string, string> items, PlayOneShot playOneShotAnimation)
        {
            // Keep Items Locally
            _items = items;

            // Clear List of Items
            ClearList();

            float yPos = 0.0f;
            int i = 0;

            foreach (KeyValuePair<string, string> item in items)
            {
                // Clone Item from Template and add to items
                GameObject goItem = Instantiate(ItemTemplate, ItemTemplate.transform.parent);

                goItem.transform.SetParent(ItemTemplate.transform.parent);

                // Fix Position and Scale
                RectTransform rt = goItem.GetComponent<RectTransform>();
                rt.localPosition = new Vector3(0.0f, yPos, 0.0f);
                rt.localScale = Vector3.one;

                // Set Text
                CharacterViewerTrackItem ti = goItem.GetComponentInChildren<CharacterViewerTrackItem>();
                ti.itemID = item.Key;

                ti.onClick.AddListener((string animationSuffix) => {
                    var animation = AnimationHelper.GetAnimationType(animationSuffix);
                    if (animation != null)
                        playOneShotAnimation((AnimationType) animation);
                });

                // Set Text
                TextMeshProUGUI text = goItem.GetComponentInChildren<TextMeshProUGUI>();
                text.text = item.Value;

                // Set Background Color
                Image image = goItem.GetComponentInChildren<Image>();
                image.color = Color.clear;

                // Set GameObject Name and Make Active
                goItem.name = "Item " + i;
                goItem.SetActive(true);

                // Add to Running List for Destroying Later
                _goItems.Add(goItem);

                yPos -= _templateRectTransform.rect.height;
                i++;
            }

            // Resize Content Height to Accomodate New Item
            Vector2 sizeDelta = new Vector2(0.0f, yPos * -1);
            _contentRectTransform.sizeDelta = sizeDelta;
        }
    }
}
