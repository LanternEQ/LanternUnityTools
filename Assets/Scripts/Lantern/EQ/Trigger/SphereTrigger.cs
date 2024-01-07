using UnityEngine;

namespace Lantern.EQ.Trigger
{
    public abstract class SphereTrigger : MonoBehaviour
    {
        [SerializeField]
        protected SphereCollider _collider;

        [SerializeField]
        protected string _tag;

        protected abstract void OnEnter();
        protected abstract void OnExit();

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag(_tag))
            {
                OnEnter();
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag(_tag))
            {
                OnExit();
            }
        }
    }
}
