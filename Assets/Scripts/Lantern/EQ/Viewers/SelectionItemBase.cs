using System;
using UnityEngine;

namespace Lantern.EQ.Viewers
{
    public abstract class SelectionItemBase : MonoBehaviour
    {
        protected Action<int> SelectionAction;
        private int _index;

        public abstract void Initialize(Action<int> action, params string[] args);

        public void OnClick()
        {
            SelectionAction?.Invoke(_index);
        }

        public void SetIndex(int index)
        {
            _index = index;
        }

        public virtual void Clear()
        {
            SelectionAction = null;
        }
    }
}
