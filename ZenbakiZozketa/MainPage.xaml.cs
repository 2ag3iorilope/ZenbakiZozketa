using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenbakiZozketa
{
    public partial class MainPage : ContentPage
    {
        const int MaxCheckBoxes = 49;
        const int MaxSelectedNumbers = 6;
        List<CheckBox> checkBoxes;
        List<int> selectedNumbers = new List<int>();

        public MainPage()
        {
            InitializeComponent();
            CrearCheckBoxes();
        }

        private void CrearCheckBoxes()
        {
            checkBoxes = new List<CheckBox>();
            int columns = 7; // 7 columnas de checkboxes
            int rows = MaxCheckBoxes / columns; // filas necesarias

            for (int i = 1; i <= MaxCheckBoxes; i++)
            {
                var checkBox = new CheckBox
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                checkBox.CheckedChanged += OnCheckBoxCheckedChanged;
                checkBoxes.Add(checkBox);

                // Crear una etiqueta para el número del CheckBox
                var label = new Label
                {
                    Text = i.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                // Añadir ambos al Grid en la misma celda
                checkboxGrid.Children.Add(label, (i - 1) % columns, (i - 1) / columns); // Añadir la etiqueta a la celda
                checkboxGrid.Children.Add(checkBox, (i - 1) % columns, (i - 1) / columns); // Añadir el CheckBox a la misma celda
            }
        }



        private void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            int index = checkBoxes.IndexOf(checkBox);
            int numero = index + 1;

            if (e.Value)
            {
                if (selectedNumbers.Count < MaxSelectedNumbers)
                {
                    selectedNumbers.Add(numero);
                }
                else
                {
                    checkBox.IsChecked = false; // Si ya hay 6 números, desmarcar
                    return;
                }
            }
            else
            {
                selectedNumbers.Remove(numero);
            }

            ActualizarEstadoBotonSorteo();
            MostrarNumerosSeleccionados();
        }

        private void ActualizarEstadoBotonSorteo()
        {
            btnSorteo.IsEnabled = selectedNumbers.Count == MaxSelectedNumbers;
        }

        private void MostrarNumerosSeleccionados()
        {
            selectedNumbersLayout.Children.Clear();
            foreach (var numero in selectedNumbers)
            {
                selectedNumbersLayout.Children.Add(new Label { Text = numero.ToString() });
            }
        }

        private void OnSorteoClicked(object sender, EventArgs e)
        {
            var numerosPremiados = GenerarNumerosAleatorios();
            txtNumerosPremiados.Text = string.Join(", ", numerosPremiados);

            int aciertos = selectedNumbers.Intersect(numerosPremiados).Count();
            txtAciertos.Text = aciertos.ToString();

            // Deshabilitar checkboxes después del sorteo
            foreach (var checkBox in checkBoxes)
            {
                checkBox.IsEnabled = false;
            }

            // Habilitar botón Nuevo
            btnNuevo.IsEnabled = true;
        }

        private List<int> GenerarNumerosAleatorios()
        {
            Random random = new Random();
            return Enumerable.Range(1, MaxCheckBoxes).OrderBy(x => random.Next()).Take(MaxSelectedNumbers).ToList();
        }

        private void OnNuevoClicked(object sender, EventArgs e)
        {
            // Reiniciar el juego
            foreach (var checkBox in checkBoxes)
            {
                checkBox.IsChecked = false;
                checkBox.IsEnabled = true;
            }
            selectedNumbers.Clear();
            txtNumerosPremiados.Text = string.Empty;
            txtAciertos.Text = string.Empty;
            btnSorteo.IsEnabled = false;
            btnNuevo.IsEnabled = false;
            MostrarNumerosSeleccionados();
        }

        private void OnSalirClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
    }
}
