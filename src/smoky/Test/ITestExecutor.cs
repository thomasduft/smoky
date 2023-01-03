using System.Threading;
using System.Threading.Tasks;

namespace tomware.Smoky;

internal interface ITestExcecutor
{
  Task<TestResult> ExecuteAsync(string domain, CancellationToken cancellationToken);
}
