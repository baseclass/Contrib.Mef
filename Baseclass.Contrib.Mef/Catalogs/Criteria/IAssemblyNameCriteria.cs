namespace Baseclass.Contrib.Mef.Catalogs.Criteria
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///   Defines the contract for the criteria responsible to filter assemblies for the discovery in <see cref="AssemblyFilteredDirectoryCatalog"/>.
    /// </summary>
    public interface IAssemblyNameCriteria
    {
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
        IEnumerable<AssemblyName> MeetCriteria(IEnumerable<AssemblyName> assemblyNames);

        #endregion
    }
}