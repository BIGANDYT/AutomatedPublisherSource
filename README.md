# AutomatedPublisherSource
The Automated Publisher module is a module that will automatically generate a Sitecore task to publish an item after its scheduled publication date. Any time an item is saved (programatically or manually), the Automated Publisher will look to see if the item has a future publication date and set up the task accordingly. This alleviates the need for a manual publish or the use of blanket publishing tasks on a regular basis. Please note that this utility does not bypass workflow and an item that is not in a finalized state of workflow will not be marked as publishable by Sitecore. Please see the documentation link below for technical details (a single modification to the web.config is required to run the Automated Publisher).

Source provided by Joel Konecny (joel.konecny@gmail.com) and David DeBruin, idea contributed by John West. 

Other contributors:

Jenny Bernhard

GETTING STARTED WITH THE AUTOMATED PUBLISHER
The module installation includes a config file for the item:saved event handler that gets appended to the web.config at runtime.

The handler will then create a scheduled task timed to launch shortly after your item is saved with a future publication date. The schedules will be located within the Sitecore schedules folder:

/sitecore/system/Tasks/Schedules

The current naming convention of the scheduled task created is:

/sitecore/system/Tasks/Schedules/ItemId_[GUID of the item to be published]_AutoPub

The package will include the installation of a command called "Auto Publish" with this item path:

/sitecore/system/Tasks/Commands/Auto Publish

When the task fires it will execute the "Auto Publish" command for the item being published. If the item is in a finalized state in workflow the item will be published to production. This is important, the tool does not circumvent Sitecore workflow, so you will need to follow workflow as normal, prior to the items publish date, for the item to be published successfully using the automated publisher.

The Automated Publisher will use the item's publishing targets programatically so it will be deployed to the location(s) you set on the item.

After the scheduled task fires the task will be automatically removed from the /sitecore/system/Tasks/Schedules/ folder. It is only meant to fire a single time.

Please note that Sitecore contains settings in the <scheduling> section of the web.config for the frequency that it detects scheduled tasks waiting to be executed as well as schedules for the timers that run the scheduled tasks. In the event a publication date is due to fire at midnight the Automated Publisher will actually add one minute to the scheduled task to account for an interval value needed by the task itself.

The package includes the Auto Publish task noted above, an Sitecore.Modules.AutomatedPublisher.dll that will be dropped into your bin directory and an AutomatedPublisher.config file which will be placed into your App_Config > Include directory.

This module has full logging and will write to the Sitecore logs in the data folder when the Automated Publisher starts and ends and if any errors occur.
