using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Mangers.UI
{
    /// <summary>
    /// Base class for UI views, providing functionality to show, hide, and initialize views.
    /// Handles view lifecycle management including destruction on hide, if specified.
    /// </summary>
    public class BaseView : MonoBehaviour
    {
        #region Serialize Fields

        [SerializeField, Tooltip("If true, the view will be destroyed when hidden.")]
        private bool destroyOnHide;

        #endregion

        #region Properties

        private GameObject m_GameObject;

        /// <summary>
        /// Cached reference to the view's GameObject.
        /// </summary>
        protected GameObject GameObject
        {
            get
            {
                if (!m_GameObject) m_GameObject = gameObject;
                return m_GameObject;
            }
            set => m_GameObject = value;
        }

        /// <summary>
        /// Indicates if the view should be destroyed when hidden.
        /// </summary>
        public bool DestroyOnHide => destroyOnHide;

        /// <summary>
        /// Reference to the UIManager managing this view.
        /// </summary>
        public UIManager ContextUIManager { get; private set; }

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Unity's Awake callback. Caches the GameObject reference on initialization.
        /// </summary>
        public virtual void Awake()
        {
            m_GameObject = gameObject;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the view with the specified UIManager.
        /// </summary>
        /// <param name="uiManager">The UIManager managing this view.</param>
        public virtual void Initialize(UIManager uiManager)
        {
            ContextUIManager = uiManager;
        }

        /// <summary>
        /// Shows the view, making it active in the hierarchy.
        /// </summary>
        public virtual void Show()
        {
            ShowViewContent();
        }

        /// <summary>
        /// Shows the view with optional parameters.
        /// </summary>
        /// <param name="args">Optional parameters for view setup.</param>
        public virtual void Show(params object[] args)
        {
            ShowViewContent();
        }

        /// <summary>
        /// Shows the view and registers callbacks for show and hide events.
        /// </summary>
        /// <param name="showCallback">Callback to invoke when the view is shown.</param>
        /// <param name="hideCallback">Callback to invoke when the view is hidden.</param>
        public virtual void Show(Action showCallback, Action hideCallback)
        {
            ShowViewContent();
        }

        /// <summary>
        /// Shows the view with callbacks and optional parameters.
        /// </summary>
        /// <param name="showCallback">Callback to invoke when the view is shown.</param>
        /// <param name="hideCallback">Callback to invoke when the view is hidden.</param>
        /// <param name="args">Optional parameters for view setup.</param>
        public virtual void Show(Action showCallback, Action hideCallback, params object[] args)
        {
            ShowViewContent();
        }

        /// <summary>
        /// Hides the view, making it inactive in the hierarchy. If <c>destroyOnHide</c> is true,
        /// removes the view from UIManager and destroys it.
        /// </summary>
        public virtual void Hide()
        {
            HideViewContent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Activates the view's GameObject, showing the view.
        /// </summary>
        private void ShowViewContent()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Deactivates the view's GameObject, hiding the view. Destroys the view if <c>destroyOnHide</c> is true.
        /// </summary>
        private void HideViewContent()
        {
            m_GameObject.SetActive(false);
            if (destroyOnHide && ContextUIManager)
            {
                ContextUIManager.RemoveView(GetType());
                ContextUIManager.ViewContainer.DestroyView(this);
            }
        }

        #endregion
    }
}
