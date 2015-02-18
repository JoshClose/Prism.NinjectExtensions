using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Infrastructure;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;

namespace Microsoft.Practices.Prism.NinjectExtensions
{
	internal static class NinjectExtensions
	{
		internal static IEnumerable<IBinding> GetBindings( this IKernel kernel )
		{
			var bindingsField = GetField( kernel.GetType(), "bindings" );
			if( bindingsField == null )
			{
				throw new InvalidOperationException( "Ninject's underlying code has changed and broke Prism.NinjectExtensions. Please contact the library maintainer." );
			}

			var bindings = (Multimap<Type, IBinding>)bindingsField.GetValue( kernel );

			return bindings.SelectMany( b => b.Value );
		}

		internal static Type GetBindingTargetType( this IKernel kernel, IBinding binding )
		{
			var request = kernel.CreateRequest( binding.Service, metadata => true, new IParameter[0], true, false );
			var context = new Context( kernel, request, binding, kernel.Components.Get<ICache>(), kernel.Components.Get<IPlanner>(), kernel.Components.Get<IPipeline>() );
			var provider = binding.GetProvider( context );
			return provider.Type;
		}

		private static FieldInfo GetField( Type type, string name )
		{
			var field = type.GetField( name, BindingFlags.Instance | BindingFlags.NonPublic );
			if( field == null && type.BaseType != null )
			{
				return GetField( type.BaseType, name );
			}

			return field;
		}
	}
}
