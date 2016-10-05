using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Data;
using System.Text.RegularExpressions;
using Sitecore.Globalization;

namespace Sitecore.Modules.AutomatedPublisher
{
	/// <summary>
	/// The Item Event Handler Class. OnItemSaved will be called, if a Item is saved. 
	/// See also: <br/> 
	/// <a href="http://trac.sitecore.net/AutomatedPublisher" target="_blank">http://trac.sitecore.net/AutomatedPublisher</a> <br/> 
	/// or <br/> 
	/// <a href="http://sdn.sitecore.net/forum/ShowPost.aspx?PostID=28778" target="_blank">http://sdn.sitecore.net/forum//ShowPost.aspx?PostID=28778</a>
	/// </summary>
	/// <remarks>
	/// A config file or a web.config entry will be needed.
	/// e.g.
	/// <code>
	/// &lt;configuration &gt;
	///     &lt;sitecore&gt;
	///         &lt;events timingLevel="custom"&gt;
	///             &lt;event name="item:saved"&gt;
	///                &lt;handler type="MCH.Shell.Applications.AutomatedPublisher.ItemEventHandler, MCH.CMS.Client" method="OnItemSaved"/&gt;
	///             &lt;/event&gt;
	///         &lt;/events&gt;
	///     &lt;/sitecore&gt;
	/// &lt;/configuration&gt;
	/// </code>
	/// </remarks>
	/// <copyright>Copyright © 2010-2011 Joel Konecny and David DeBruin Sitecore Shared Source Modules</copyright>
	public class ItemEventHandler
	{
		#region MethodsPublic
		/// <summary>
		/// Eventhandler, called after saving a item
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="args">event arguments</param>
		public void OnItemSaved(object sender, EventArgs args)
		{
			SitecoreEventArgs eventArgs = args as SitecoreEventArgs;

			try
			{
				Assert.IsNotNull(eventArgs, "eventArgs");
				Item savedItem = eventArgs.Parameters[0] as Item;
				Assert.IsNotNull(savedItem, "savedItem");

				// ignore tasks themselves, orphand items and only in the context of the 'shell'
				if ((savedItem.Template.FullName != Constants.ScheduleTemplatePath) &&
					(savedItem.Paths.FullPath.StartsWith(Constants.Orphan, StringComparison.OrdinalIgnoreCase) == false) &&
					(Sitecore.Context.GetSiteName() == Constants.SiteNameShell))
				{
					Database db = Sitecore.Context.ContentDatabase;
					Language lang = savedItem.Language;

					TaskBuilder taskBuilder = new TaskBuilder();
					taskBuilder.CreateItemPublishSchedule(savedItem, lang, db, true);
					taskBuilder.CreateItemPublishSchedule(savedItem, lang, db, false);
					taskBuilder.CreateItemVersionPublishSchedule(savedItem, lang, db, true);
					taskBuilder.CreateItemVersionPublishSchedule(savedItem, lang, db, false);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Error OnItemSaved in Automated Publisher", ex, this);
			}
		}
		#endregion
	}
}
