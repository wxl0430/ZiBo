using System.Windows;
using System.Windows.Controls;

namespace CRSim.ScreenSimulator.Views
{
    public class BaseDataGrid : DataGrid
    {
        public BaseDataGrid()
        {
            GridLinesVisibility = DataGridGridLinesVisibility.None;
            HeadersVisibility = DataGridHeadersVisibility.Column;
            BorderThickness = new Thickness(0);
            CanUserResizeRows = false;
            CanUserReorderColumns = false;
            CanUserResizeColumns = false;
            CanUserSortColumns = false;
            CanUserAddRows = false;
            AutoGenerateColumns = false;
            IsEnabled = false;
            IsReadOnly = true;
        }
    }
}
