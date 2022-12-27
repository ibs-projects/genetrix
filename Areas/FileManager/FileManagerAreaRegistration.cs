﻿using System.Web.Mvc;

namespace FileManagerApp.Areas.FileManager {
    public class FileManagerAreaRegistration : AreaRegistration {
        public override string AreaName => "FileManager";

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "FileManager_default2",
                "FileManager/{controller}/{action}/{id}",
                new { action = "Index", controller = "Main", id = UrlParameter.Optional }
            );
        }
    }
}