using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rainbow.ObjectFlow.Framework;

namespace Objectflow.core.tests
{
    [TestFixture]
    public class WhenConfiguringListWorkflows
    {
        private Workflow<IEnumerable<string>> _pipe;

        [SetUp]
        public void BeforeEachTest()
        {
            _pipe = new Workflow<IEnumerable<string>>();
        }

        [Test]
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

            Assert.That(result, Is.InstanceOf<IEnumerable<string>>());
            Assert.That(((IList)result)[0].ToString().Contains("Rainbow rhyme member"));
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
