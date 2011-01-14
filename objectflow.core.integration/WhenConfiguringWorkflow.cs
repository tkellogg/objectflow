using objectflow.core.tests.integration.TestOperations;
using Rainbow.ObjectFlow.Framework;
using Rainbow.ObjectFlow.Helpers;
using Rainbow.ObjectFlow.Interfaces;

namespace objectflow.core.tests.integration
{
    public class WhenConfiguringWorkflow : Specification
    {
        private IOperation<Colour> _doubleSpace;
        private IOperation<Colour> _duplicateName;
        private IOperation<Colour> _secondDuplicateName;
        private Workflow<Colour> _workflow;
        private Colour _colour;
        private IOperation<Colour> _defaultLoader;

        [ScenarioAttribute]
        public void Given()
        {
            _doubleSpace = new DoubleSpace();
            _duplicateName = new DuplicateName();
            _secondDuplicateName = new DuplicateName();
            _colour = new Colour("Red");
            _defaultLoader = new WorkflowMemoryLoader<Colour>(_colour);

            _workflow = new Workflow<Colour>();
            _workflow.Do(_defaultLoader);
        }

        [Observation]
        public void ShouldExecuteSingleOperation()
        {
            _workflow.Do(_doubleSpace);

            var result = _workflow.Start();

            result.ShouldNotbeNull();
            result.ShouldBe("R e d");
        }

        [Observation]
        public void ShouldExecuteChainedOperations()
        {
            _workflow.Do(_duplicateName).Do(_doubleSpace);

            var result = _workflow.Start();

            result.ShouldBe("R e d R e d");
        }

        [Observation]
        public void ShouldNotExecuteFalseConditionalOperations()
        {
            _workflow.Do(_duplicateName).Do(_doubleSpace, If.IsTrue(false));

            var result = _workflow.Start();

            result.ShouldBe("RedRed");
        }

        [Observation]
        public void ShouldExecuteTrueConditionalOperations()
        {
            _workflow.Do(_duplicateName).Do(_doubleSpace, If.IsTrue(true));

            var result = _workflow.Start();

            result.ShouldBe("R e d R e d");
        }

        [Observation]
        public void ShouldChainNotOperationsCorrectly()
        {
            _workflow
                .Do(_duplicateName)
                .Do(_doubleSpace, If.Not.Successfull(_duplicateName))
                .Do(_secondDuplicateName, If.Successfull(_duplicateName));

            var result = _workflow.Start();

            result.ShouldBe("RedRedRedRed");
        }

        [Observation]
        public void ShouldBeAbleToAssignMultipleConstraintsToOperationExecutionClause()
        {
            _workflow
                .Do(_duplicateName)
                .Do(_doubleSpace, If.Not.Successfull(_duplicateName))
                .Do(_duplicateName, If.Successfull(_duplicateName));

            var result = _workflow.Start();

            result.ShouldBe("RedRedRedRed");
        }

        [Observation]
        public void ShouldHandleMultipleTypeReuseWithConstraints()
        {
            _workflow
                .Do(_duplicateName)
                .Do(_duplicateName, If.Not.Successfull(_duplicateName))
                .Do(_duplicateName, If.Successfull(_duplicateName))
                .Do(_duplicateName, If.Successfull(_duplicateName));

            var result = _workflow.Start();

            result.ShouldBe("RedRedRedRedRedRedRedRed");
        }

        [Observation]
        public void ShouldAddOperationRegisteredByType()
        {
            string result =
                Workflow<Colour>.Definition()
                    .Do<DuplicateName>()
                    .Do<DuplicateName>(If.IsTrue(true))
                    .Start(new Colour("Red")).ToString();

            result.ShouldBe("RedRedRedRed");
        }
    }
}