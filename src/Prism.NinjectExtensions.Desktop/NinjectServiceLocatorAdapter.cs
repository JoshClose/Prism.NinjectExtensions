using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Ninject;

namespace Microsoft.Practices.Prism.NinjectExtensions
{
	public class NinjectServiceLocatorAdapter : ServiceLocatorImplBase
	{
		private readonly IKernel kernel;

		public NinjectServiceLocatorAdapter( IKernel kernel )
		{
			this.kernel = kernel;
		}

		protected override object DoGetInstance( Type serviceType, string key )
		{
			return kernel.Get( serviceType, key );
		}

		protected override IEnumerable<object> DoGetAllInstances( Type serviceType )
		{
			return kernel.GetAll( serviceType );
		}
	}
}
