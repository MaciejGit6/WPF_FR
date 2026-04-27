//book.cs

using System.ComponentModel;

namespace WpfApp1
{
    public class Book : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify(string n) => PropertyChanged?.Invoke(this, new(n));

        private string _title = "";
        public string Title
        {
            get => _title;
            set { _title = value; Notify(nameof(Title)); }
        }

        private string _author = "";
        public string Author
        {
            get => _author;
            set { _author = value; Notify(nameof(Author)); }
        }

        private string _rating = "";
        public string Rating
        {
            get => _rating;
            set { _rating = value; Notify(nameof(Rating)); }
        }

        private string _color = "#CCCCCC";
        public string Color
        {
            get => _color;
            set { _color = value; Notify(nameof(Color)); }
        }
    }
}

//main.cs

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify(string n) => PropertyChanged?.Invoke(this, new(n));

        public ObservableCollection<Book> Books { get; set; } = new();

        private Book _newBook = new();
        public Book NewBook
        {
            get => _newBook;
            set { _newBook = value; Notify(nameof(NewBook)); }
        }

        private Book? _selectedBook;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; Notify(nameof(SelectedBook)); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewBook.Title)) return;

            Books.Add(new Book
            {
                Title  = NewBook.Title,
                Author = NewBook.Author,
                Rating = NewBook.Rating,
                Color  = NewBook.Color
            });

            NewBook = new Book();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBook != null)
                Books.Remove(SelectedBook);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Books.Clear();
        }
    }
}

//xaml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp1"
        mc:Ignorable="d"
        Title="Book List" Height="550" Width="700">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- formularz dodawania -->
            <RowDefinition Height="*"/>     <!-- lista -->
            <RowDefinition Height="Auto"/>  <!-- szczegóły wybranej książki -->
            <RowDefinition Height="Auto"/>  <!-- przyciski -->
        </Grid.RowDefinitions>

        //formularz
        <Border Grid.Row="0" BorderBrush="LightGray" BorderThickness="1" Margin="5" Padding="8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="70"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="70"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Label Grid.Row="0" Grid.Column="0" VerticalAlignment="Center">Tytuł:</Label>
                <TextBox Grid.Row="0" Grid.Column="1" Margin="0,3"
                         Text="{Binding NewBook.Title, UpdateSourceTrigger=PropertyChanged}"/>

                <Label Grid.Row="0" Grid.Column="2" VerticalAlignment="Center">Autor:</Label>
                <TextBox Grid.Row="0" Grid.Column="3" Margin="0,3"
                         Text="{Binding NewBook.Author, UpdateSourceTrigger=PropertyChanged}"/>

                <Label Grid.Row="1" Grid.Column="0" VerticalAlignment="Center">Ocena:</Label>
                <TextBox Grid.Row="1" Grid.Column="1" Margin="0,3"
                         Text="{Binding NewBook.Rating, UpdateSourceTrigger=PropertyChanged}"/>

                <Label Grid.Row="1" Grid.Column="2" VerticalAlignment="Center">Kolor:</Label>
                <TextBox Grid.Row="1" Grid.Column="3" Margin="0,3"
                         Text="{Binding NewBook.Color, UpdateSourceTrigger=PropertyChanged}"/>
            </Grid>
        </Border>

        //list
        <ListBox Grid.Row="1" Margin="5"
                 ItemsSource="{Binding Books}"
                 SelectedItem="{Binding SelectedBook}"
                 AlternationCount="2">

            <ListBox.ItemContainerStyle>
                <Style TargetType="ListBoxItem">
                    <Style.Triggers>
                        <Trigger Property="ItemsControl.AlternationIndex" Value="0">
                            <Setter Property="Background" Value="#FFF0F0F0"/>
                        </Trigger>
                        <Trigger Property="ItemsControl.AlternationIndex" Value="1">
                            <Setter Property="Background" Value="#FFD6EAF8"/>
                        </Trigger>
                    </Style.Triggers>
                </Style>
            </ListBox.ItemContainerStyle>

            <ListBox.ItemTemplate>
                <DataTemplate>
                    <!-- DataContext tu = jeden Book -->
                    <Border BorderBrush="Gray" BorderThickness="1"
                            Padding="6" Margin="2">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="80"/>
                                <ColumnDefinition Width="40"/>
                            </Grid.ColumnDefinitions>

                            //title author
                            <StackPanel Grid.Column="0" VerticalAlignment="Center">
                                <TextBlock Text="{Binding Title}"
                                           FontSize="15" FontWeight="Bold"/>
                                <TextBlock Text="{Binding Author}"
                                           Foreground="Gray" FontStyle="Italic"/>
                            </StackPanel>

                            //grade
                            <Border Grid.Column="1"
                                    BorderBrush="DarkGray" BorderThickness="1"
                                    Margin="5,0" Padding="4"
                                    VerticalAlignment="Center"
                                    HorizontalAlignment="Center">
                                <TextBlock Text="{Binding Rating, StringFormat='⭐ {0}'}"
                                           FontSize="13"/>
                            </Border>

                            //kolor
                            <Border Grid.Column="2"
                                    Width="35" Height="35"
                                    BorderBrush="Black" BorderThickness="1"
                                    Background="{Binding Color}"/>
                        </Grid>
                    </Border>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>

        //SZCZEGÓŁY WYBRANEJ KSIĄŻKI 
        //DataContext zmieniony na SelectedBook dla całego panelu 
        <Border Grid.Row="2" Margin="5" Padding="8"
                BorderBrush="SteelBlue" BorderThickness="2"
                DataContext="{Binding SelectedBook}">
            <StackPanel>
                <TextBlock Text="Wybrana książka:" FontWeight="Bold" Margin="0,0,0,4"/>
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="60"/>
                        <ColumnDefinition Width="*"/>
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                    </Grid.RowDefinitions>

                    <TextBlock Grid.Row="0" Grid.Column="0">Tytuł:</TextBlock>
                    <!-- Edytowalne pola — zmiana tu = zmiana na liście od razu -->
                    <TextBox Grid.Row="0" Grid.Column="1"
                             Text="{Binding Title, UpdateSourceTrigger=PropertyChanged}"/>

                    <TextBlock Grid.Row="1" Grid.Column="0" Margin="0,3,0,0">Autor:</TextBlock>
                    <TextBox Grid.Row="1" Grid.Column="1" Margin="0,3,0,0"
                             Text="{Binding Author, UpdateSourceTrigger=PropertyChanged}"/>
                </Grid>
            </StackPanel>
        </Border>

        //buttons
        <StackPanel Grid.Row="3" Orientation="Horizontal"
                    HorizontalAlignment="Right" Margin="5">
            <Button Content="Dodaj" Width="80" Margin="5,0"
                    Click="Add_Click"/>
            <Button Content="Usuń zaznaczoną" Width="120" Margin="5,0"
                    Click="Delete_Click"/>
            <Button Content="Wyczyść" Width="80"
                    Click="Clear_Click"/>
        </StackPanel>
    </Grid>
</Window>