namespace Baseclass.Contrib.Mef.Catalogs.Criteria
{
    /// <summary>
    /// Extension methods for <see cref="IAssemblyNameCriteria"/>.
    /// </summary>
    public static class AssemblyNameCriteriaExtensions
    {
        /// <summary>
        /// Creates a combined criteria which consists of two criteria.
        /// </summary>
        /// <param name="firstCriteria">
        /// The first criteria which needs to be met.
        /// </param>
        /// <param name="secondCriteria">
        /// The second criteria which needs to be met.
        /// </param>
        /// <returns>
        /// A combined criteria which consists of two criteria.
        /// </returns>
        public static IAssemblyNameCriteria And(this IAssemblyNameCriteria firstCriteria,
                                                IAssemblyNameCriteria secondCriteria)
        {
            return new AndAssemblyNameCriteria(firstCriteria, secondCriteria);
        }
    }
}