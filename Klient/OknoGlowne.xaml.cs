using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy OknoGlowne.xaml
    /// </summary>
    public partial class OknoGlowne : UserControl
    {
        //Pola Otrzymywane z zewnątrz 

        /// <summary>
        /// Pole przechowujące konfiguracje
        /// </summary>
        private Konfiguracja _conf;

        /// <summary>
        /// Pole przechowujące link do okna głównego
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// Lista przechowująca kontrolki podglądów dla klientów
        /// </summary>
        List<KlientPodgląd> klientPodgląds=new List<KlientPodgląd>();

        public OknoGlowne(Konfiguracja i_conf, MainWindow mw)
        {
            //Inicjalizacja
            InitializeComponent();
            
            //Przypisanie zmiennych
            _mainWindow = mw;
            _conf = i_conf;

            //Wpisanie informacji
            lbl_aktywni.Content = "Aktywni klienci: " + klientPodgląds.Count;
            lbl_ilosc.Content = "Dostępna ilość: " + _conf.IloscKlientow.ToString();
        }

        //Metody

        /// <summary>
        /// Metoda aktualizująca dane dotyczące klientów
        /// </summary>
        public void Aktualizuj(Konfiguracja i_conf)
        {
            _conf = i_conf;
            lbl_ilosc.Content = "Dostępna ilość: " + _conf.IloscKlientow.ToString();
            foreach(KlientPodgląd kP in klientPodgląds)
            {
                kP.Aktualizuj();
            }
        }

        /// <summary>
        /// Metoda wymuszająca przewania wątków w podglądach klientów
        /// </summary>
        public void ForeStop()
        {
            foreach(KlientPodgląd kP in klientPodgląds)
            {
                kP.ForceStop();
            }
        }

        //Reakcje na zdarzenia


        /// <summary>
        /// Dodanie nowego aktywnego klienta
        /// </summary>
        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
            int licznik = klientPodgląds.Count;
            if (licznik < _conf.IloscKlientow)
            {
                if (licznik > 0) {
                    dGridKlienci.Height += 130;
                    this.Height += 130;
                }
                dGridKlienci.RowDefinitions.Add(new RowDefinition());
                _mainWindow.DodajKlienta(new KlientRabbit(_conf.KlienciNazwy.ElementAt(licznik), _conf));
                klientPodgląds.Add(new KlientPodgląd(_mainWindow.PobierzKlienta(licznik)));
                Grid.SetRow(klientPodgląds.ElementAt(licznik), licznik);
                dGridKlienci.Children.Add(klientPodgląds.ElementAt(licznik));
                licznik++;
            }
            else
            {
                MessageBox.Show("Osiągnieta została maksymalna liczba klientów");
            }
            lbl_aktywni.Content = "Aktywni klienci: " + licznik.ToString(); 


        }

        /// <summary>
        /// Przejście do okna klonfiguracji
        /// </summary>
        private void btn_konfiguracja_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.PrzejdzDoKonfiguracji();
        }

        private void txt_addn_TextChanged(object sender, TextChangedEventArgs e)
        {
            int i;

            if (int.TryParse(txt_addn.Text, out i))
            {
                btn_add_n.Content = ("Dodaj: " + i);
                btn_add_n.IsEnabled = true;
                txt_addn.Background = new SolidColorBrush(Color.FromRgb(0,255 , 23));
            }
            else
            {
                btn_add_n.IsEnabled = false;
                txt_addn.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }

        private void btn_add_n_Click(object sender, RoutedEventArgs e)
        {
            int licznik = klientPodgląds.Count;
            int i = 0;
            int temp = licznik;

            if (!int.TryParse(txt_addn.Text, out i))
            {
                MessageBox.Show("Błędna liczba klientów do dodania");
                throw new Exception();
            }

            for (int j = 0; j < i; j++)
            {
                if (licznik < _conf.IloscKlientow)
                {
                    if (licznik > 0)
                    {
                        dGridKlienci.Height += 130;
                        this.Height += 130;
                    }
                    dGridKlienci.RowDefinitions.Add(new RowDefinition());
                    _mainWindow.DodajKlienta(new KlientRabbit(_conf.KlienciNazwy.ElementAt(licznik), _conf));
                    klientPodgląds.Add(new KlientPodgląd(_mainWindow.PobierzKlienta(licznik)));
                    Grid.SetRow(klientPodgląds.ElementAt(licznik), licznik);
                    dGridKlienci.Children.Add(klientPodgląds.ElementAt(licznik));
                    licznik++;
                    lbl_aktywni.Content = "Aktywni klienci: " + licznik.ToString();
                }
            }

            if (temp + i > _conf.IloscKlientow)
            {
                temp = _conf.IloscKlientow - temp;
                MessageBox.Show("Osiągnięto limit klientów \nDodano " + temp + " Klientów");
            }

        }

        private void btn_start_all_Click(object sender, RoutedEventArgs e)
        {
            foreach(KlientPodgląd i in klientPodgląds)
            {
                i.StartKlient();
            }
        }

        private void btn_stop_all_Click(object sender, RoutedEventArgs e)
        {
            foreach (KlientPodgląd i in klientPodgląds)
            {
                i.StopKlient();
            }
        }
    }
}
