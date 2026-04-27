namespace NeHive.Core;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StoreAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class NoSignalAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class ComputedAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class NoBatchAttribute : Attribute
{
}