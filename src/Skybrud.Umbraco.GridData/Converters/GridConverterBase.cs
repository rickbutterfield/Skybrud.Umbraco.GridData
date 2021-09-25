﻿using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Interfaces;
using Skybrud.Umbraco.GridData.Rendering;
using Umbraco.Core.Models.PublishedContent;

namespace Skybrud.Umbraco.GridData.Converters {

    /// <summary>
    /// Abstract base implementation of <see cref="IGridConverter"/>.
    /// </summary>
    public abstract class GridConverterBase : IGridElementConverter {

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridControlValue"/>.
        /// </summary>
        /// <param name="control">A reference to the parent <see cref="GridControl"/>.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the control value.</param>
        /// <param name="value">The converted control value.</param>
        public virtual bool ConvertControlValue(GridControl control, JToken token, out IGridControlValue value) {
            value = null;
            return false;
        }

        /// <summary>
        /// Converts the specified <paramref name="token"/> into an instance of <see cref="IGridEditorConfig"/>.
        /// </summary>
        /// <param name="editor">A reference to the parent <see cref="GridEditor"/>.</param>
        /// <param name="token">The instance of <see cref="JToken"/> representing the editor config.</param>
        /// <param name="config">The converted editor config.</param>
        public virtual bool ConvertEditorConfig(GridEditor editor, JToken token, out IGridEditorConfig config) {
            config = null;
            return false;
        }

        /// <summary>
        /// Gets an instance <see cref="GridControlWrapper"/> for the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The control to be wrapped.</param>
        /// <param name="wrapper">The wrapper.</param>
        public virtual bool GetControlWrapper(GridControl control, out GridControlWrapper wrapper) {
            wrapper = null;
            return false;
        }
        
        /// <summary>
        /// Attempts to get the the searchable text for the specified <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="text">When this method returns, contains the searchable text for <paramref name="element"/>.</param>
        /// <returns><c>true</c> if <paramref name="element"/> was recognized by this converter; otherwise <c>false</c>.</returns>
        public virtual bool TryGetSearchableText(IPublishedElement element, out string text) {
            text = null;
            return false;
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> is contained in <paramref name="source"/> (case insensitive).
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns><c>true</c> if <paramref name="source"/> contains <paramref name="value"/>; otherwise <c>false</c>.</returns>
        protected bool ContainsIgnoreCase(string source, string value) {
            if (string.IsNullOrWhiteSpace(source)) return false;
            if (string.IsNullOrWhiteSpace(value)) return false;
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(source, value, CompareOptions.IgnoreCase) >= 0;
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> is equal <paramref name="source"/> (case insensitive).
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns><c>true</c> if <paramref name="value"/> equal to <paramref name="source"/>; otherwise <c>false</c>.</returns>
        protected bool EqualsIgnoreCase(string source, string value) {
            if (string.IsNullOrWhiteSpace(source)) return false;
            if (string.IsNullOrWhiteSpace(value)) return false;
            return source.Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }

    }

}