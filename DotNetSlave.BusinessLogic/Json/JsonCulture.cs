using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Web.Script.Serialization;

namespace BlogEngine.Core.Json
{

    /// <summary>
    /// Represents the i18n culture used by blog.js. Used by ResourceHandler.
    /// </summary>
    public sealed class JsonCulture
    {

        #region "Fields"

        private readonly CultureInfo cultureInfo;
        private readonly Dictionary<string, string> translationDict = new Dictionary<string, string>();

        #endregion

        /// <summary>
        /// Creates a new JsonCulture instance from the supplied CultureInfo.
        /// </summary>
        /// <param name="cultureInfo">The CultureInfo needed to get the proper translations for this JsonCulture instance</param>
        /// <remarks>
        /// 
        /// This class uses a dictionary as its basis for storing/caching its information. This makes it incredibly easy to extend
        /// without having to create/remove properties.
        /// 
        /// </remarks>
        public JsonCulture(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }
            this.cultureInfo = cultureInfo;

            this.AddResource("hasRated", "youAlreadyRated");
            this.AddResource("savingTheComment", "savingTheComment");
            this.AddResource("comments", "comments");
            this.AddResource("commentWasSaved", "commentWasSaved");
            this.AddResource("commentWaitingModeration", "commentWaitingModeration");
            this.AddResource("cancel", "cancel");
            this.AddResource("filter", "filter");
            this.AddResource("apmlDescription", "filterByApmlDescription");
            this.AddResource("beTheFirstToRate", "beTheFirstToRate");
            this.AddResource("currentlyRated", "currentlyRated");
            this.AddResource("ratingHasBeenRegistered", "ratingHasBeenRegistered");
            this.AddResource("rateThisXStars", "rateThisXStars");
        }


        #region "Methods"

        /// <summary>
        /// Adds a new translatable string resource to this JsonCulture.
        /// </summary>
        /// <param name="scriptKey">The key used to retrieve this value from clientside script.</param>
        /// <param name="resourceLabelKey">The key used to retrieve the translated value from global resource labels.</param>
        /// <returns>The translated string.</returns>
        public string AddResource(string scriptKey, string resourceLabelKey)
        {
            var translation = Utils.Translate(resourceLabelKey, null, this.cultureInfo);
            this.translationDict.Add(scriptKey, translation);
            return translation;
        }

       
        /// <summary>
        /// Returns a JSON formatted string repressentation of this JsonCulture instance's culture labels.
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return new JavaScriptSerializer().Serialize(this.translationDict);
        }

      
        #endregion


    }
}
