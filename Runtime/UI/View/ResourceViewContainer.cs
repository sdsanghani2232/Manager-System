using System;
using System.Collections;
using System.Linq;
using Managers.UI;
using UnityEngine;

namespace Mangers.UI
{
    /// <summary>
    /// Resource-based view container that generates and manages views loaded from resources.
    /// </summary>
    [CreateAssetMenu(fileName = "View Holder", menuName = "UI/View Containers/View Holder",order = 1)]
    public class ResourceViewContainer : ViewContainer
    {
        public override void GenerateView<T>(UIManager uiManager, Action<BaseView> result)
        {
            GenerateView(typeof(T), uiManager, result);
        }

        public override void GenerateView(Type viewType, UIManager uiManager, Action<BaseView> result)
        {
            if (TryGetViewItem(viewType, out var resourceViewItem))
            {
                var resourcePath = resourceViewItem.ResourcePath;
                uiManager.StartCoroutine(GenerateResourcesView(resourcePath, uiManager, result));
            }
            else
            {
                result.Invoke(null);
            }
        }

        public override void DestroyView(BaseView view)
        {
            Destroy(view.gameObject);
        }

        protected bool TryGetViewItem<T>(out ResourceViewItem viewItem)
        {
            return TryGetViewItem(typeof(T), out viewItem);
        }

        protected bool TryGetViewItem(Type viewType, out ResourceViewItem viewItem)
        {
#if !ODIN_INSPECTOR
            var viewTypeName = viewType.ToString().Split('.').Last();
            viewItem = views.FirstOrDefault(view => view.ViewTypeName.Equals(viewTypeName));
            return viewItem != null;
#else
            viewItem = views.FirstOrDefault(view => view.ViewType == viewType);
            return viewItem != null;
#endif
        }

        public override bool HasView<T>()
        {
            return HasView(typeof(T));
        }

        public override bool HasView(Type viewType)
        {
            return TryGetViewItem(viewType, out _);
        }

        #region Private Coroutine

        private IEnumerator GenerateResourcesView(string path, UIManager uiManager, Action<BaseView> result)
        {
            if (string.IsNullOrEmpty(path))
            {
                result.Invoke(null);
                yield break;
            }

            var resourceRequest = Resources.LoadAsync<GameObject>(path);
            yield return new WaitUntil(() => resourceRequest.isDone);

            var viewObject = Instantiate(resourceRequest.asset, uiManager.ViewParent) as GameObject;
            result.Invoke(viewObject?.GetComponent<BaseView>());
        }

        #endregion
    }

    /// <summary>
    /// Represents a view item with a type and a resource path for loading.
    /// </summary>
    [Serializable]
    public class ResourceViewItem
    {
        #region Serialized Fields

#if !ODIN_INSPECTOR
        [SerializeField, Tooltip("Name of the view type.")]
        private string viewTypeName;
#else
        [SerializeField, Tooltip("Type of the view.")]
        private Type viewType;
#endif
        [SerializeField, Tooltip("Path to the view resource.")]
        private string resourcePath;

        #endregion

        #region Properties

#if !ODIN_INSPECTOR
        public string ViewTypeName => viewTypeName;
#else
        public Type ViewType => viewType;
#endif
        public string ResourcePath => resourcePath;

        #endregion
    }
}
