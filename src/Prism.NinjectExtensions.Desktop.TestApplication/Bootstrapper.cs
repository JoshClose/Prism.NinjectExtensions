using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.NinjectExtensions;
using Ninject;
using Prism.NinjectExtensions.Desktop.TestApplication.Views;

namespace Prism.NinjectExtensions.Desktop.TestApplication
{
	public class Bootstrapper : NinjectBootstrapper
	{
		protected override DependencyObject CreateShell()
		{
			return Kernel.Get<MainWindow>();
		}

		protected override void ConfigureKernel()
		{
			base.ConfigureKernel();
		}

		protected override void InitializeShell()
		{
			base.InitializeShell();
			App.Current.MainWindow = (Window)Shell;
			App.Current.MainWindow.Show();
		}
	}
}
