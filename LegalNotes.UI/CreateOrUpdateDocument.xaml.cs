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
        private int oldId;

        public CreateOrUpdateDocument() : this(new Document())
        {
        }

        public CreateOrUpdateDocument(Document document)
        {
            viewModel.Document = document;

            InitializeComponent();

            InitControls();
        }

        private void InitControls()
        {
            if (viewModel.Document.DocumentId == 0)
            {
                viewModel.Document.DocumentId = notarialActionsService.GetNewDocumentId();
                this.Title = "Добавить документ";
            }
            else
            {
                isNewDocument = false;
                oldId = viewModel.Document.DocumentId;
                this.Title = "Изменить документ";
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
            var document = new Document
            {
                DocumentId = int.Parse(txtId.Text),
                CreateDate = DateTime.UtcNow,
                Price = decimal.Parse(txtSum.Text),
                NotarialAction = cmbNotarialActions.SelectedItem as NotarialAction,
                NotarialActionsType = cmbNotarialTypes.SelectedItem as NotarialActionsType,
                NotarialActionsObject = cmbNotarialObjects.SelectedItem as NotarialActionsObject,
                Client = new Client
                {
                    CreateDate = DateTime.UtcNow,
                    PassportNumber = txtPassportNumber.Text.Trim().ToUpper(),
                    PassportData = txtPassportData.Text,
                    Address = txtAddress.Text,
                    Name = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    MiddleName = txtMiddleName.Text.Trim()
                }
            };

            if (isNewDocument)
                notarialActionsService.CreateDocument(document);
            else
                notarialActionsService.UpdateDocument(oldId, document);

            this.Close();
        }
    }
}