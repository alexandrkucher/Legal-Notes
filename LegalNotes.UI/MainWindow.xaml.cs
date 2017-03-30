using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LegalNotes.BL;
using LegalNotes.DTO;
using LegalNotes.UI.Helpers;
using LegalNotes.UI.ViewModels;

namespace LegalNotes.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotarialActionsService notarialActionsService = new NotarialActionsService();
        private DocumentsList viewModel = new DocumentsList();

        public MainWindow()
        {
            InitializeComponent();

            InitControls();
            LoadDocuments();
        }

        private void InitControls()
        {
            viewModel.NotarialActions = notarialActionsService.GetNotarialActions();

            this.DataContext = viewModel;
            dtpFrom.SelectedDate = DateTime.Now.AddMonths(-1);
            dtpTo.SelectedDate = DateTime.Now;
        }

        private void btnAddDocument_Click(object sender, RoutedEventArgs e)
        {
            var addDocumentPage = new CreateOrUpdateDocument();
            addDocumentPage.Owner = this;
            addDocumentPage.ShowDialog();

            LoadDocuments();
        }

        private void LoadDocuments()
        {
            var filters = GetFilters();

            viewModel.DocumentsViewModels.Clear();

            var docs = notarialActionsService.GetDocuments(filters);
            var docsViewModels = DocumentsHelper.ConvertToViewModels(docs);

            foreach (var doc in docsViewModels)
                viewModel.DocumentsViewModels.Add(doc);
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadDocuments();
        }

        private void dgrDocuments_Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedDocument = (dgrDocuments.SelectedItem as DocumentViewModel).Document;
            var updateDocumentPage = new CreateOrUpdateDocument(selectedDocument);
            updateDocumentPage.Owner = this;
            updateDocumentPage.ShowDialog();

            LoadDocuments();
        }

        private void dgrDocuments_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (dgrDocuments.SelectedItem == null)
                return;

            if (e.Key != Key.Delete && e.Key != Key.Space)
                return;

            if (e.Key == Key.Delete)
            {
                var selectedDocuments = dgrDocuments.SelectedItems.Cast<DocumentViewModel>();
                var dialogResult = MessageBox.Show(String.Format("Вы действительлно хотите удалить запись(-и) под номером(-ами) {0}?", String.Join(", ", selectedDocuments.Select(x => x.Document.RecordNumber))), "Внимание", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.OK)
                {
                    selectedDocuments.ToList().ForEach(x => notarialActionsService.Delete(x.Document));
                }
                else
                    e.Handled = true;
            }
            else if (e.Key == Key.Space)
            {
                if (dgrDocuments.SelectedItems.Count > 0)
                {
                    var docsIds = dgrDocuments.SelectedItems.Cast<DocumentViewModel>().Select(x => x.Document.DocumentId);
                    notarialActionsService.GroupDocuments(docsIds);

                    e.Handled = true;
                    LoadDocuments();
                }
            }
        }

        private void cmbNotarialActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var selectedItem = e.AddedItems[0] as NotarialAction;

            if (selectedItem == null || !selectedItem.NotarialActionsTypes.Any())
            {
                if (cmbNotarialActionsTypes != null)
                    cmbNotarialActionsTypes.Visibility = Visibility.Collapsed;
                if (cmbNotarialActionsObjects != null)
                {
                    cmbNotarialActionsObjects.Visibility = Visibility.Collapsed;
                    cmbNotarialActionsObjects.DataContext = null;
                    cmbNotarialActionsObjects.DataContext = null;
                }
            }
            else
            {
                cmbNotarialActionsTypes.DataContext = selectedItem.NotarialActionsTypes;
                cmbNotarialActionsTypes.Visibility = Visibility.Visible;
            }
        }

        private void cmbNotarialActionsTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var selectedItem = e.AddedItems[0] as NotarialActionsType;

            if (selectedItem == null || !selectedItem.NotarialActionsObjects.Any())
            {
                if (cmbNotarialActionsObjects != null)
                {
                    cmbNotarialActionsObjects.Visibility = Visibility.Collapsed;
                    cmbNotarialActionsObjects.DataContext = null;
                }
            }
            else
            {
                cmbNotarialActionsObjects.DataContext = selectedItem.NotarialActionsObjects;
                cmbNotarialActionsObjects.Visibility = Visibility.Visible;
            }
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            var filters = GetFilters();
            var reportWindow = new Report(filters);
            reportWindow.ShowDialog();
        }

        private Filters GetFilters()
        {
            return new Filters
            {
                StartDate = dtpFrom.SelectedDate.Value.Date,
                EndDate = dtpTo.SelectedDate.Value.Date,
                NotarialActionId = cmbNotarialActions.SelectedIndex <= 0 ? (int?)null : (cmbNotarialActions.SelectedItem as NotarialAction).NotarialActionId,
                NotarialActionTypeId = cmbNotarialActionsTypes.SelectedIndex <= 0 ? (int?)null : (cmbNotarialActionsTypes.SelectedItem as NotarialActionsType).NotarialActionTypeId,
                NotarialActionObjectId = cmbNotarialActionsObjects.SelectedIndex <= 0 ? (int?)null : (cmbNotarialActionsObjects.SelectedItem as NotarialActionsObject).NotarialActionObjectId
            };
        }
    }
}
