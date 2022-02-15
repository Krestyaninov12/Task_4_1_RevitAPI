using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_4_1
{//
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            string wallInfo = string.Empty;

            var walls = new FilteredElementCollector(doc)
            .OfClass(typeof(Wall))
                .Cast<Wall>()
                .ToList();

            foreach (var wall in walls)
            {
                string wallType = wall.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString();
                Parameter wallVolue = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                double wallVolumeValue = UnitUtils.ConvertFromInternalUnits(wallVolue.AsDouble(), UnitTypeId.CubicMeters);
                wallInfo += $"{wallType} \t {wallVolumeValue}{Environment.NewLine}";
            }

            var saveDialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "All files (*.*) | *.*",
                FileName= "wallInfo.csv",
                DefaultExt=".csv"
            };

            string selectedFilePath = string.Empty;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = saveDialog.FileName;
            }
            
            if (string.IsNullOrEmpty(selectedFilePath))
                return Result.Cancelled;

            File.WriteAllText(selectedFilePath, wallInfo);
            System.Diagnostics.Process.Start(selectedFilePath);

            return Result.Succeeded;
        }
    }
}
