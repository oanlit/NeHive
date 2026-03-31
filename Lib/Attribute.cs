namespace Lib;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StoreAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class NoSignalAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class MemoAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class NoBatchAttribute : Attribute
{
}