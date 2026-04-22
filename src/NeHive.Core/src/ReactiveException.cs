namespace NeHive.Core;

internal sealed class InfiniteReactiveLoopException(string? message) : Exception(message)
{
}