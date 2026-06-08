namespace NeHive.Reactive;

/// <summary>
/// Marks a class as a reactive store. Used for attribute-based store generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StoreAttribute : Attribute
{
}

/// <summary>
/// Marks a property as non-reactive. The property will not generate a signal in a store class.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NoSignalAttribute : Attribute
{
}

/// <summary>
/// Marks a property as a computed value derived from other signals.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ComputedAttribute : Attribute
{
}
/// <summary>
/// Marks a method to not be wrapped in a batch update.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class NoBatchAttribute : Attribute
{
}