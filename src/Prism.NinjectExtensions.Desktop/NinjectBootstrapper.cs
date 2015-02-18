using System;
using System.Globalization;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.NinjectExtensions.Properties;
using Microsoft.Practices.Prism.NinjectExtensions.Regions;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Ninject;
using ActivationException = Ninject.ActivationException;

namespace Microsoft.Practices.Prism.NinjectExtensions
{
	public abstract class NinjectBootstrapper : Bootstrapper
	{
		private bool useDefaultConfiguration = true;

		public IKernel Kernel { get; protected set; }

		public override void Run( bool runWithDefaultConfiguration )
		{
			useDefaultConfiguration = runWithDefaultConfiguration;

			Logger = CreateLogger();
			if( Logger == null )
			{
				throw new InvalidOperationException( Resources.NullLoggerFacadeException );
			}

			Logger.Log( Resources.LoggerCreatedSuccessfully, Category.Debug, Priority.Low );

			Logger.Log( Resources.CreatingModuleCatalog, Category.Debug, Priority.Low );
			ModuleCatalog = this.CreateModuleCatalog();
			if( ModuleCatalog == null )
			{
				throw new InvalidOperationException( Resources.NullModuleCatalogException );
			}

			Logger.Log( Resources.ConfiguringModuleCatalog, Category.Debug, Priority.Low );
			ConfigureModuleCatalog();

			Logger.Log( Resources.CreatingNinjectKernel, Category.Debug, Priority.Low );
			Kernel = CreateKernel();
			if( Kernel == null )
			{
				throw new InvalidOperationException( Resources.NullNinjectKernelException );
			}

			Logger.Log( Resources.ConfiguringNinjectKernel, Category.Debug, Priority.Low );
			ConfigureKernel();

			Logger.Log( Resources.ConfiguringServiceLocatorSingleton, Category.Debug, Priority.Low );
			ConfigureServiceLocator();

			Logger.Log( Resources.ConfiguringRegionAdapters, Category.Debug, Priority.Low );
			ConfigureRegionAdapterMappings();

			Logger.Log( Resources.ConfiguringDefaultRegionBehaviors, Category.Debug, Priority.Low );
			ConfigureDefaultRegionBehaviors();

			Logger.Log( Resources.RegisteringFrameworkExceptionTypes, Category.Debug, Priority.Low );
			RegisterFrameworkExceptionTypes();

			Logger.Log( Resources.CreatingShell, Category.Debug, Priority.Low );
			Shell = this.CreateShell();
			if( Shell != null )
			{
				Logger.Log( Resources.SettingTheRegionManager, Category.Debug, Priority.Low );
				RegionManager.SetRegionManager( this.Shell, Kernel.Get<IRegionManager>() );

				Logger.Log( Resources.UpdatingRegions, Category.Debug, Priority.Low );
				RegionManager.UpdateRegions();

				Logger.Log( Resources.InitializingShell, Category.Debug, Priority.Low );
				InitializeShell();
			}

			if( Kernel.CanResolve<IModuleManager>() )
			{
				Logger.Log( Resources.InitializingModules, Category.Debug, Priority.Low );
				InitializeModules();
			}

			Logger.Log( Resources.BootstrapperSequenceCompleted, Category.Debug, Priority.Low );
		}

		protected override void ConfigureServiceLocator()
		{
			ServiceLocator.SetLocatorProvider( () => Kernel.Get<IServiceLocator>() );
		}

		protected override void RegisterFrameworkExceptionTypes()
		{
			base.RegisterFrameworkExceptionTypes();

			ExceptionExtensions.RegisterFrameworkExceptionType( typeof( ActivationException ) );
		}

		protected virtual void ConfigureKernel()
		{
			Kernel.Bind<ILoggerFacade>().ToConstant( Logger );
			Kernel.Bind<IModuleCatalog>().ToConstant( ModuleCatalog );

			if( useDefaultConfiguration )
			{
				RegisterTypeIfMissing( typeof( IServiceLocator ), typeof( NinjectServiceLocatorAdapter ), true );
				RegisterTypeIfMissing( typeof( IModuleInitializer ), typeof( ModuleInitializer ), true );
				RegisterTypeIfMissing( typeof( IModuleManager ), typeof( ModuleManager ), true );
				RegisterTypeIfMissing( typeof( RegionAdapterMappings ), typeof( RegionAdapterMappings ), true );
				RegisterTypeIfMissing( typeof( IRegionManager ), typeof( RegionManager ), true );
				RegisterTypeIfMissing( typeof( IEventAggregator ), typeof( EventAggregator ), true );
				RegisterTypeIfMissing( typeof( IRegionViewRegistry ), typeof( RegionViewRegistry ), true );
				RegisterTypeIfMissing( typeof( IRegionBehaviorFactory ), typeof( RegionBehaviorFactory ), true );
				RegisterTypeIfMissing( typeof( IRegionNavigationJournalEntry ), typeof( RegionNavigationJournalEntry ), false );
				RegisterTypeIfMissing( typeof( IRegionNavigationJournal ), typeof( RegionNavigationJournal ), false );
				RegisterTypeIfMissing( typeof( IRegionNavigationService ), typeof( RegionNavigationService ), false );
				RegisterTypeIfMissing( typeof( IRegionNavigationContentLoader ), typeof( NinjectRegionNavigationContentLoader ), true );
			}
		}

		protected override void InitializeModules()
		{
			IModuleManager manager;

			try
			{
				manager = Kernel.Get<IModuleManager>();
			}
			catch( ActivationException ex )
			{
				if( ex.Message.Contains( "IModuleCatalog" ) )
				{
					throw new InvalidOperationException( Resources.NullModuleCatalogException );
				}

				throw;
			}

			manager.Run();
		}

		protected virtual IKernel CreateKernel()
		{
			return new StandardKernel();
		}

		protected void RegisterTypeIfMissing( Type fromType, Type toType, bool registerAsSingleton )
		{
			if( fromType == null )
			{
				throw new ArgumentNullException( "fromType" );
			}

			if( toType == null )
			{
				throw new ArgumentNullException( "toType" );
			}

			if( (bool)Kernel.CanResolve( fromType ) )
			{
				Logger.Log( String.Format( CultureInfo.CurrentCulture, Resources.TypeMappingAlreadyRegistered, fromType.Name ), Category.Debug, Priority.Low );
			}
			else
			{
				if( registerAsSingleton )
				{
					Kernel.Bind( fromType ).To( toType ).InSingletonScope();
				}
				else
				{
					Kernel.Bind( fromType ).To( toType );
				}
			}
		}
	}
}
