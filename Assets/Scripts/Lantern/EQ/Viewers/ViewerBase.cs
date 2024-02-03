using System.Text;
using UnityEngine;

namespace Lantern.EQ.Viewers
{
    public abstract class ViewerBase : MonoBehaviour
    {
        [Header("Viewer Base")]
        [SerializeField]
        private ViewerError _viewerError;

        protected AssetBundleLoader AssetBundleLoader;
        protected StringBuilder DebugTextBuilder;

        protected virtual void Awake()
        {
            AssetBundleLoader = new AssetBundleLoader();
            DebugTextBuilder = new StringBuilder();
        }

        protected void ShowError(string text)
        {
            _viewerError.ShowError(text);
        }
    }
}
