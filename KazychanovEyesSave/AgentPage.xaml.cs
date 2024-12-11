using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;



namespace KazychanovEyesSave
{



  public partial class AgentPage : Page
  {

    int CountRecords;
    int CountPage;
    int CurrentPage = 0;

    List<Agent> CurrentPageList = new List<Agent>();
    List<Agent> TableList;

    public AgentPage()
    {


      InitializeComponent();

      var _currentAgent = KazychanovEyesSaveEntities.GetContext().Agent.ToList();

      AgentListView.ItemsSource = _currentAgent;

      ComboType.SelectedIndex = 0;
      ComboSorting.SelectedIndex = 0;

      UpdateAgents();
    }

    public void UpdateAgents()
    {
      var _currentAgent = KazychanovEyesSaveEntities.GetContext().Agent.ToList();
      _currentAgent = _currentAgent.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()) || 
      p.Phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("+", "")
      .Contains(TBoxSearch.Text.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("+", ""))).ToList();


      switch (ComboSorting.SelectedIndex)
      {
        case 1:
          _currentAgent = _currentAgent.OrderBy(agent => agent.Title).ToList();
          break;
        case 2:
          _currentAgent = _currentAgent.OrderByDescending(agent => agent.Title).ToList();
          break;
        case 5:
          _currentAgent = _currentAgent.OrderBy(agent => agent.Priority).ToList();
          break;
        case 6:
          _currentAgent = _currentAgent.OrderByDescending(agent => agent.Priority).ToList();
          break;
        default:
          break;
      }

      switch (ComboType.SelectedIndex)
      {
        case 1:
          _currentAgent = _currentAgent.Where(agent => agent.AgentTypeTitle == "МФО").ToList();
          break;
        case 2:
          _currentAgent = _currentAgent.Where(agent => agent.AgentTypeTitle == "ООО").ToList();
          break;
        case 3:
          _currentAgent = _currentAgent.Where(agent => agent.AgentTypeTitle == "ЗАО").ToList();
          break;
        case 4:
          _currentAgent = _currentAgent.Where(agent => agent.AgentTypeTitle == "МКК").ToList();
          break;
        case 5:
          _currentAgent = _currentAgent.Where(agent => agent.AgentTypeTitle == "ОАО").ToList();
          break;
        case 6:
          _currentAgent = _currentAgent.Where(agent => agent.AgentTypeTitle == "ПАО").ToList();
          break;
        default:
          break;
      }


      AgentListView.ItemsSource = _currentAgent.ToList();
      TableList = _currentAgent;
      ChangePage(0, 0);
    }

    private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateAgents();
    }

    private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdateAgents();
    }

    private void ComboSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdateAgents();
    }

    private void ChangePage(int direction, int? selectedPage)
    {
      CurrentPageList.Clear();
      CountRecords = TableList.Count;
      CountPage = (CountRecords + 9) / 10;  // Сократили формулу вычисления числа страниц

      if (selectedPage.HasValue && selectedPage >= 0 && selectedPage < CountPage)
      {
        CurrentPage = selectedPage.Value;
      }
      else
      {
        if (direction == 1 && CurrentPage > 0)
          CurrentPage--;
        else if (direction == 2 && CurrentPage < CountPage - 1)
          CurrentPage++;
        else
          return; // Нет изменений, можно выйти
      }

      int startIndex = CurrentPage * 10;
      int endIndex = Math.Min(startIndex + 10, CountRecords);

      for (int i = startIndex; i < endIndex; i++)
      {
        CurrentPageList.Add(TableList[i]);
      }

      // Обновление графического интерфейса
      PageListBox.Items.Clear();
      for (int i = 0; i < CountPage; i++)
      {
        PageListBox.Items.Add(i + 1);
      }
      PageListBox.SelectedIndex = CurrentPage;

      TBCount.Text = endIndex.ToString();
      TBAllRecords.Text = $" из {CountRecords}";

      AgentListView.ItemsSource = CurrentPageList;
      AgentListView.Items.Refresh();
    }



    private void ListBoxPage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      ChangePage(0,Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
    }
    private void BtnLeftDir_Click(object sender, RoutedEventArgs e)
    {
      ChangePage(1, null);
    }

    private void BtnRightDir_Click(object sender, RoutedEventArgs e)
    {
      ChangePage(2, null);
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
      Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));

    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
      Manager.MainFrame.Navigate(new AddEditPage(null));

    }

    private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (Visibility == Visibility.Visible)
      {
        KazychanovEyesSaveEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
        AgentListView.ItemsSource = KazychanovEyesSaveEntities.GetContext().Agent.ToList();
        UpdateAgents();
      }
    }
  }
}