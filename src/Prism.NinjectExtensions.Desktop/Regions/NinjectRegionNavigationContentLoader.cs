using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Ninject;

namespace Microsoft.Practices.Prism.NinjectExtensions.Regions
{
	public class NinjectRegionNavigationContentLoader : RegionNavigationContentLoader
	{
		private readonly IKernel kernel;

		public NinjectRegionNavigationContentLoader( IServiceLocator serviceLocator, IKernel kernel ) : base( serviceLocator )
		{
			this.kernel = kernel;
		}

		[SuppressMessage( "ReSharper", "PossibleMultipleEnumeration" )]
		protected override IEnumerable<object> GetCandidatesFromRegion( IRegion region, string candidateNavigationContract )
		{
			if( candidateNavigationContract == null || candidateNavigationContract.Equals( string.Empty ) )
			{
				throw new ArgumentNullException( "candidateNavigationContract" );
			}

			var contractCandidates = base.GetCandidatesFromRegion( region, candidateNavigationContract );

			if( !contractCandidates.Any() )
			{
				//First try friendly name registration. If not found, try type registration
				var matchingRegistration = kernel.GetBindings().FirstOrDefault( b => candidateNavigationContract.Equals( b.Metadata.Name, StringComparison.Ordinal ) );
				if( matchingRegistration == null )
				{
					matchingRegistration = kernel.GetBindings().FirstOrDefault( b => candidateNavigationContract.Equals( kernel.GetBindingTargetType( b ).Name, StringComparison.Ordinal ) );
				}

				if( matchingRegistration == null )
				{
					return new object[0];
				}

				var typeCandidateName = kernel.GetBindingTargetType( matchingRegistration ).FullName;

				contractCandidates = base.GetCandidatesFromRegion( region, typeCandidateName );
			}

			return contractCandidates;
		}
	}
}
