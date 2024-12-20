using Microsoft.Win32;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace KazychanovEyesSave
{
  public partial class AddEditPage : Page
  {
    private Agent _currentAgent = new Agent();
    private ProductSale _currentProductSale = new ProductSale();
    private CollectionViewSource _productsView;
    public AddEditPage(Agent SelectedAgent)
    {
      InitializeComponent();

      if (SelectedAgent != null)
        _currentAgent = SelectedAgent;

      DataContext = _productsView;
      DataContext = _currentAgent;

      ComboType.ItemsSource = KazychanovEyesSaveEntities.GetContext().AgentType.ToList();
      ComboType.DisplayMemberPath = "Title"; // Отображаемые названия
      ComboType.SelectedValuePath = "ID";   // Идентификатор для привязки
      ComboType.SelectedValue = _currentAgent.AgentTypeID; // Устанавливаем начальное значение

      int selectAgentID = _currentAgent.ID;
      var FilterSale = KazychanovEyesSaveEntities.GetContext().ProductSale.Where(sale => sale.AgentID == selectAgentID).ToList();
      Realize.ItemsSource = FilterSale;
      Realize.DisplayMemberPath = "Datacount";
      Realize.SelectedValuePath = "AgentID";

      _productsView = new CollectionViewSource();

      var products = KazychanovEyesSaveEntities.GetContext().Product.ToList();
      _productsView.Source = products;
      CBoxProducts.ItemsSource = _productsView.View; // Привязываем View к ComboBox
      CBoxProducts.DisplayMemberPath = "Title";
      CBoxProducts.SelectedValuePath = "ID";
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

    private void SearchProduct_TextChanged(object sender, TextChangedEventArgs e)
    {
      string searchText = SearchProduct.Text.ToLower();
      _productsView.View.Filter = o =>
      {
        Product p = o as Product;
        return p != null && p.Title.ToLower().Contains(searchText);
      };
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
      {
        try
        {
          if (Realize.SelectedItem != null) // Проверка на наличие выбранного элемента
          {
            ProductSale selectedHistory = (ProductSale)Realize.SelectedItem; // Получаем выбранный объект
            KazychanovEyesSaveEntities.GetContext().ProductSale.Remove(selectedHistory);
            KazychanovEyesSaveEntities.GetContext().SaveChanges();
            MessageBox.Show("Информация удалена!");
            Manager.MainFrame.GoBack();
          }
          else
          {
            MessageBox.Show("Пожалуйста, выберите запись для удаления.");
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message.ToString());
        }
      }
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
      StringBuilder errors = new StringBuilder();
      if (CBoxProducts.SelectedItem == null)
        errors.AppendLine("Укажите продукт");
      if (string.IsNullOrWhiteSpace(ProductCount.Text))
        errors.AppendLine("Укажите количество продуктов");
      bool isProductCountDigits = true;
      for (int i = 0; i < ProductCount.Text.Length; i++)
      {
        if (ProductCount.Text[i] < '0' || ProductCount.Text[i] > '9')
        {
          isProductCountDigits = false;
        }
      }
      if (!isProductCountDigits)
        errors.AppendLine("Укажите численное положительное продуктов");
      if (ProductCount.Text == "0")
      {
        errors.AppendLine("Укажите количество продаж");
      }
      if (string.IsNullOrWhiteSpace(SaleData.Text))
        errors.AppendLine("Укажите дату продажи");

      if (errors.Length > 0)
      {
        MessageBox.Show(errors.ToString());
        return;
      }
      _currentProductSale.AgentID = _currentAgent.ID;
      _currentProductSale.ProductID = CBoxProducts.SelectedIndex + 1;
      _currentProductSale.ProductCount = Convert.ToInt32(ProductCount.Text);
      _currentProductSale.SaleDate = Convert.ToDateTime(SaleData.Text);
      if (_currentProductSale.ID == 0)
        KazychanovEyesSaveEntities.GetContext().ProductSale.Add(_currentProductSale);

      try
      {
        KazychanovEyesSaveEntities.GetContext().SaveChanges();
        MessageBox.Show("информация сохранена");
        Manager.MainFrame.GoBack();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message.ToString());
      }
    }
  }
}
