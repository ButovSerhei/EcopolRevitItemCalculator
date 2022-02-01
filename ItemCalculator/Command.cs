#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Data.SQLite;
using System.Windows;

#endregion

namespace TestAddin2905
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;



            Selection sel = uidoc.Selection;

            using (Transaction tx = new Transaction(doc))
            {
                //ElementClassFilter filter = new ElementClassFilter(typeof(MEPCurve));
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.OfClass(typeof(Autodesk.Revit.DB.Plumbing.Pipe));
                ICollection<Element> allpipe = collector.ToElements();

                FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                collector2.OfCategory(BuiltInCategory.OST_PipeFitting).WhereElementIsNotElementType();
                ICollection<Element> alldetail = collector2.ToElements();

                FilteredElementCollector collector3 = new FilteredElementCollector(doc);
                IList<Element> ALLelements = new List<Element>();

                List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
                builtInCats.Add(BuiltInCategory.OST_PipeFitting);
                builtInCats.Add(BuiltInCategory.OST_GenericModel);
                builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
                builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
                builtInCats.Add(BuiltInCategory.OST_SpecialityEquipment);
                builtInCats.Add(BuiltInCategory.OST_PlumbingFixtures);
                builtInCats.Add(BuiltInCategory.OST_Mass);

                ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);
                collector3.WherePasses(filter1).WhereElementIsNotElementType();
                ALLelements = collector3.ToElements();



                //Reference reference = uidoc.Selection.PickObject(ObjectType.Element);
                //Element element = uidoc.Document.GetElement(reference);

                tx.Start("Transaction Name");

                using (var Dbase = new SQLiteConnection("Data Source=H:\\3D Проект\\Архив скриптов\\SQLITE\\specification; Version=3"))
                {
                    foreach (var pipe in allpipe)
                    {
                        SetPipeParam(Dbase, pipe);
                    }

                    SetSumParam(ALLelements);

                }
                tx.Commit();
            }
            return Result.Succeeded;
        }
        private static void SetSumParam(IList<Element> Allelement)
        {
            foreach (var el in Allelement)
            {
                try
                {
                    el.LookupParameter("Кол.").Set(1);
                }
                catch
                {
                    break;
                }

            }
        }
        private static void SetDetailParam(SQLiteConnection Dbase, Element detail)
        {
            //MessageBox.Show(detail.Name);
        }
        private static void SelectionValuesInTheDatabase()
        {

        }
        private static void SetPipeParam(SQLiteConnection Dbase, Element pipe)
        {
            double R_pipe_ND = Math.Round(pipe.LookupParameter("Внешний диаметр").AsDouble() * 0.3048 * 1000, 1);
            double R_pipe_DN = Math.Round(pipe.LookupParameter("Размер").AsDouble() * 0.3048 * 1000, 1);
            double R_pipe_TS = Math.Round(pipe.LookupParameter("ECOPOL_Толщина стенки").AsDouble() * 0.3048 * 1000, 1);


            pipe.LookupParameter("Сортировка спецификации").Set(2.02);

            string R_pipe_NameS = pipe.LookupParameter("ECOPOL_Имя системы").AsString();
            pipe.LookupParameter("ECOPOL_Имя системы марка").Set(R_pipe_NameS);

            double R_pipe_Len = pipe.LookupParameter("Длина").AsDouble() * 0.3048;
            pipe.LookupParameter("Кол.").Set(R_pipe_Len + R_pipe_Len * 0.1);

            pipe.LookupParameter("ECOPOL_Ед.измерения").Set("м.");

            ElementId R_pipe_Id = pipe.Id;
            Dbase.Open();

            SQLiteCommand CMD = Dbase.CreateCommand();
            CMD.CommandText = "select * from Pipe";
            SQLiteDataReader SQL = CMD.ExecuteReader();
            while (SQL.Read())
            {
                if (SQL["Pipe_NameType"].Equals(pipe.Name) && SQL["Pipe_ND"].Equals(R_pipe_ND) && SQL["Pipe_TS"].Equals(R_pipe_TS))
                {
                    try
                    {

                        pipe.LookupParameter("ECOPOL_Наименование и техническая характеристика").Set(SQL["Name_pipe"].ToString());

                        pipe.LookupParameter("ECOPOL_Масса 1 ед., кг").Set(Convert.ToDouble(SQL["Pipe_mass"]));

                        pipe.LookupParameter("ECOPOL_марка").Set(SQL["Pipe_Mark"].ToString());
                        break;
                    }
                    catch
                    {
                        TaskDialog.Show("///", "Параметр не добавлен", TaskDialogCommonButtons.Ok);
                        break;
                    }
                }
                else
                {
                    //TaskDialog.Show("///", SQL["Pipe_ND"].Equals(R_pipe_ND) + " ND + " + SQL["Pipe_ND"].ToString() + " " + R_pipe_ND.ToString(), TaskDialogCommonButtons.Ok);   
                }

            }
            Dbase.Close();
        }
    }
}
