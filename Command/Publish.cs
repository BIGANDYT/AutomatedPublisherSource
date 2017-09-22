using System;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.Diagnostics;
using System.Linq;
using Sitecore.Globalization;
using Sitecore.Configuration;

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
        private Database _master = null;
        protected Database master
        {
            get
            {
                return _master ?? (_master = Sitecore.Configuration.Factory.GetDatabase(Constants.MasterDatabaseName));
            }
        }
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
            //DateTime dtPublishDate = DateTime.UtcNow;

            using (new SecurityDisabler())
            {
                try
                {
                    // for each existing target
                    Sitecore.Collections.ChildList publishingTargets = master.GetItem(Constants.PublishingTargetsFolder)?.Children;
                    foreach (Item publishingTarget in publishingTargets)
                    {
                        var targetDatabase = GetTargetDatabase(publishingTarget);
                        if(targetDatabase == null)
                        {
                            Log.Info("Skipping target: Could not find Database target for Publishing target: " + publishingTarget.Name, this);
                            continue;
                        }

                        PublishItems(items, publishingTarget, targetDatabase);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error Publishing in Automated Publisher", ex, this);
                }
            }
        }

        protected void PublishItems(Item[] items, Item publishingTarget, Database targetDb)
        {
            

            // for each existing language
            foreach (var language in master.Languages)
            {
                foreach (Item actItem in items)
                {
                    ID itemToPublishId = actItem.ID;
                    Item itemToPublish = master.GetItem(itemToPublishId, language);

                    try
                    {
                        if (ValidatePublishingTargets(itemToPublish, publishingTarget))
                        {
                            string strMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Automated Publishing Item '{0}' Target '{1}' Language '{2}'", itemToPublish.ID.ToString(), publishingTarget.Name, language.Name);
                            Log.Info(strMessage, this);


                            PublishItem(itemToPublish, targetDb, language);
                        }
                        else
                        {
                            string strMessage = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Automated Publishing Item '{0}' Target '{1}' Language '{2}': No publishing targets were selected in Automated Publisher",
                                itemToPublish.ID.ToString(), publishingTarget.Name, language.Name);
                            throw new Exception(strMessage);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error Auto publishing for Item: " + actItem.ID + ", Language: " + language.Name, e, this);
                    }
                }
            }
        }

        protected bool ValidatePublishingTargets(Item itemToPublish, Item publishingTarget)
        {
            Sitecore.Data.Fields.MultilistField targets = itemToPublish.Fields[Sitecore.FieldIDs.PublishingTargets];

            var itemToPublishTargets = targets?.GetItems();
            if (itemToPublishTargets != null)
            {
                if (itemToPublishTargets.Any(x => string.Equals(x.Name, publishingTarget.Name)))
                {
                    return true; //found matching target!
                }
            }
            else
            {
                //there are no available publishing targets to select so no target is still valid
                return true;
            }

            //couldnt find a matching target from the available targets
            return false;

        }

        protected Database GetTargetDatabase(Item publishingTarget)
        {
            string targetDBName = publishingTarget[Constants.TargetDatabase];
            if (string.IsNullOrWhiteSpace(targetDBName))
            {
                return null;
            }

            return Factory.GetDatabase(targetDBName);
        }

        protected void PublishItem(Item itemToPublish, Database targetDb, Language language)
        {
            PublishOptions publishOptions = new PublishOptions(master, targetDb, PublishMode.Full, language, DateTime.UtcNow);
            publishOptions.Deep = true;
            publishOptions.RootItem = itemToPublish;

            Publisher publisher = new Publisher(publishOptions);
            publisher.Publish();
        }
    }
}
