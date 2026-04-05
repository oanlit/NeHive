namespace Lib;

internal sealed class InfiniteReactiveLoopException(string? message) : Exception(message)
{
}