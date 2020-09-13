﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.GridData {

    /// <summary>
    /// Class representing a row in an Umbraco Grid.
    /// </summary>
    public class GridRow : GridElement {

        #region Properties

        /// <summary>
        /// Gets a reference to the parent <see cref="GridSection"/>.
        /// </summary>
        public GridSection Section { get; private set; }

        /// <summary>
        /// Gets the unique ID of the row.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the alias of the row.
        /// </summary>
        [Obsolete("Use Name instead")]
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the label of the row. Use <see cref="HasLabel"/> to check whether a label has been specified.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets whether a label has been specified for the definition of this row.
        /// </summary>
        public bool HasLabel => string.IsNullOrWhiteSpace(Label) == false;

        /// <summary>
        /// Gets the name of the row.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets an array of all areas in the row.
        /// </summary>
        public GridArea[] Areas { get; private set; }

        /// <summary>
        /// Gets a reference to the previous row.
        /// </summary>
        public GridRow PreviousRow { get; internal set; }

        /// <summary>
        /// Gets a reference to the next row.
        /// </summary>
        public GridRow NextRow { get; internal set; }

        /// <summary>
        /// Gets whether the row has any areas.
        /// </summary>
        public bool HasAreas => Areas.Length > 0;

        /// <summary>
        /// Gets the first area of the row. If the row doesn't contain any areas, this property will return <c>null</c>.
        /// </summary>
        public GridArea FirstRow => Areas.FirstOrDefault();

        /// <summary>
        /// Gets the last area of the row. If the row doesn't contain any areas, this property will return <c>null</c>.
        /// </summary>
        public GridArea LastRow => Areas.LastOrDefault();

        /// <summary>
        /// Gets whether at least one area or control within the row is valid.
        /// </summary>
        public override bool IsValid {
            get { return Areas.Any(x => x.IsValid); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">An instance of <see cref="JObject"/> representing the row.</param>
        protected GridRow(JObject obj) : base(obj) { }

        #endregion

        #region Member methods

        /// <summary>
        /// Gets an array of all nested controls. 
        /// </summary>
        public GridControl[] GetAllControls() {
            return (
                from area in Areas
                from control in area.Controls
                select control
            ).ToArray();
        }

        /// <summary>
        /// Gets an array of all nested controls with the specified editor <paramref name="alias"/>. 
        /// </summary>
        /// <param name="alias">The editor alias of controls to be returned.</param>
        public GridControl[] GetAllControls(string alias) {
            return GetAllControls(x => x.Editor.Alias == alias);
        }

        /// <summary>
        /// Gets an array of all nested controls matching the specified <paramref name="predicate"/>. 
        /// </summary>
        /// <param name="predicate">The predicate (callback function) used for comparison.</param>
        public GridControl[] GetAllControls(Func<GridControl, bool> predicate) {
            return (
                from area in Areas
                from control in area.Controls
                where predicate(control)
                select control
            ).ToArray();
        }

        /// <summary>
        /// Generates the HTML for the Grid row.
        /// </summary>
        /// <param name="helper">The <see cref="HtmlHelper"/> used for rendering the Grid row.</param>
        /// <returns>The grid row as an instance of <see cref="HtmlString"/>.</returns>
        public HtmlString GetHtml(HtmlHelper helper) {
            return GetHtml(helper, Name);
        }

        /// <summary>
        /// Generates the HTML for the Grid row.
        /// </summary>
        /// <param name="helper">The <see cref="HtmlHelper"/> used for rendering the Grid row.</param>
        /// <param name="partial">The alias or virtual path to the partial view for rendering the Grid row.</param>
        /// <returns>The grid row as an instance of <see cref="HtmlString"/>.</returns>
        public HtmlString GetHtml(HtmlHelper helper, string partial) {

            // Some input validation
            if (helper == null) throw new ArgumentNullException(nameof(helper));
            if (string.IsNullOrWhiteSpace(partial)) throw new ArgumentNullException(nameof(partial));

            // Prepend the path to the "Rows" folder if not already specified
            if (GridUtils.IsValidPartialName(partial)) {
                partial = "TypedGrid/Rows/" + partial;
            }

            // Render the partial view
            return helper.Partial(partial, this);

        }

        /// <summary>
        /// Gets a textual representation of the row - eg. to be used in Examine.
        /// </summary>
        /// <param name="context">The current grid context.</param>
        /// <returns>An instance of <see cref="System.String"/> representing the value of the row.</returns>
        public override string GetSearchableText(GridContext context) {
            return Areas.Aggregate(Environment.NewLine, (current, area) => current + area.GetSearchableText(context));
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses a row from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="section">The parent section of the row.</param>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        public static GridRow Parse(GridSection section, JObject obj) {

            // Some input validation
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            #pragma warning disable 618
            
            // Parse basic properties
            GridRow row = new GridRow(obj) {
                Section = section,
                Id = obj.GetString("id"),
                Alias = obj.GetString("alias"),
                Label = obj.GetString("label"),
                Name = obj.GetString("name")
            };

            #pragma warning restore 618

            // Parse the areas
            row.Areas = obj.GetArray("areas", x => GridArea.Parse(row, x)) ?? new GridArea[0];

            // Update "PreviousArea" and "NextArea" properties
            for (int i = 1; i < row.Areas.Length; i++) {
                row.Areas[i - 1].NextArea = row.Areas[i];
                row.Areas[i].PreviousArea = row.Areas[i - 1];
            }

            // Return the row
            return row;

        }

        #endregion

    }

}