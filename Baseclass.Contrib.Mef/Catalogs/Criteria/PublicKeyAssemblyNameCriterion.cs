namespace Baseclass.Contrib.Mef.Catalogs.Criteria
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The criteria responsible to filter assemblies which meet a specific public key.
    /// </summary>
    public class PublicKeyAssemblyNameCriterion : IAssemblyNameCriteria
    {
        #region Fields

        /// <summary>
        /// The public key which should be met by the assemblies.
        /// </summary>
        private readonly byte[] publicKey;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicKeyAssemblyNameCriterion"/> class.
        /// </summary>
        /// <param name="publicKey">
        /// The public key .
        /// </param>
        public PublicKeyAssemblyNameCriterion(byte[] publicKey)
        {
            this.publicKey = publicKey;
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
        /// The enumerable of <see cref="AssemblyName"/> which meets this criteria.
        /// </returns>
        public IEnumerable<AssemblyName> MeetCriteria(IEnumerable<AssemblyName> assemblyNames)
        {
            return assemblyNames.Where(this.HasSamePublicKey);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the <paramref name="assemblyName"/> has the same publickey as <see cref="publicKey"/>.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name which's public key should be compared to the <see cref="publicKey"/>.
        /// </param>
        /// <returns>
        /// <c>True</c> if the <paramref name="assemblyName"/>'s public key equals to the <see cref="publicKey"/>.
        /// </returns>
        private bool HasSamePublicKey(AssemblyName assemblyName)
        {
            var assemblyNamePublicKey = assemblyName.GetPublicKey() ?? new byte[0];

            return assemblyNamePublicKey.SequenceEqual(this.publicKey);
        }

        #endregion
    }
}