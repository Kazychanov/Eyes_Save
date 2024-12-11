using Microsoft.Win32;
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

namespace KazychanovEyesSave
{
  public partial class AddEditPage : Page
  {
    private Agent _currentAgent = new Agent();
    public AddEditPage(Agent SelectedAgent)
    {
      InitializeComponent();

      if (SelectedAgent != null)
        _currentAgent = SelectedAgent;


      DataContext = _currentAgent;

      ComboType.ItemsSource = KazychanovEyesSaveEntities.GetContext().AgentType.ToList();
      ComboType.DisplayMemberPath = "Title"; // Отображаемые названия
      ComboType.SelectedValuePath = "ID";   // Идентификатор для привязки
      ComboType.SelectedValue = _currentAgent.AgentTypeID; // Устанавливаем начальное значение
    }

    private void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
      StringBuilder errors = new StringBuilder();

      if (ComboType.SelectedItem == null)
        errors.AppendLine("Укажите тип агента");
      if (string.IsNullOrWhiteSpace(_currentAgent.Title))
        errors.AppendLine("Укажите Наименование агента");
      if (string.IsNullOrWhiteSpace(_currentAgent.Email))
        errors.AppendLine("Укажите почту агента");
      if (string.IsNullOrWhiteSpace(_currentAgent.Address))
        errors.AppendLine("Укажите адресс агента");
      if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName))
        errors.AppendLine("Укажите ФИО директора");
      if (string.IsNullOrWhiteSpace(_currentAgent.Priority.ToString()))
        errors.AppendLine("Укажите приоритет");
      if (_currentAgent.Priority <= 0)
        errors.AppendLine("Укажите положительный приоритет агента");
      if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
        errors.AppendLine("Укажите КПП агента");
      if (string.IsNullOrWhiteSpace(_currentAgent.INN))
        errors.AppendLine("Укажите ИНН агента");
      if (string.IsNullOrWhiteSpace(_currentAgent.Phone))
        errors.AppendLine("Укажите телефон агента");
      else
      {
        string ph = _currentAgent.Phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("+", "");
        if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11 && (ph[0] == '7' || ph[0] == '8'))
            || (ph[1] == '3' && ph.Length != 12) && (ph[0] == '7' || ph[0] == '8'))
          errors.AppendLine("Укажите правильно телефон агента");
      }
      if (errors.Length > 0)
      {
        MessageBox.Show(errors.ToString());
        return;
      }

        if (_currentAgent.ID == 0)
        {
          KazychanovEyesSaveEntities.GetContext().Agent.Add(_currentAgent);
        }
        try
        {
          KazychanovEyesSaveEntities.GetContext().SaveChanges();
          MessageBox.Show("Информация сохранена");
          Manager.MainFrame.GoBack();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message.ToString());
        }
      


    }
    private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog myOpenFileDialog = new OpenFileDialog();
      if (myOpenFileDialog.ShowDialog() == true)
      {
        _currentAgent.Logo = myOpenFileDialog.FileName;
        LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
      }
    }


    private void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {

      var currentProductSale = KazychanovEyesSaveEntities.GetContext().ProductSale.ToList();
      currentProductSale = currentProductSale.Where(p => p.AgentID == _currentAgent.ID).ToList();

      if (currentProductSale.Count != 0)
      {
        MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу");
        return;
      }
      if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
          MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
      {
        try
        {
          KazychanovEyesSaveEntities.GetContext().Agent.Remove(_currentAgent);
          KazychanovEyesSaveEntities.GetContext().SaveChanges();
          Manager.MainFrame.GoBack();
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message.ToString());
        }
      }
      
    }


    private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      
    }
  }
}
