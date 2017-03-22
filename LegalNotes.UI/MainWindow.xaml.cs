using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LegalNotes.BL;
using LegalNotes.DTO;

namespace LegalNotes.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotarialActionsService notarialActionsService = new NotarialActionsService();

        private ObservableCollection<Document> documents = new ObservableCollection<Document>();

        public MainWindow()
        {
            InitializeComponent();

            InitControls();
            ReloadData();
        }

        private void InitControls()
        {
            dgrDocuments.DataContext = documents;
            dtpFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dtpTo.SelectedDate = DateTime.Now;
        }
        
        private void btnAddDocument_Click(object sender, RoutedEventArgs e)
        {
            var addDocumentPage = new CreateOrUpdateDocument();
            addDocumentPage.Owner = this;
            addDocumentPage.ShowDialog();

            ReloadData();
        }

        private void ReloadData()
        {
            var filters = new Filters
            {
                StartDate = dtpFrom.SelectedDate.Value.Date,
                EndDate = dtpTo.SelectedDate.Value.Date
            };

            documents.Clear();

            var docs = notarialActionsService.GetDocuments(filters);
            foreach (var doc in docs)
                documents.Add(doc);

        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        private void dgrDocuments_Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedDocument = dgrDocuments.SelectedItem as Document;
            var updateDocumentPage = new CreateOrUpdateDocument(selectedDocument);
            updateDocumentPage.Owner = this;
            updateDocumentPage.ShowDialog();

            ReloadData();
        }        

        private void dgrDocuments_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (dgrDocuments.SelectedItem == null || e.Key != Key.Delete)
                return;

            var selectedDocument = dgrDocuments.SelectedItem as Document;

            var dialogResult = MessageBox.Show(String.Format("Вы действительлно хотите удалить запись под номером {0}?", selectedDocument.DocumentId), "Внимание", MessageBoxButton.OKCancel);
            if (dialogResult == MessageBoxResult.OK)
            {
                notarialActionsService.Delete(selectedDocument);
            }
            else
                e.Handled = true;
        }
    }
}
