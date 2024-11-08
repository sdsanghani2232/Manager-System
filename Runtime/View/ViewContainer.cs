using System;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Managers.UI
{
    /// <summary>
    /// Abstract base class for containers managing a collection of views, providing methods to generate and destroy views.
    /// </summary>
#if !ODIN_INSPECTOR
    public abstract class ViewContainer : ScriptableObject
#else
    public abstract class ViewContainer : SerializedScriptableObject
#endif
    {
        #region Serialized Fields

        [SerializeField, Tooltip("List of view items.")]
        protected List<ResourceViewItem> views = new List<ResourceViewItem>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a view based on its type.
        /// </summary>
        /// <param name="uiManager">UI Manager instance.</param>
        /// <param name="result">Callback invoked with the generated view.</param>
        /// <typeparam name="T">Type of view to generate.</typeparam>
        public abstract void GenerateView<T>(UIManager uiManager, Action<BaseView> result);

        /// <summary>
        /// Generates a view based on its type.
        /// </summary>
        /// <param name="viewType">Type of view.</param>
        /// <param name="uiManager">UI Manager instance.</param>
        /// <param name="result">Callback invoked with the generated view.</param>
        public abstract void GenerateView(Type viewType, UIManager uiManager, Action<BaseView> result);

        /// <summary>
        /// Destroys the specified view.
        /// </summary>
        /// <param name="view">The view to destroy.</param>
        public abstract void DestroyView(BaseView view);

        /// <summary>
        /// Checks if the container has a view of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of view to check.</typeparam>
        /// <returns>True if the view is in the container, false otherwise.</returns>
        public abstract bool HasView<T>();

        /// <summary>
        /// Checks if the container has a view of the specified type.
        /// </summary>
        /// <param name="viewType">Type of view to check.</param>
        /// <returns>True if the view is in the container, false otherwise.</returns>
        public abstract bool HasView(Type viewType);

        #endregion
    }
}
