using System;
using System.Linq;
using Microsoft.Practices.Prism.NinjectExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace Prism.NinjectExtensions.Desktop.Tests
{
	[TestClass]
	public class NinjectExtensionsTests
	{
		[TestMethod]
		public void GetBindingTargetTypeToTypeTest()
		{
			var kernel = new StandardKernel();
			kernel.Bind<ITest>().To<Test>();

			var binding = kernel.GetBindings( typeof( ITest ) ).ElementAt( 0 );
			var type = kernel.GetBindingTargetType( binding );

			Assert.AreEqual( typeof( Test ), type );
		}

		[TestMethod]
		public void GetBindingTargetTypeToConstantTest()
		{
			var kernel = new StandardKernel();
			kernel.Bind<ITest>().ToConstant( new Test() );

			var binding = kernel.GetBindings( typeof( ITest ) ).ElementAt( 0 );
			var type = kernel.GetBindingTargetType( binding );

			Assert.AreEqual( typeof( Test ), type );
		}

		[TestMethod]
		public void GetBindingTargetTypeToMethodTest()
		{
			var kernel = new StandardKernel();
			kernel.Bind<ITest>().ToMethod( ctx => new Test() );

			var binding = kernel.GetBindings( typeof( ITest ) ).ElementAt( 0 );
			var type = kernel.GetBindingTargetType( binding );

			Assert.AreEqual( typeof( Test ), type );
		}

		[TestMethod]
		public void GetBindingTargetTypeToSelfTest()
		{
			var kernel = new StandardKernel();
			kernel.Bind<Test>().ToSelf();

			var binding = kernel.GetBindings( typeof( Test ) ).ElementAt( 0 );
			var type = kernel.GetBindingTargetType( binding );

			Assert.AreEqual( typeof( Test ), type );
		}

		[TestMethod]
		public void GetBindingsTest()
		{
			var kernel = new StandardKernel();
			kernel.Bind<ITest>().To<Test>();
			kernel.Bind<ITest>().To<Test>().Named( "Test" );

			var bindings = kernel.GetBindings();
			Assert.AreEqual( 4, bindings.Count() );
		}

		private interface ITest
		{
		}

		private class Test : ITest
		{
		}
	}
}
