using System;
using System.Collections;
using System.Collections.Generic;
using Rainbow.ObjectFlow.Framework;

namespace objectflow.core.tests.integration
{
    public class WhenConfiguringListWorkflows : Specification
    {        
        private Workflow<IEnumerable<string>> _pipe;

        [Scenario]
        public void Given()
        {
            _pipe = new Workflow<IEnumerable<string>>();
        }

        [Observation]
        public void ShouldExecuteListMethod()
        {
            _pipe
                .Do(new Func<IEnumerable<string>, IEnumerable<string>>(Sort))
                .Do((list) =>
                        {
                            foreach (string member in list)
                            {
                                Console.WriteLine(member);
                            }

                            return list;
                        });

            var result = _pipe.Start(new[]
                                         {
                                             "Richard",
                                             "York",
                                             "Gave",
                                             "battle",
                                             "in",
                                             "vain"
                                         });

            result.IsInstanceOf<IEnumerable<string>>();
            ((IList)result)[0].ToString().ShouldBe("Rainbow rhyme member: Richard");
        }

        private static IEnumerable<string> Sort(IEnumerable<string> arg)
        {
            IList<string> results = new List<string>();
            foreach (string s in arg)
            {
                string member;
                member = "Rainbow rhyme member: " + s;
                results.Add(member);
            }

            return results;
        }
    }
}