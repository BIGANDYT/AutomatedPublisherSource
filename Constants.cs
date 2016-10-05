using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;

namespace Sitecore.Modules.AutomatedPublisher
{
	/// <summary>
	/// Implements a static class for holding miscellaneous constant values.
	/// </summary>
	/// <copyright>Copyright © 2010-2011 Joel Konecny and David DeBruin Sitecore Shared Source Modules</copyright>
	public static class Constants
	{
		#region Constants
		/// <summary>
		/// Constant for: /sitecore/system/Tasks/Schedules
		/// </summary>
		public static readonly string SchedulesFolder = "/sitecore/system/Tasks/Schedules";
		/// <summary>
		/// Constant for: System/Tasks/Schedule
		/// </summary>
		public static readonly string ScheduleTemplatePath = "System/Tasks/Schedule";
		/// <summary>
		/// Constant for the format for AutomatedPublisher ItemId
		/// </summary>
		public static readonly string AutomatedPublisherItemNameFormat = "ItemId_{0}_AutoPub";
		/// <summary>
		/// Constant for the format for AutomatedUnPublisher ItemId
		/// </summary>
		public static readonly string AutomatedUnPublisherItemNameFormat = "ItemId_{0}_AutoUnPub";
		/// <summary>
		/// Constant for: /sitecore/system/Tasks/Commands/Auto Publish
		/// </summary>
		public static readonly string AutoPublishCommandPath = "/sitecore/system/Tasks/Commands/Auto Publish";
		/// <summary>
		/// Constant for the format for AutomatedPublisher ItemVersionId
		/// </summary>
		public static readonly string AutomatedPublisherItemVersionNameFormat = "ItemId_{0}_{1}_V_{2}_AutoPub";
		/// <summary>
		/// Constant for the format for AutomatedUnPublisher ItemVersionId
		/// </summary>
		public static readonly string AutomatedUnPublisherItemVersionNameFormat = "ItemId_{0}_{1}_V_{2}_AutoUnPub";

		/// <summary>
		/// Constant for: [orphan]
		/// </summary>
		public static readonly string Orphan = "[orphan]";

		/// <summary>
		/// Constant for: shell
		/// </summary>
		public static readonly string SiteNameShell = "shell";
		/// <summary>
		/// Constant for the database name: master
		/// </summary>
		public static readonly string MasterDatabaseName = "master";
		/// <summary>
		/// Constant for: /sitecore/system/publishing targets
		/// </summary>
		public static readonly string PublishingTargetsFolder = "/sitecore/system/publishing targets";
		/// <summary>
		/// Constant for: target database
		/// </summary>
		public static readonly string TargetDatabase = "target database";

		/// <summary>
		/// Constant for field name Command
		/// </summary>
		public static readonly string Command = "Command";
		/// <summary>
		/// Constant for field name Items 
		/// </summary>
		public static readonly string Items = "Items";
		/// <summary>
		/// Constant for field name Schedule 
		/// </summary>
		public static readonly string Schedule = "Schedule";
		/// <summary>
		/// Constant for field name Last run 
		/// </summary>
		public static readonly string LastRun = "Last run";
		#endregion
	}
}
