using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;
using System.Collections;

namespace Rainbow.ObjectFlow.Engine
{
    internal class DeclaredOperation : IDeclaredOperation
    {
        private IList _tasks;
        private int _taskOffset;

        /// <summary>
        /// Set the remaining tasks
        /// </summary>
        /// <param name="tasks"></param>
        public virtual void SetTasks(IList tasks)
        {
            if (_tasks != null)
                throw new InvalidOperationException("Redefinition of declared operations is not allowed");
            _tasks = tasks;
            _taskOffset = tasks.Count;
        }

        /// <summary>
        /// Executing the operations returned from this method is equivalent to branching
        /// down this path
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable GetRemainingOperations()
        {
            for (int i = _taskOffset; i < _tasks.Count; i++)
                yield return _tasks[i];
        }
    }
}
