using System;

namespace System.Runtime.CompilerServices
{
	// This attribute allows us to define extension methods without requiring FW 3.5.

	[AttributeUsage (AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
	public sealed class ExtensionAttribute : Attribute { }
}
