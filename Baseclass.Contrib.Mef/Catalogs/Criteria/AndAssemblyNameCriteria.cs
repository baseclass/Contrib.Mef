namespace Baseclass.Contrib.Mef.Catalogs.Criteria
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The criteria responsible to filter assemblies which meet two criteria.
    /// </summary>
    public class AndAssemblyNameCriteria : IAssemblyNameCriteria
    {
        #region Fields

        /// <summary>
        /// The first criteria which needs to be met.
        /// </summary>
        private readonly IAssemblyNameCriteria firstCriteria;

        /// <summary>
        /// The second criteria which needs to be met.
        /// </summary>
        private readonly IAssemblyNameCriteria secondCriteria;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AndAssemblyNameCriteria"/> class.
        /// </summary>
        /// <param name="firstCriteria">
        /// The first criteria which needs to be met.
        /// </param>
        /// <param name="secondCriteria">
        /// The second criteria which needs to be met.
        /// </param>
        public AndAssemblyNameCriteria(IAssemblyNameCriteria firstCriteria, 
                                       IAssemblyNameCriteria secondCriteria)
        {
            this.firstCriteria = firstCriteria;
            this.secondCriteria = secondCriteria;
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
            var meetingFirstCriteria = this.firstCriteria.MeetCriteria(assemblyNames);

            return this.secondCriteria.MeetCriteria(meetingFirstCriteria);
        }

        #endregion
    }
}