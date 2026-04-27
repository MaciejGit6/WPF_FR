//xaml

<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp1"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="688">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- formularz -->
            <RowDefinition Height="*"/>     <!-- lista -->
            <RowDefinition Height="Auto"/>  <!-- przyciski -->
        </Grid.RowDefinitions>

        
        <Grid Grid.Row="0" Margin="5">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="80"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>

            <Label Grid.Row="0" Grid.Column="0" VerticalAlignment="Center">Imię:</Label>
            <TextBox Grid.Row="0" Grid.Column="1" Margin="0,3"
                     Text="{Binding NewPerson.Name, UpdateSourceTrigger=PropertyChanged}"/>

            <Label Grid.Row="1" Grid.Column="0" VerticalAlignment="Center">Wiek:</Label>
            <TextBox Grid.Row="1" Grid.Column="1" Margin="0,3"
                     Text="{Binding NewPerson.Age, UpdateSourceTrigger=PropertyChanged}"/>

            <Label Grid.Row="2" Grid.Column="0" VerticalAlignment="Center">Kolor:</Label>
            <TextBox Grid.Row="2" Grid.Column="1" Margin="0,3"
                     Text="{Binding NewPerson.Color, UpdateSourceTrigger=PropertyChanged}"/>
        </Grid>

        //lista
        <ListBox Grid.Row="1" Margin="5"
                 ItemsSource="{Binding People}"
                 SelectedItem="{Binding SelectedPerson}"
                 AlternationCount="2">

            <ListBox.ItemContainerStyle>
                <Style TargetType="ListBoxItem">
                    <Style.Triggers>
                        <Trigger Property="ItemsControl.AlternationIndex" Value="0">
                            <Setter Property="Background" Value="#FFAFC5FF"/>
                        </Trigger>
                        <Trigger Property="ItemsControl.AlternationIndex" Value="1">
                            <Setter Property="Background" Value="#FF75A1FF"/>
                        </Trigger>
                    </Style.Triggers>
                </Style>
            </ListBox.ItemContainerStyle>

            <ListBox.ItemTemplate>
                <DataTemplate>
                    <Border BorderBrush="Gray" BorderThickness="1"
                            Padding="5" Margin="2">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="Auto"/>
                                <ColumnDefinition Width="50"/>
                            </Grid.ColumnDefinitions>

                            //imie wiek
                            <StackPanel Grid.Column="0" VerticalAlignment="Center">
                                <TextBlock Text="{Binding Name}" FontSize="16" FontWeight="Bold"/>
                                <TextBlock Text="{Binding Age, StringFormat='Wiek: {0}'}"
                                           Foreground="Gray"/>
                            </StackPanel>

                            <!-- kolor jako tekst -->
                            <TextBlock Grid.Column="1" Text="{Binding Color}"
                                       VerticalAlignment="Center" Margin="5,0"/>

                            //kolor
                            <Border Grid.Column="2"
                                    Width="40" Height="40"
                                    BorderBrush="Black" BorderThickness="1"
                                    Background="{Binding Color}"/>
                        </Grid>
                    </Border>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>

        //buttons
        <StackPanel Grid.Row="2" Orientation="Horizontal"
                    HorizontalAlignment="Right" Margin="5">
            <Button Content="Dodaj" Width="80" Margin="5,0"
                    Click="Add_Click"/>
            <Button Content="Wyczyść" Width="80"
                    Click="Clear_Click"/>
        </StackPanel>

    </Grid>
</Window>

//main.xaml.cs

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify(string n) => PropertyChanged?.Invoke(this, new(n));

        public ObservableCollection<Person> People { get; set; } = new();

        private Person _newPerson = new();
        public Person NewPerson
        {
            get => _newPerson;
            set { _newPerson = value; Notify(nameof(NewPerson)); }
        }

        private Person? _selectedPerson;
        public Person? SelectedPerson
        {
            get => _selectedPerson;
            set { _selectedPerson = value; Notify(nameof(SelectedPerson)); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            People.Add(new Person
            {
                Name  = NewPerson.Name,
                Age   = NewPerson.Age,
                Color = NewPerson.Color
            });
            NewPerson = new Person(); // czyści formularz
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            People.Clear();
        }
    }
}

//person.cs

using System.ComponentModel;

namespace WpfApp1
{
    public class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify(string n) => PropertyChanged?.Invoke(this, new(n));

        private string _name = "";
        public string Name
        {
            get => _name;
            set { _name = value; Notify(nameof(Name)); }
        }

        private string _age = "";
        public string Age
        {
            get => _age;
            set { _age = value; Notify(nameof(Age)); }
        }

        private string _color = "#CCCCCC";
        public string Color
        {
            get => _color;
            set { _color = value; Notify(nameof(Color)); }
        }
    }
}