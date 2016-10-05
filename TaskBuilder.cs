using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.Text.RegularExpressions;
using Sitecore.Diagnostics;
using Sitecore.Globalization;

namespace Sitecore.Modules.AutomatedPublisher
{
    /// <summary>
    /// Class witch creates / update /deletes the schedule items
    /// </summary>
    /// <copyright>Copyright © 2010-2011 Joel Konecny and David DeBruin Sitecore Shared Source Modules</copyright>
    internal class TaskBuilder
	{
		#region MethodsPublic
		/// <summary>
		/// This function checks if a schedule item has to be created / updated / deleted
		/// </summary>
		/// <param name="savedItem">the original saved item</param>
		/// <param name="lang">Language of the original saved item</param>
		/// <param name="db">Database where the item is stored</param>
		/// <param name="bPublishAndNotUnpublish">
		/// <b>true</b>: Publish date will be checked <br/>
		/// <b>false</b>: Unpublish date will be checked 
		/// </param>
		public void CreateItemPublishSchedule(Item savedItem, Language lang, Database db, bool bPublishAndNotUnpublish)
		{
			const string strFunc = "Sitecore.Modules.AutomatedPublisher.TaskBuilder.CreateItemPublishSchedule";
			if (savedItem == null)
			{
				throw new ArgumentNullException("savedItem",
					string.Format(System.Globalization.CultureInfo.CurrentCulture, "Function '{0}' Parameter '{1}' is null.",
					strFunc, "savedItem"));
			}
			if (lang == null)
			{
				throw new ArgumentNullException("lang",
					string.Format(System.Globalization.CultureInfo.CurrentCulture, "Function '{0}' Parameter '{1}' is null.",
					strFunc, "lang"));
			}
			if (db == null)
			{
				throw new ArgumentNullException("db",
					string.Format(System.Globalization.CultureInfo.CurrentCulture, "Function '{0}' Parameter '{1}' is null.",
					strFunc, "db"));
			}

			string strItemName = Regex.Replace(savedItem.ID.ToString(), @"[{}]", String.Empty);
			string strScheduleItemName = string.Format(Constants.AutomatedPublisherItemNameFormat, strItemName);
			if (bPublishAndNotUnpublish == false)
			{
				strScheduleItemName = string.Format(Constants.AutomatedUnPublisherItemNameFormat, strItemName);
			}
			string strScheduleItemNameAndPath = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}/{1}", Constants.SchedulesFolder, strScheduleItemName);

			// search for existing schedules with this item and remove it if it exists
			Item existingScheduleItem = db.GetItem(strScheduleItemNameAndPath, lang);

			//this will not create tasks for items with publishing dates prior to now.
			ItemPublishing ip = savedItem.Publishing;
			if ((ip != null) && (ip.NeverPublish == false) &&
				(((ip.PublishDate > DateTime.UtcNow) && (bPublishAndNotUnpublish == true)) ||
				 ((ip.UnpublishDate > DateTime.UtcNow) && (ip.UnpublishDate != DateTime.MaxValue) && (bPublishAndNotUnpublish == false))))
			{
				DateTime dtSchedulerStartDate = ip.PublishDate;
				if (bPublishAndNotUnpublish == false)
				{
					dtSchedulerStartDate = ip.UnpublishDate;
				}
				CreateScheduleItem(db, savedItem.ID, existingScheduleItem, strScheduleItemName, dtSchedulerStartDate, lang);
			}
			else if (existingScheduleItem != null)
			{
				using (new SecurityDisabler())
				{
					try
					{
						existingScheduleItem.Delete();
					}
					catch (Exception ex)
					{
						Log.Error("Error Deleting old scheduler item", ex, this);
					}
				}
			}
		}
		/// <summary>
		/// This function checks if a schedule item has to be created / updated / deleted for each version
		/// </summary>
		/// <param name="savedItem">the original saved item</param>
		/// <param name="lang">Language of the original saved item</param>
		/// <param name="db">Database where the item is stored</param>
		/// <param name="bPublishAndNotUnpublish">
		/// <b>true</b>: Publish date will be checked <br/>
		/// <b>false</b>: Unpublish date will be checked 
		/// </param>
		public void CreateItemVersionPublishSchedule(Item savedItem, Language lang, Database db, bool bPublishAndNotUnpublish)
		{
			const string strFunc = "Sitecore.Modules.AutomatedPublisher.TaskBuilder.CreateItemVersionPublishSchedule";
			if (savedItem == null)
			{
				throw new ArgumentNullException("savedItem",
					string.Format(System.Globalization.CultureInfo.CurrentCulture, "Function '{0}' Parameter '{1}' is null.",
					strFunc, "savedItem"));
			}
			if (lang == null)
			{
				throw new ArgumentNullException("lang",
					string.Format(System.Globalization.CultureInfo.CurrentCulture, "Function '{0}' Parameter '{1}' is null.",
					strFunc, "lang"));
			}
			if (db == null)
			{
				throw new ArgumentNullException("db",
					string.Format(System.Globalization.CultureInfo.CurrentCulture, "Function '{0}' Parameter '{1}' is null.",
					strFunc, "db"));
			}

			Item[] savedItemVersions = savedItem.Versions.GetVersions();
			int iCnt = savedItemVersions.Length;
			for (int i = 0; i < iCnt; ++i)
			{
				Item savedItemVersion = savedItemVersions[i];
				int iVersionNumber = savedItemVersion.Version.Number;

				string strItemName = Regex.Replace(savedItemVersion.ID.ToString(), @"[{}]", String.Empty);
				string strScheduleItemName = string.Format(Constants.AutomatedPublisherItemVersionNameFormat, strItemName, lang.Name, iVersionNumber);
				if (bPublishAndNotUnpublish == false)
				{
					strScheduleItemName = string.Format(Constants.AutomatedUnPublisherItemVersionNameFormat, strItemName, lang.Name, iVersionNumber);
				}
				string strScheduleItemNameAndPath = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}/{1}", Constants.SchedulesFolder, strScheduleItemName);

				// search for existing schedules with this item and remove it if it exists
				Item existingScheduleItem = db.GetItem(strScheduleItemNameAndPath, lang);

				//this will not create tasks for items with publishing dates prior to now.
				ItemPublishing ip = savedItemVersion.Publishing;
				if ((ip != null) && (ip.NeverPublish == false) && (ip.HideVersion == false) &&
					(((ip.ValidFrom > DateTime.UtcNow) && (bPublishAndNotUnpublish == true)) ||
					 ((ip.ValidTo > DateTime.UtcNow) && (ip.ValidTo != DateTime.MaxValue) && (bPublishAndNotUnpublish == false))))
				{
					DateTime dtSchedulerStartDate = ip.ValidFrom;
					if (bPublishAndNotUnpublish == false)
					{
						dtSchedulerStartDate = ip.ValidTo;
					}
					CreateScheduleItem(db, savedItemVersion.ID, existingScheduleItem, strScheduleItemName, dtSchedulerStartDate, lang);
				}
				else if (existingScheduleItem != null)
				{
					using (new SecurityDisabler())
					{
						try
						{
							existingScheduleItem.Delete();
						}
						catch (Exception ex)
						{
							Log.Error("Error Deleting old scheduler item", ex, this);
						}
					}
				}
			}
		}
		#endregion

		#region MethodsPrivate
		/// <summary>
		/// Insert / Update of the schedule item
		/// </summary>
		/// <param name="db">Database of the item</param>
		/// <param name="sourceItemId">Id of the original saved item</param>
		/// <param name="existingScheduleItem">if not null, the schedule item already exists.</param>
		/// <param name="strScheduleItemName">Name of the schedule item to ceate / update</param>
		/// <param name="dtSchedulerStartDate">start date for the scheduler</param>
		/// <param name="lang">Language of the original saved item</param>
		private void CreateScheduleItem(Database db, ID sourceItemId, Item existingScheduleItem, string strScheduleItemName, DateTime dtSchedulerStartDate,
			Language lang)
		{
			using (new SecurityDisabler())
			{
				try
				{
					//offset by one minute for midnight runs only to account for interval
					if ((dtSchedulerStartDate.Hour == 0) && (dtSchedulerStartDate.Minute == 0))
					{
						dtSchedulerStartDate = dtSchedulerStartDate.AddMinutes(1);
					}

					DateTime dtSchedulerEndDate = dtSchedulerStartDate.AddDays(1);

					string strStartDate = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:yyyyMMdd}", dtSchedulerStartDate);
					string strEndDate = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:yyyyMMdd}", dtSchedulerEndDate);
					string strDaysToRun = "127";

					// sitecore uses 24 hour date format
					string strInterval = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:HH:mm:ss}", dtSchedulerStartDate);

					string strPublishSchedule = strStartDate + "|" + strEndDate + "|" + strDaysToRun + "|" + strInterval;

					Item autoPublishCommand = db.GetItem(Constants.AutoPublishCommandPath, lang);
					Assert.IsNotNull(autoPublishCommand, "command");

					// create schedule item, if it does not exist
					if (existingScheduleItem == null)
					{
						Item schedulesFolder = db.GetItem(Constants.SchedulesFolder, lang);
						Assert.IsNotNull(schedulesFolder, "schedules folder");

						TemplateItem scheduleTemplate = db.GetTemplate(Constants.ScheduleTemplatePath);
						Assert.IsNotNull(scheduleTemplate, "schedule template");

						existingScheduleItem = schedulesFolder.Add(strScheduleItemName, scheduleTemplate);
					}
					Assert.IsNotNull(existingScheduleItem, "schedule");

					using (new EditContext(existingScheduleItem))
					{
						existingScheduleItem.Fields[Constants.Command].Value = autoPublishCommand.ID.ToString();
						existingScheduleItem.Fields[Constants.Items].Value = sourceItemId.ToString();
						existingScheduleItem.Fields[Constants.Schedule].Value = strPublishSchedule;
						existingScheduleItem.Fields[Constants.LastRun].Value = Sitecore.DateUtil.ToIsoDate(new DateTime(dtSchedulerStartDate.Year, dtSchedulerStartDate.Month, dtSchedulerStartDate.Day, 0, 0, 0));
					}
				}
				catch (Exception ex)
				{
					Log.Error("Error Creating Task in Automated Publisher", ex, this);
				}
			}
		}
		#endregion
	}
}
