namespace Baseclass.Contrib.Mef.Catalogs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Baseclass.Contrib.Mef.Catalogs.Criteria;

    /// <summary>
    ///     Discovers attributed parts in the assemblies in a specified directory.
    /// </summary>
    public class AssemblyFilteredDirectoryCatalog : ComposablePartCatalog, 
        INotifyComposablePartCatalogChanged, 
        ICompositionElement
    {
        #region Static Fields

        /// <summary>
        ///     The default assembly name criteria.
        /// </summary>
        private static readonly IAssemblyNameCriteria DefaultAssemblyNameCriteria =
            new AcceptingAllAssemblyNameCriteria();

        #endregion

        #region Fields

        /// <summary>
        ///     The <see cref="IAssemblyNameCriteria" /> which needs to be met by discovered assemblies.
        /// </summary>
        private readonly IAssemblyNameCriteria assemblyNameCriteria;

        /// <summary>
        ///     The path.
        /// </summary>
        private readonly string path;

        /// <summary>
        ///     The search pattern.
        /// </summary>
        private readonly string searchPattern;

        /// <summary>
        ///     The aggregate catalog.
        /// </summary>
        private AggregateCatalog aggregateCatalog;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFilteredDirectoryCatalog"/> class with
        ///     <see cref="ComposablePartDefinition"/> objects based on all the DLL files in the specified directory path and its
        ///     sub-directories.
        /// </summary>
        /// <param name="path">
        /// Path to the directory to scan for assemblies to add to the catalog.
        /// </param>
        public AssemblyFilteredDirectoryCatalog(string path)
            : this(path, "*.dll", DefaultAssemblyNameCriteria)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyFilteredDirectoryCatalog"/> class with
        ///     <see cref="ComposablePartDefinition"/> objects based on the specified search pattern in the specified directory
        ///     path path and its sub-directories.
        /// </summary>
        /// <param name="path">
        /// Path to the directory to scan for assemblies to add to the catalog.
        /// </param>
        /// <param name="searchPattern">
        /// The pattern to search with. The format of the pattern should be the same as specified for
        ///     GetFiles.
        /// </param>
        /// <param name="assemblyNameCriteria">
        /// The criteria which needs to be met by the found assemblies.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The value of the <paramref name="path"/> parameter was <see langword="null"/>
        ///     .
        /// </exception>
        public AssemblyFilteredDirectoryCatalog(
            string path, 
            string searchPattern, 
            IAssemblyNameCriteria assemblyNameCriteria)
        {
            this.path = path;

            this.searchPattern = searchPattern;

            this.assemblyNameCriteria = assemblyNameCriteria;

            this.Initialize();
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when the contents of the catalog has changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        /// <summary>
        ///     Occurs when the catalog is changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the part definitions that are contained in the recursive directory catalog.
        /// </summary>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return this.aggregateCatalog.Parts;
            }
        }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        ///     Gets the display name of the directory catalog.
        /// </summary>
        string ICompositionElement.DisplayName
        {
            get
            {
                return string.Format(
                    CultureInfo.CurrentCulture, 
                    "{0} (Path=\"{1}\")", 
                    new object[] { this.GetType().Name, this.path });
            }
        }

        /// <summary>
        ///     Gets the composition element from which the directory catalog originated.
        /// </summary>
        ICompositionElement ICompositionElement.Origin
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return ((ICompositionElement)this).DisplayName;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The create aggregate catalog.
        /// </summary>
        private void CreateAggregateCatalog()
        {
            this.aggregateCatalog = new AggregateCatalog();
            this.aggregateCatalog.Changed += (o, e) =>
            {
                if (this.Changed != null)
                {
                    this.Changed(o, e);
                }
            };
            this.aggregateCatalog.Changing += (o, e) =>
            {
                if (this.Changing != null)
                {
                    this.Changing(o, e);
                }
            };
        }

        /// <summary>
        ///     The get files.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" /> array.
        /// </returns>
        private IEnumerable<string> GetFiles()
        {
            return Array.ConvertAll(Directory.GetFiles(this.path, this.searchPattern), file => file.ToUpperInvariant());
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        private void Initialize()
        {
            IEnumerable<AssemblyName> assemblyNames = this.GetFiles().Select(AssemblyName.GetAssemblyName);

            IEnumerable<AssemblyName> assemblyNamesMeetingCriteria =
                this.assemblyNameCriteria.MeetCriteria(assemblyNames);

            List<AssemblyCatalog> assemblyCatalogs =
                assemblyNamesMeetingCriteria.Select(Assembly.Load)
                    .Select(assembly => new AssemblyCatalog(assembly))
                    .ToList();

            this.CreateAggregateCatalog();

            foreach (AssemblyCatalog catalog in assemblyCatalogs)
            {
                this.aggregateCatalog.Catalogs.Add(catalog);
            }
        }

        #endregion

        /// <summary>
        ///     An implementation of IAssemblyCriteria which matches every assembly passed to <see cref="MeetCriteria" />.
        /// </summary>
        private class AcceptingAllAssemblyNameCriteria : IAssemblyNameCriteria
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
            public IEnumerable<AssemblyName> MeetCriteria(IEnumerable<AssemblyName> assemblyNames)
            {
                return assemblyNames;
            }

            #endregion
        }
    }
}