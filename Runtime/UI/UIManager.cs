using System;
using System.Collections.Generic;
using Managers.UI;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Mangers.UI
{
    /// <summary>
    /// Manages UI views by generating, showing, hiding, and initializing views as needed.
    /// Contains a dictionary of active views and a reference to a ViewContainer for view creation.
    /// </summary>
    public class UIManager : Manager
    {
        /// <summary>
        /// Dictionary of active views keyed by their type.
        /// </summary>
        public Dictionary<Type, BaseView> views = new Dictionary<Type, BaseView>();

        [SerializeField] private ViewContainer viewContainer;
        [SerializeField] private RectTransform viewParent;

        /// <summary>
        /// Gets the view container used for generating views.
        /// </summary>
        public ViewContainer ViewContainer => viewContainer;

        /// <summary>
        /// Gets the RectTransform parent under which new views are instantiated.
        /// </summary>
        public RectTransform ViewParent => viewParent;

        /// <summary>
        /// Called when the UIManager is initialized. Initializes all pre-existing views.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            foreach (var view in views.Values)
                view.Initialize(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// Shows a view of the specified type. If the view does not exist, generates it from the ViewContainer.
        /// </summary>
        /// <typeparam name="T">Type of the view to show.</typeparam>
        /// <param name="args">Optional parameters for view setup.</param>
        public virtual void ShowView<T>(params object[] args) where T : BaseView
        {
            if (views.TryGetValue(typeof(T), out BaseView view))
            {
                view.Show(args);
            }
            else
            {
                viewContainer.GenerateView<T>(this, resultView =>
                {
                    if (PrepareView(typeof(T), resultView))
                    {
                        resultView.Show(args);
                    }
                });
            }
        }

        /// <summary>
        /// Hides a view of the specified type if it exists.
        /// </summary>
        /// <typeparam name="T">Type of the view to hide.</typeparam>
        public virtual void HideView<T>() where T : BaseView
        {
            if (views.TryGetValue(typeof(T), out BaseView view))
            {
                view.Hide();
            }
        }

        /// <summary>
        /// Gets a view of the specified type if it exists in the dictionary.
        /// </summary>
        /// <typeparam name="T">Type of the view to retrieve.</typeparam>
        /// <returns>The requested view if found; otherwise, null.</returns>
        public T GetView<T>() where T : BaseView
        {
            return views.TryGetValue(typeof(T), out BaseView view) ? view as T : null;
        }

        /// <summary>
        /// Removes a view of the specified type from the dictionary of active views.
        /// </summary>
        /// <param name="type">Type of the view to remove.</param>
        public void RemoveView(Type type)
        {
            views.Remove(type);
        }

        /// <summary>
        /// Prepares a newly generated view by adding it to the active views dictionary and initializing it.
        /// </summary>
        /// <param name="viewType">Type of the view being prepared.</param>
        /// <param name="view">The view instance to prepare.</param>
        /// <returns>True if the view was successfully prepared; otherwise, false.</returns>
        private bool PrepareView(Type viewType, BaseView view)
        {
            if (view == null)
            {
                Debug.LogError($"{viewType} not found or failed to initialize.");
                return false;
            }

            views[viewType] = view;
            view.Initialize(this);
            return true;
        }
    }
}
