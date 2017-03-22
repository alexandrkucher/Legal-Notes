using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LegalNotes.BL;
using LegalNotes.DTO;

namespace LegalNotes.UI
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        private readonly NotarialActionsService notarialActionsService = new NotarialActionsService();

        public Report() : this(new Filters())
        {
        }

        public Report(Filters filters)
        {
            InitializeComponent();

            LoadData(filters);
        }

        private void LoadData(Filters filters)
        {
            var result = notarialActionsService.GetReport(filters);
            dgrDocuments.DataContext = result;
        }
    }
}
