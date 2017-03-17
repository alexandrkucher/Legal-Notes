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

namespace LegalNotes.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NotarialActionsService notarialActionsService = new NotarialActionsService();

        public MainWindow()
        {
            InitializeComponent();

            InitControls();
        }

        private void InitControls()
        {
            var notarialActions = notarialActionsService.GetNotarialActions();
            cmbNotarialActions.DataContext = notarialActions;
        }

        private void cmbNotarialActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems[0] as NotarialAction;
            cmbNotarialTypes.DataContext = selectedItem.NotarialActionsTypes;

            cmbNotarialTypes.Visibility = selectedItem.NotarialActionsTypes.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void cmbNotarialTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var selectedItem = e.AddedItems[0] as NotarialActionsType;
            cmbNotarialObjects.DataContext = selectedItem.NotarialActionsObjects;

            cmbNotarialObjects.Visibility = selectedItem.NotarialActionsObjects.Any() ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
