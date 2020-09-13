﻿using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.GridData.Json.Converters;
using Umbraco.Web.Composing;
using Umbraco.Web.Templates;

namespace Skybrud.Umbraco.GridData.Values {

    /// <summary>
    /// Class representing the rich text value of a control.
    /// </summary>
    [JsonConverter(typeof(GridControlValueStringConverter))]
    public class GridControlRichTextValue : GridControlHtmlValue {

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="control"/> and <paramref name="token"/>.
        /// </summary>
        /// <param name="control">An instance of <see cref="GridControl"/> representing the control.</param>
        /// <param name="token">An instance of <see cref="JToken"/> representing the value of the control.</param>
        protected GridControlRichTextValue(GridControl control, JToken token) : base(control, token) {

            string html = HtmlValue.ToString();

            // TODO: Methods are obsolete from 8.6. Should we upgrade the Umbraco dependency and use DI instead?
            html = TemplateUtilities.ParseInternalLinks(html, Current.UmbracoContext.UrlProvider);
            html = TemplateUtilities.ResolveUrlsFromTextString(html);
            html = TemplateUtilities.ResolveMediaFromTextString(html);

            HtmlValue = new HtmlString(html);

        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets a rich text value from the specified <paramref name="control"/> and <paramref name="token"/>.
        /// </summary>
        /// <param name="control">The parent control.</param>
        /// <param name="token">The instance of <see cref="JToken"/> to be parsed.</param>
        public new static GridControlRichTextValue Parse(GridControl control, JToken token) {
            return token == null ? null : new GridControlRichTextValue(control, token);
        }

        #endregion

    }

}