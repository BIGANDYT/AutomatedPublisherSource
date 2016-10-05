using System;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.AutomatedPublisher.Command
{
    /// <summary>
    /// Class witch implements the auto publish command
    /// </summary>
    /// <remarks>
    /// A item has to be created in path /sitecore/system/Tasks/Commands with name Auto Publish
    /// e.g.
    /// <code>
    /// /sitecore/system/Tasks/Commands/Auto Publish
    /// Type: MCH.Shell.Applications.AutomatedPublisher.Command.Publish, MCH.CMS.Client
    /// Method: Run
    /// </code>
    /// </remarks>
    /// <copyright>Copyright © 2010-2011 Joel Konecny and David DeBruin Sitecore Shared Source Modules</copyright>
    [CLSCompliant(false)]
	public class Publish
	{
		/// <summary>
		/// This method gets called from a scheduled task command.
		/// You define a scheduled task command by navigating to 
		/// /System/Tasks/Commands and inserting a new command
		/// </summary>
		/// <param name="items">Items to publish</param>
		/// <param name="command">Command Info</param>
		/// <param name="schedule">Schedule Item</param>
		public void Run(Item[] items, Tasks.CommandItem command, Tasks.ScheduleItem schedule)
		{
			DateTime dtPublishDate = DateTime.UtcNow;
			bool bItemPublished = false;

			using (new SecurityDisabler())
			{
				try
				{
					// Database master = Sitecore.Context.ContentDatabase;
					Database master = Sitecore.Configuration.Factory.GetDatabase(Constants.MasterDatabaseName);
					Sitecore.Globalization.Language[] languages = master.Languages;

					// for each existing target
					Item publishingTargetsFolder = master.GetItem(Constants.PublishingTargetsFolder);
					Sitecore.Collections.ChildList publishingTargets = publishingTargetsFolder.Children;
					foreach (Item publishingTarget in publishingTargets)
					{
						if (publishingTarget != null)
						{
							string targetDBName = publishingTarget[Constants.TargetDatabase];
							if (string.IsNullOrEmpty(targetDBName) == false)
							{
								Database targetDb = Sitecore.Configuration.Factory.GetDatabase(targetDBName);
								if (targetDb != null)
								{
									// for each existing language
									foreach (Sitecore.Globalization.Language language in languages)
									{
										foreach (Item actItem in items)
										{
											ID itemToPublishId = actItem.ID;
											Item itemToPublish = master.GetItem(itemToPublishId, language);
											bool bPublish = false;

											Sitecore.Data.Fields.MultilistField targets = itemToPublish.Fields[Sitecore.FieldIDs.PublishingTargets];
											if (targets != null)
											{
												Item[] itemToPublishTargets = targets.GetItems();
												if (itemToPublishTargets.Length > 0)
												{
													foreach (Item itemToPublishTarget in itemToPublishTargets)
													{
														if (string.Equals(itemToPublishTarget.Name, publishingTarget.Name, StringComparison.CurrentCultureIgnoreCase) == true)
														{
															bPublish = true;
															break;
														}
													}
												}
												else
												{
													bPublish = true;
												}
											}
											else
											{
												bPublish = true;
											}

											if (bPublish == true)
											{
												try
												{
													string strMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Item '{0}' Target '{1}' Language '{2}': Automated Publisher Schedule Item '{3}'", itemToPublish.ID.ToString(), publishingTarget.Name, language.Name, schedule.Name);
													Log.Info(strMessage, this);
												}
												catch
												{
												}

												PublishOptions publishOptions = new PublishOptions(master, targetDb, PublishMode.Full, language, dtPublishDate);
												publishOptions.Deep = true;
												publishOptions.RootItem = itemToPublish;
												// publishOptions.CompareRevisions = false;
												// publishOptions.RepublishAll = true;

												Publisher publisher = new Publisher(publishOptions);
												publisher.Publish();
												bItemPublished = true;
											}
											else
											{
												string strMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Item '{0}' Target '{1}' Language '{2}': No publishing targets were selected in Automated Publisher",
													itemToPublish.ID.ToString(), publishingTarget.Name, language.Name);
												Log.Error(strMessage, this);
											}
										}
									}
								}
							}
						}
					}
					schedule.Remove();
					if (bItemPublished == true)
					{
                        //-----------------------------------------------------------------------------
                        // Gemäss Eintrag im Log, wird der Cache automatisch gelöscht nach dem Publish
                        //-----------------------------------------------------------------------------
                        // string strMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Automated Publisher Schedule Item CacheManager.ClearAllCaches(); Date / Time: {0}", DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss.fff"));
                        // Log.Info(strMessage, this);
                        // CacheManager.ClearAllCaches();
                    }
                }
				catch (Exception ex)
				{
					Log.Error("Error Publishing in Automated Publisher", ex, this);
				}
			}
		}
	}
}
