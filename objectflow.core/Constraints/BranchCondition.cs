using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Constraints
{
    internal class BranchCondition : Condition
    {
        public virtual IDeclaredOperation BranchPoint { get; private set; }
        private readonly Action _alterTaskChainForBranch;

        /// <summary>
        /// Constructor used to define a path
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="operation"></param>
        /// <param name="branchPoint"></param>
        public BranchCondition(Func<bool> condition, IDeclaredOperation branchPoint)
            : base(condition)
        {
            BranchPoint = branchPoint;
        }

        /// <summary>
        /// Copy constructor. Used to execute the path.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="alterTaskChainForBranch">Action to execute when match fails</param>
        internal BranchCondition(BranchCondition @object, Action alterTaskChainForBranch)
            : this (@object._condition, @object.BranchPoint)
        {
            _alterTaskChainForBranch = alterTaskChainForBranch;
        }

        public override bool Matches()
        {
            if (!base.Matches())
            {
                if (_alterTaskChainForBranch != null)
                    _alterTaskChainForBranch();
                return false;
            }
            else return true;
        }

        public override bool Matches(bool match)
        {
            if (!base.Matches(match))
            {
                if (_alterTaskChainForBranch != null)
                    _alterTaskChainForBranch();
                return false;
            }
            else return true;
        }
    }
}
