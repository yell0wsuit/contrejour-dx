using Xunit.Sdk;
using Xunit.v3;

// The logging seam and the crash dialog switch are process-wide statics that tests set and clear,
// and console capture swaps Console.Out, so test classes must not run in parallel.
[assembly: Parallelization(Mode = ParallelMode.None)]
