using System.Collections.Generic;

namespace Rainbow.ObjectFlow.Engine
{
    internal class TaskList<T>
    {
        internal IList<OperationDuplex<T>> Tasks;       

        public TaskList()
        {
            Tasks = new List<OperationDuplex<T>>();
        }
    }
}