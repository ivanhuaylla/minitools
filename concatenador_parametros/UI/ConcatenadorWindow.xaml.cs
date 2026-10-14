using System.Windows;

namespace Riga.ConcatenadorParametros.UI
{
    public partial class ConcatenadorWindow : Window
    {
        private ConcatenadorViewModel _vm;

        public ConcatenadorWindow(object viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            if (viewModel is ConcatenadorViewModel vm)
            {
                _vm = vm;
                _vm.CerrarVentana += OnCerrarVentana;
            }
        }

        private void OnCerrarVentana()
        {
            this.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            if (_vm != null)
            {
                _vm.CerrarVentana -= OnCerrarVentana;
            }
            base.OnClosed(e);
        }
    }
}
