using System;
using TMPro;
using UnityEngine;

namespace Lantern.EQ.Viewers
{
    public class SelectionItemZone : SelectionItemBase
    {
        [SerializeField]
        private TextMeshProUGUI _longName;

        [SerializeField]
        private TextMeshProUGUI _shortName;

        public override void Initialize(Action<int> action, params string[] args)
        {
            SelectionAction = action;
            _longName.text = args[0];
            _shortName.text = args[1];
        }

        public override void Clear()
        {
            base.Clear();
            _longName.text = string.Empty;
            _shortName.text = string.Empty;
        }
    }
}
