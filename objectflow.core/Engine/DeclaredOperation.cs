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
        private readonly IList _tasks;
        private readonly int _taskOffset;

        public DeclaredOperation(IList tasks) {
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
