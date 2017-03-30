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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LegalNotes.BL;
using LegalNotes.DTO;
using VM = LegalNotes.UI.ViewModels;

namespace LegalNotes.UI
{
    /// <summary>
    /// Interaction logic for AddDocument.xaml
    /// </summary>
    public partial class CreateOrUpdateDocument : Window
    {
        private readonly NotarialActionsService notarialActionsService = new NotarialActionsService();

        private VM.CreateOrUpdateDocument viewModel = new VM.CreateOrUpdateDocument();
        private bool isNewDocument = true;

        public CreateOrUpdateDocument() : this(new Document())
        {
        }

        public CreateOrUpdateDocument(Document document)
        {
            if (document.DocumentId != 0)
                isNewDocument = false;

            viewModel.Document = document;

            InitializeComponent();

            InitControls();
        }

        private void InitControls()
        {
            if (isNewDocument)
            {
                viewModel.Document.RecordNumber = notarialActionsService.GetNewDocumentId();
                this.Title = "Добавить документ";
                txtDocumentsCount.Visibility = Visibility.Visible;
                viewModel.Document.Date = DateTime.UtcNow.Date;
            }
            else
            {
                this.Title = "Изменить документ";
                txtDocumentsCount.Visibility = Visibility.Collapsed;
            }

            viewModel.NotarialActions = notarialActionsService.GetNotarialActions();

            DataContext = viewModel;

            if (viewModel.Document.NotarialAction != null)
                cmbNotarialActions.SelectedIndex = viewModel.NotarialActions.Select((x, index) => new { Document = x, Index = index }).First(x => x.Document.NotarialActionId == viewModel.Document.NotarialAction.NotarialActionId).Index;
        }

        private void cmbNotarialActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var selectedItem = e.AddedItems[0] as NotarialAction;
            cmbNotarialTypes.DataContext = selectedItem.NotarialActionsTypes;

            if (selectedItem.NotarialActionsTypes.Any())
            {
                cmbNotarialTypes.Visibility = Visibility.Visible;
                if (viewModel.Document.NotarialActionsType != null && viewModel.Document.NotarialAction.NotarialActionId == selectedItem.NotarialActionId)
                    cmbNotarialTypes.SelectedIndex = (cmbNotarialTypes.DataContext as IEnumerable<NotarialActionsType>).Select((x, index) => new { Document = x, Index = index }).FirstOrDefault(x => x.Document.NotarialActionTypeId == viewModel.Document.NotarialActionsType.NotarialActionTypeId).Index;
            }
            else
            {
                cmbNotarialTypes.Visibility = Visibility.Collapsed;
                cmbNotarialObjects.DataContext = null;
                cmbNotarialObjects.Visibility = Visibility.Collapsed;
            }
        }

        private void cmbNotarialTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var selectedItem = e.AddedItems[0] as NotarialActionsType;
            cmbNotarialObjects.DataContext = selectedItem.NotarialActionsObjects;

            if (selectedItem.NotarialActionsObjects.Any())
            {
                cmbNotarialObjects.Visibility = Visibility.Visible;
                if (viewModel.Document.NotarialActionsObject != null && viewModel.Document.NotarialActionsType.NotarialActionTypeId == selectedItem.NotarialActionTypeId)
                    cmbNotarialObjects.SelectedIndex = (cmbNotarialObjects.DataContext as IEnumerable<NotarialActionsObject>).Select((x, index) => new { Document = x, Index = index }).First(x => x.Document.NotarialActionObjectId == viewModel.Document.NotarialActionsObject.NotarialActionObjectId).Index;
            }
            else
                cmbNotarialObjects.Visibility = Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var recordNumber = int.Parse(txtRecordNumber.Text);
                var docsCount = int.Parse(txtDocumentsCount.Text);

                for (int i = recordNumber; i < recordNumber + docsCount; i++)
                {
                    viewModel.Document.RecordNumber = i;
                    viewModel.Document.Date = dtpDate.SelectedDate.Value;
                    viewModel.Document.Price = decimal.Parse(txtSum.Text);
                    viewModel.Document.NotarialAction = cmbNotarialActions.SelectedItem as NotarialAction;
                    viewModel.Document.NotarialActionsType = cmbNotarialTypes.SelectedItem as NotarialActionsType;
                    viewModel.Document.NotarialActionsObject = cmbNotarialObjects.SelectedItem as NotarialActionsObject;

                    if (isNewDocument)
                        notarialActionsService.CreateDocument(viewModel.Document);
                    else
                        notarialActionsService.UpdateDocument(viewModel.Document);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неправильный формат данных");
            }
        }        
    }
}