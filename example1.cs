Student.cs
using System.ComponentModel;

namespace WpfApp1
{
    public class Student : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify(string n) => PropertyChanged?.Invoke(this, new(n));

        private string _name = "";
        public string Name
        {
            get => _name;
            set { _name = value; Notify(nameof(Name)); }
        }

        private string _subject = "";
        public string Subject
        {
            get => _subject;
            set { _subject = value; Notify(nameof(Subject)); }
        }

        private string _grade = "";
        public string Grade
        {
            get => _grade;
            set
            {
                _grade = value;
                Notify(nameof(Grade));
                Notify(nameof(GradeColor)); // <-- wyliczana zależy od Grade
            }
        }

        //Wyliczana property — nie ma settera, tylko getter
        //UI wiąże się z nią jak z każdą inną
        public string GradeColor => Grade switch
        {
            "5" => "#FF4CAF50",  // zielony
            "4" => "#FF8BC34A",  // jasny zielony
            "3" => "#FFFFC107",  // żółty
            "2" => "#FFFF5722",  // czerwony
            _   => "#FFCCCCCC"   // szary (nieznana)
        };
    }
}

--------------------------------------Mainwindowxaml.cs
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Notify(string n) => PropertyChanged?.Invoke(this, new(n));

        public ObservableCollection<Student> Students { get; set; } = new();

        private Student _newStudent = new();
        public Student NewStudent
        {
            get => _newStudent;
            set { _newStudent = value; Notify(nameof(NewStudent)); }
        }

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set { _selectedStudent = value; Notify(nameof(SelectedStudent)); }
        }

        //Wyliczana na podstawie całej kolekcji
        public string AverageGrade
        {
            get
            {
                var valid = Students
                    .Where(s => double.TryParse(s.Grade, out _))
                    .Select(s => double.Parse(s.Grade))
                    .ToList();

                return valid.Count == 0
                    ? "Brak"
                    : valid.Average().ToString("F2");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            //Gdy kolekcja się zmienia → obliczanie średniej
            Students.CollectionChanged += (s, e) => Notify(nameof(AverageGrade));
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewStudent.Name)) return;

            Students.Add(new Student
            {
                Name    = NewStudent.Name,
                Subject = NewStudent.Subject,
                Grade   = NewStudent.Grade
            });

            NewStudent = new Student();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent != null)
                Students.Remove(SelectedStudent);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Students.Clear();
        }
    }
}
-----------------------xaml

<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:WpfApp1"
        mc:Ignorable="d"
        Title="Oceny studentów" Height="550" Width="650">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- formularz -->
            <RowDefinition Height="*"/>     <!-- lista -->
            <RowDefinition Height="Auto"/>  <!-- szczegóły -->
            <RowDefinition Height="Auto"/>  <!-- średnia -->
            <RowDefinition Height="Auto"/>  <!-- przyciski -->
        </Grid.RowDefinitions>

        //formularz
        <Border Grid.Row="0" BorderBrush="LightGray" BorderThickness="1"
                Margin="5" Padding="8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="75"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="75"/>
                    <ColumnDefinition Width="80"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Label Grid.Row="0" Grid.Column="0" VerticalAlignment="Center">Nazwisko:</Label>
                <TextBox Grid.Row="0" Grid.Column="1" Margin="0,3"
                         Text="{Binding NewStudent.Name, UpdateSourceTrigger=PropertyChanged}"/>

                <Label Grid.Row="0" Grid.Column="2" VerticalAlignment="Center">Ocena:</Label>
                <TextBox Grid.Row="0" Grid.Column="3" Margin="5,3,0,3"
                         Text="{Binding NewStudent.Grade, UpdateSourceTrigger=PropertyChanged}"/>

                <Label Grid.Row="1" Grid.Column="0" VerticalAlignment="Center">Przedmiot:</Label>
                <TextBox Grid.Row="1" Grid.Column="1" Margin="0,3"
                         Text="{Binding NewStudent.Subject, UpdateSourceTrigger=PropertyChanged}"/>
            </Grid>
        </Border>

        //lista
        <ListBox Grid.Row="1" Margin="5"
                 ItemsSource="{Binding Students}"
                 SelectedItem="{Binding SelectedStudent}"
                 AlternationCount="2">

            <ListBox.ItemContainerStyle>
                <Style TargetType="ListBoxItem">
                    <Style.Triggers>
                        <Trigger Property="ItemsControl.AlternationIndex" Value="0">
                            <Setter Property="Background" Value="#FFF5F5F5"/>
                        </Trigger>
                        <Trigger Property="ItemsControl.AlternationIndex" Value="1">
                            <Setter Property="Background" Value="#FFE3F2FD"/>
                        </Trigger>
                    </Style.Triggers>
                </Style>
            </ListBox.ItemContainerStyle>

            <ListBox.ItemTemplate>
                <DataTemplate>
                    <Border BorderBrush="LightGray" BorderThickness="1"
                            Padding="6" Margin="2">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="120"/>
                                <ColumnDefinition Width="50"/>
                            </Grid.ColumnDefinitions>

                            //nazwisk
                            <TextBlock Grid.Column="0"
                                       Text="{Binding Name}"
                                       FontSize="15" FontWeight="Bold"
                                       VerticalAlignment="Center"/>

                            //przedmiot
                            <Border Grid.Column="1"
                                    BorderBrush="LightGray" BorderThickness="1"
                                    Padding="4" Margin="4,0"
                                    VerticalAlignment="Center">
                                <TextBlock Text="{Binding Subject}"
                                           Foreground="Gray"
                                           VerticalAlignment="Center"/>
                            </Border>

                            //ocena — kolor zależy od wartości (GradeColor) -->
                            <Border Grid.Column="2"
                                    Width="42" Height="42"
                                    BorderBrush="Black" BorderThickness="1"
                                    Background="{Binding GradeColor}">
                                <TextBlock Text="{Binding Grade}"
                                           FontSize="20" FontWeight="Bold"
                                           HorizontalAlignment="Center"
                                           VerticalAlignment="Center"/>
                            </Border>
                        </Grid>
                    </Border>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>

        //SZCZEGÓŁY — edycja zaznaczonego ===== -->
        <Border Grid.Row="2" Margin="5" Padding="8"
                BorderBrush="SteelBlue" BorderThickness="2"
                DataContext="{Binding SelectedStudent}">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="75"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="75"/>
                    <ColumnDefinition Width="80"/>
                </Grid.ColumnDefinitions>

                <TextBlock Grid.Column="0" VerticalAlignment="Center">Nazwisko:</TextBlock>
                <TextBox Grid.Column="1" Margin="0,3"
                         Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}"/>

                <TextBlock Grid.Column="2" VerticalAlignment="Center">Ocena:</TextBlock>
                <TextBox Grid.Column="3" Margin="5,3,0,3"
                         Text="{Binding Grade, UpdateSourceTrigger=PropertyChanged}"/>
            </Grid>
        </Border>

        //ŚREDNIA — wyliczana z kolekcji ===== -->
        <Border Grid.Row="3" Margin="5,0,5,5" Padding="8"
                BorderBrush="LightGray" BorderThickness="1">
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="Średnia ocen: " FontWeight="Bold" FontSize="14"/>
                <TextBlock Text="{Binding AverageGrade}"
                           FontSize="14" Foreground="SteelBlue"/>
            </StackPanel>
        </Border>

        //buttons
        <StackPanel Grid.Row="4" Orientation="Horizontal"
                    HorizontalAlignment="Right" Margin="5">
            <Button Content="Dodaj" Width="80" Margin="5,0"
                    Click="Add_Click"/>
            <Button Content="Usuń" Width="80" Margin="5,0"
                    Click="Delete_Click"/>
            <Button Content="Wyczyść" Width="80"
                    Click="Clear_Click"/>
        </StackPanel>
    </Grid>
</Window>