#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace TestAddin2905
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            /*var assembly = Assembly.GetExecutingAssembly();

            PushButtonData pushButtonData
                = new PushButtonData("adncisButton",
                    "ECOPOLIMER",
                    assembly.Location,
                    "RibbonUtilSample.Command");
            pushButtonData.LongDescription = "Пример описания для команды";

            // ищем путь к файлу изображения
            var assemblyDir = new FileInfo(assembly.Location).DirectoryName;
            var imagePath = Path.Combine(assemblyDir, "adn-cis-logo.png");
            pushButtonData.LargeImage = new BitmapImage(new Uri(imagePath));

            // Создадим новую вкладку
            // При этом нельзя проверить есть ли уже такая вкладка или нет
            //a.CreateRibbonTab("ECOPOLIMER");
            //a.GetRibbonPanels("ECOPOLIMER");


            // Создаем новую панель на вкладке ADN-CIS
            var panel = a.CreateRibbonPanel("ECOPOLIMER", "ECOPOLIMER");

            // Добавляем кнопку на панель
            panel.AddItem(pushButtonData);*/

            return Result.Succeeded;

        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
