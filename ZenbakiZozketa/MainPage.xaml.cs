using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenbakiZozketa
{
    public partial class MainPage : ContentPage
    {
        // Definizio konstanteak Checkbox gehieneko kopururako eta hautatutako zenbakietarako
        const int MaxCheckBoxes = 49;
        const int MaxSelectedNumbers = 6;

        // CheckBox eta hautatutako zenbakiak biltegiratzeko zerrendak
        List<CheckBox> checkBoxes;
        List<int> selectedNumbers = new List<int>();

        public MainPage()
        {
            InitializeComponent();
            SortuCheckBoxes(); // sortu checkbox-ak
        }


        /// <summary>
        /// CheckBoxak sortzeko eta Grid batean antolatzeko metodoa
        /// </summary>
        private void SortuCheckBoxes()
        {
            checkBoxes = new List<CheckBox>();
            int columns = 7;
            int rows = (int)Math.Ceiling(MaxCheckBoxes / (double)columns);

            for (int i = 0; i < columns; i++)
            {
                checkboxGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < rows; i++)
            {
                checkboxGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            //Gehitu checkbox-ak grid-era
            for (int i = 1; i <= MaxCheckBoxes; i++)
            {
           
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

               
                Label label = new Label
                {
                    Text = i.ToString(), 
                    HorizontalOptions = LayoutOptions.Center
                };

                // sortu checkbox
                CheckBox checkBox = new CheckBox
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                checkBox.CheckedChanged += OnCheckBoxCheckedChanged;
                checkBoxes.Add(checkBox); 

                stackLayout.Children.Add(label);
                stackLayout.Children.Add(checkBox);

            
                checkboxGrid.Children.Add(stackLayout);
                Grid.SetColumn(stackLayout, (i - 1) % columns);
                Grid.SetRow(stackLayout, (i - 1) / columns);
            }
        }

        /// <summary>
        /// CheckBox baten egoera-aldaketako gertaera maneiatzen du.
        /// </summary>
        /// <param name="sender">aldatutako checkbox-a.</param>
        /// <param name="e">argumentuak.</param>
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
                    checkBox.IsChecked = false; 
                    return;
                }
            }
            else 
            {
                selectedNumbers.Remove(numero); 
            }

            EguneratuBotoiaSorteoa(); // Eguneratu botoia soerteo
            ErakutsiAukeratutakoZenbakiak(); // Erakutsi auekratutakoak
        }
        /// <summary>
        /// Zozketa-botoia gaitu edo desgaitu egiten du, hautatutakoen kopuruaren arabera.
        /// </summary>
        private void EguneratuBotoiaSorteoa()
        {
            btnSorteoa.IsEnabled = selectedNumbers.Count == MaxSelectedNumbers; 
        }

        /// <summary>
        /// TextBoxetan hautatutako zenbakiak erakusten ditu.
        /// </summary>
        private void ErakutsiAukeratutakoZenbakiak()
        {
            var textBoxes = new List<Entry> { txtNumero1, txtNumero2, txtNumero3, txtNumero4, txtNumero5, txtNumero6 };

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (i < selectedNumbers.Count)
                {
                    textBoxes[i].Text = selectedNumbers[i].ToString();
                }
                else
                {
                    textBoxes[i].Text = string.Empty; 
                }
            }
        }

        /// <summary>
        /// Zozketa botoia menjatzen duen metodoa
        /// </summary>
        /// <param name="sender">Sakatutako botoia.</param>
        /// <param name="e">Argumentuak.</param>
        private void OnSorteoClicked(object sender, EventArgs e)
        {
            var premiatutakozenbakiak = AusazkoZenbakiakSortu();
            txtPremiatutakozenbakiak.Text = string.Join(", ", premiatutakozenbakiak); // Erakutsi premiatutaok zenbakiak

            int asmatutakokop = selectedNumbers.Intersect(premiatutakozenbakiak).Count(); // kalkulatu asmatutakoak
            txtAsmatutakoak.Text = asmatutakokop.ToString(); // Erakutsi asmatutako kopurua

            // Premioa ezarri
            string premioa = DeterminarPremio(asmatutakokop);
            DisplayAlert("Premioa", premioa, "Ados"); // Erakutsi premioa

            
            foreach (var checkBox in checkBoxes)
            {
                checkBox.IsEnabled = false; 
            }


            btnBerria.IsEnabled = true; 
        }

        /// <summary>
        /// Ausazko zenbkiak sortzeko metodoa
        /// </summary>
        private List<int> AusazkoZenbakiakSortu()
        {
            Random random = new Random(); 
            return Enumerable.Range(1, MaxCheckBoxes).OrderBy(x => random.Next()).Take(MaxSelectedNumbers).ToList();
        }

        /// <summary>
        /// Premioa ezartzen du asmatutako kopuruaren arabera
        /// </summary>
        /// <param name="aciertos">Asmatutako kopurua.</param>
        /// <returns>Premioa.</returns>
        private string DeterminarPremio(int aciertos)
        {
            return aciertos switch
            {
                0 => "Ez duzua smatu saiatu berriro.", //0
                1 => "Premioa: % 10eko deskontu-kupoia.", // 1 
                2 => "Premio: Ezusteko opari txiki bat.", // 2 
                3 => "Premio: Doako edari baterako balea.", // 3 
                4 => "Premio: Postre baterako bale bat.", // 4 
                5 => "Premio: Afari bat doan egiteko txartela.", // 5 
                6 => "Premio: \r\nSari handia! Asteburuko bidaia\r\n.", // 6 
                _ => "Definitu gabeko premioa."
            };
        }

        /// <summary>
        ///Partida berri abt egiteko botoia manejatzen duen metodoa
        /// </summary>
        /// <param name="sender">Sakatutako botoia.</param>
        /// <param name="e">Argumentuak.</param>
        private void OnNuevoClicked(object sender, EventArgs e)
        {
       
            foreach (var checkBox in checkBoxes)
            {
                checkBox.IsChecked = false; 
                checkBox.IsEnabled = true; 
            }
            selectedNumbers.Clear();
            txtPremiatutakozenbakiak.Text = string.Empty;
            txtAsmatutakoak.Text = string.Empty;


            ErakutsiAukeratutakoZenbakiak();

            btnSorteoa.IsEnabled = false;
            btnBerria.IsEnabled = false; 
        }

        /// <summary>
        /// Irten botoia manejatzen duen metodoa
        /// </summary>
        /// <param name="sender">Sakatutako botoia.</param>
        /// <param name="e">Argumentuak.</param>
        private void OnSalirClicked(object sender, EventArgs e)
        {
            Application.Current!.Quit(); // Cerrar la aplicación
        }
    }
}
