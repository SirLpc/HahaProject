using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace thelab.core
{
    
    /// <summary>
    /// Defines a class that can execute Update methods.
    /// </summary>
    public interface IUpdateable {  
        /// <summary>
        /// Callback
        /// </summary>
        /// <returns></returns>
        void OnUpdate(); 
    }

    /// <summary>
    /// Defines a class that can execute LateUpdate methods.
    /// </summary>
    public interface ILateUpdateable { 
        /// <summary>
        /// Callback
        /// </summary>
        /// <returns></returns>
        void OnLateUpdate(); 
    }

    /// <summary>
    /// Defines a class that can execute FixedUpdate methods.
    /// </summary>
    public interface IFixedUpdateable  { 
        /// <summary>
        /// Callback
        /// </summary>
        /// <returns></returns>
        void OnFixedUpdate(); 
    }

    /// <summary>
    /// Defines a class that executes an update method while the condition is true.
    /// </summary>
    public interface IConditionalUpdateable { 
        /// <summary>
        /// Callback
        /// </summary>
        /// <returns></returns>
        bool OnConditionUpdate(); 
    }
        

}
