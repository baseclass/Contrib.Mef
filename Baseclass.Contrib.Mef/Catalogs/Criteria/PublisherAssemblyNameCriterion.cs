namespace Baseclass.Contrib.Mef.Catalogs.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;

    using Mono.Security.Authenticode;

    /// <summary>
    /// The criteria responsible to filter assemblies which meet a specific public key.
    /// </summary>
    public class PublisherAssemblyNameCriterion : IAssemblyNameCriteria
    {
        #region Fields

        /// <summary>
        /// The publisher which should be met by the assemblies.
        /// </summary>
        private readonly byte[] publisherPublicKey;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherAssemblyNameCriterion"/> class.
        /// </summary>
        /// <param name="publisher">
        /// The publisher.
        /// </param>
        public PublisherAssemblyNameCriterion(Publisher publisher)
        {
            this.publisherPublicKey = publisher != null
                                          ? publisher.Certificate.GetPublicKey()
                                          : null;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Filters an enumerable of <see cref="IEnumerable{AssemblyName}"/>s.
        /// </summary>
        /// <param name="assemblyNames">
        /// The enumerable of <see cref="AssemblyName"/> which should be filtered.
        /// </param>
        /// <returns>
        /// The enumerable of <see cref="AssemblyName"/> where the <see cref="publisherPublicKey"/> is the same.
        /// If <see cref="publisherPublicKey"/> is null, every assembly is returned.
        /// </returns>
        public IEnumerable<AssemblyName> MeetCriteria(IEnumerable<AssemblyName> assemblyNames)
        {
            if (this.publisherPublicKey == null)
            {
                return assemblyNames;
            }

            var assemblyNamesMeetingCriteria = assemblyNames.Where(this.HasSamePublisher);

            return assemblyNamesMeetingCriteria;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the <paramref name="assemblyName"/> has the same publisher public key as <see cref="publisherPublicKey"/>.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name where to compare it's publisher against <see cref="publisherPublicKey"/>.
        /// </param>
        /// <returns>
        /// <c>True</c> if the <paramref name="assemblyName"/>'s publisher public key equals to the <see cref="publisherPublicKey"/>.
        /// </returns>
        private bool HasSamePublisher(AssemblyName assemblyName)
        {
            var path = new Uri(assemblyName.CodeBase).LocalPath;

            var authenticodeDeformatter = new AuthenticodeDeformatter(path);

            if (authenticodeDeformatter.SigningCertificate == null)
            {
                return false;
            }

            var assemblyPublisherPublicKey = authenticodeDeformatter.SigningCertificate.PublicKey;

            return assemblyPublisherPublicKey.SequenceEqual(this.publisherPublicKey);
        }

        #endregion
    }
}