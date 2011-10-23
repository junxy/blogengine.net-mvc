// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The widget zone.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace App_Code.Controls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Xml;

    using BlogEngine.Core;
    using BlogEngine.Core.DataStore;

    using Resources;

    /// <summary>
    /// The widget zone.
    /// </summary>
    public class WidgetZone : PlaceHolder
    {
        #region Constants and Fields

        /// <summary>
        /// The xml document by zone.
        /// </summary>
        private static readonly Dictionary<string, XmlDocument> XmlDocumentByZone =
            new Dictionary<string, XmlDocument>();

        /// <summary>
        ///     The zone name.
        /// </summary>
        /// <remarks>
        ///     For backwards compatibility or if a ZoneName is omitted, provide a default ZoneName.
        /// </remarks>
        private string zoneName = "be_WIDGET_ZONE";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref = "WidgetZone" /> class.
        /// </summary>
        static WidgetZone()
        {
            WidgetEditBase.Saved += (sender, args) => OnZonesUpdated();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the name of the data-container used by this instance
        /// </summary>
        public string ZoneName
        {
            get
            {
                return this.zoneName;
            }

            set
            {
                this.zoneName = Utils.RemoveIllegalCharacters(value);
            }
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <value>The XML document.</value>
        private XmlDocument XmlDocument
        {
            get
            {
                // look up the document by zone name
                return XmlDocumentByZone.ContainsKey(this.ZoneName) ? XmlDocumentByZone[this.ZoneName] : null;
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            if (this.XmlDocument == null)
            {
                // if there's no document for this zone name yet, load it
                var doc = RetrieveXml(this.ZoneName);
                if (doc != null)
                {
                    XmlDocumentByZone[this.ZoneName] = doc;
                }
            }

            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs"></see> object that contains the event data.
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var zone = this.XmlDocument.SelectNodes("//widget");
            if (zone == null)
            {
                return;
            }

            // This is for compatibility with older themes that do not have a WidgetContainer control.
            var widgetContainerExists = WidgetContainer.DoesThemeWidgetContainerExist();
            var widgetContainerVirtualPath = WidgetContainer.GetThemeWidgetContainerVirtualPath();

            foreach (XmlNode widget in zone)
            {
                var fileName = string.Format("{0}widgets/{1}/widget.ascx", Utils.RelativeWebRoot, widget.InnerText);
                try
                {

                    var control = (WidgetBase)this.Page.LoadControl(fileName);
                    if (widget.Attributes != null)
                    {
                        control.WidgetId = new Guid(widget.Attributes["id"].InnerText);
                        control.Title = widget.Attributes["title"].InnerText;
                        control.ShowTitle = control.IsEditable
                                                ? bool.Parse(widget.Attributes["showTitle"].InnerText)
                                                : control.DisplayHeader;
                    }

                    control.ID = control.WidgetId.ToString().Replace("-", string.Empty);
                    control.Zone = this.zoneName;

                    control.LoadWidget();

                    // This will return the WidgetContainer with the control in it.
                    var widgetContainer = WidgetContainer.GetWidgetContainer(control, widgetContainerExists, widgetContainerVirtualPath);
                    this.Controls.Add(widgetContainer);
                }
                catch (Exception ex)
                {
                    var lit = new Literal
                        {
                           Text = string.Format("<p style=\"color:red\">Widget {0} not found.<p>", widget.InnerText) 
                        };
                    lit.Text += ex.Message;
                    if (widget.Attributes != null)
                    {
                        lit.Text +=
                            string.Format(
                                "<a class=\"delete\" href=\"#\" onclick=\"BlogEngine.widgetAdmin.removeWidget('{0}');return false\" title=\"{1} widget\">X</a>", 
                                widget.Attributes["id"].InnerText, 
                                labels.delete);
                    }

                    this.Controls.Add(lit);
                }
            }
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> 
        ///     object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object 
        ///     that receives the server control content.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div id=\"widgetzone_{0}\" class=\"widgetzone\">", this.zoneName);

            base.Render(writer);

            writer.Write("</div>");

            if (!Security.IsAuthorizedTo(Rights.ManageWidgets))
            {
                return;
            }

            var selectorId = string.Format("widgetselector_{0}", this.zoneName);
            writer.Write("<select id=\"{0}\" class=\"widgetselector\">", selectorId);
            var di = new DirectoryInfo(this.Page.Server.MapPath(string.Format("{0}widgets", Utils.RelativeWebRoot)));
            foreach (var dir in di.GetDirectories().Where(dir => File.Exists(Path.Combine(dir.FullName, "widget.ascx"))))
            {
                writer.Write("<option value=\"{0}\">{1}</option>", dir.Name, dir.Name);
            }

            writer.Write("</select>&nbsp;&nbsp;");
            writer.Write(
                "<input type=\"button\" value=\"Add\" onclick=\"BlogEngine.widgetAdmin.addWidget(BlogEngine.$('{0}').value, '{1}')\" />", 
                selectorId, 
                this.zoneName);
            writer.Write("<div class=\"clear\" id=\"clear\">&nbsp;</div>");
        }

        /// <summary>
        /// Called when [zones updated].
        /// </summary>
        private static void OnZonesUpdated()
        {
            XmlDocumentByZone.Clear();
        }

        /// <summary>
        /// Retrieves the XML.
        /// </summary>
        /// <param name="zoneName">
        /// The zone Name.
        /// </param>
        /// <returns>
        /// An Xml Document.
        /// </returns>
        private static XmlDocument RetrieveXml(string zoneName)
        {
            var ws = new WidgetSettings(zoneName) { SettingsBehavior = new XmlDocumentBehavior() };
            var doc = (XmlDocument)ws.GetSettings();
            return doc;
        }

        #endregion
    }
}