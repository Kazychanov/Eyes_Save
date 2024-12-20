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
using System.Windows.Shapes;

namespace KazychanovEyesSave
{
  /// <summary>
  /// Логика взаимодействия для SetPriory.xaml
  /// </summary>
  public partial class SetPriory : Window
  {
    public SetPriory(int Maxnumber)
    {
      InitializeComponent();
      PriorySet.Text = Maxnumber.ToString();
    }

    private void SaveBut_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
