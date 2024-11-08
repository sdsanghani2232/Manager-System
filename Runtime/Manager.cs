using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;    
#endif

namespace Mangers
{
    /// <summary>
    ///  Manager class that manages singleton instances of game managers.
    /// </summary>
#if !ODIN_INSPECTOR
    public abstract class Manager : MonoBehaviour
#else
    public class Manager : SerializedMonoBehaviour
#endif
    {
        #region Prviate varible
        /// <summary>
        /// Static dictionary to hold references to each manager instance
        /// </summary>
        private static Dictionary<Type, Manager> m_Managers;
        
        /// <summary>
        /// LockObject for thread safety to ensure only one thread initializes m_Managers
        /// </summary>
        private static readonly object LockObject = new object();
        
        private static Dictionary<Type, Manager> Managers
        {
            get
            {
                if (m_Managers != null) return m_Managers;
                lock (LockObject)
                {
                    m_Managers ??= new Dictionary<Type, Manager>();
                }
                return m_Managers;
            }
        }
        #endregion
        
        #region Unity Callbacks
        public virtual void Awake()
        {
            // Ensure only one instance of each manager type exists
            
            if (!Managers.ContainsKey(GetType())) 
                Managers.Add(GetType(), this);
            else
                Debug.LogError($"Duplicate manager detected for type {GetType()}.");
        }
        
        public virtual void OnDestroy()
        {
            // Remove this manager from the dictionary and clear static references
            
            if (Managers == null || !Managers.ContainsKey(GetType())) return;
            Managers.Remove(GetType());
            ClearAllStaticReferences(GetType());
        }
        #endregion
        
        #region Private Methods

        /// <summary>
        /// Clear all static references of a specific type recursively in base classes.
        /// </summary>
        /// <param name="type">The type of the manager to clear</param>
        private static void ClearAllStaticReferences(Type type)
        {
            while (true)
            {
                if (type == null) return;

                // Use reflection to clear static fields of the given type
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
                foreach (var field in fields)
                {
                    if (field.FieldType == type) field.SetValue(null, null);
                }

                // Recursively clear base type static references
                type = type.BaseType;
            }
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// Get a manager instance of a specified type.
        /// </summary>
        /// <typeparam name="T">Type of manager</typeparam>
        /// <returns>Manager instance of specified type, or null if not found</returns>
        
        public static T GetManager<T>() where T : Manager
        {
            return Managers.TryGetValue(typeof(T), out var manager) ? (T)manager : null;
        }
        #endregion
    }
}