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
            var filters = new Filters
            {
                StartDate = dtpFrom.SelectedDate.Value.Date,
                EndDate = dtpTo.SelectedDate.Value.Date,
                NotarialActionId = cmbNotarialActions.SelectedIndex <= 0 ? (int?)null : (cmbNotarialActions.SelectedItem as NotarialAction).NotarialActionId,
                NotarialActionTypeId = cmbNotarialActionsTypes.SelectedIndex <= 0 ? (int?)null : (cmbNotarialActionsTypes.SelectedItem as NotarialActionsType).NotarialActionTypeId,
                NotarialActionObjectId = cmbNotarialActionsObjects.SelectedIndex <= 0 ? (int?)null : (cmbNotarialActionsObjects.SelectedItem as NotarialActionsObject).NotarialActionObjectId
            };

            viewModel.Documents.Clear();

            var docs = notarialActionsService.GetDocuments(filters);
            foreach (var doc in docs)
                viewModel.Documents.Add(doc);

        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadDocuments();
        }

        private void dgrDocuments_Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedDocument = dgrDocuments.SelectedItem as Document;
            var updateDocumentPage = new CreateOrUpdateDocument(selectedDocument);
            updateDocumentPage.Owner = this;
            updateDocumentPage.ShowDialog();

            LoadDocuments();
        }

        private void dgrDocuments_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (dgrDocuments.SelectedItem == null || e.Key != Key.Delete)
                return;

            var selectedDocument = dgrDocuments.SelectedItem as Document;

            var dialogResult = MessageBox.Show(String.Format("Вы действительлно хотите удалить запись под номером {0}?", selectedDocument.RecordNumber), "Внимание", MessageBoxButton.OKCancel);
            if (dialogResult == MessageBoxResult.OK)
            {
                notarialActionsService.Delete(selectedDocument);
            }
            else
                e.Handled = true;
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
    }
}
