using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Threading;
using Task3_2.Models;

namespace Task3_2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly CrossingModel _crossingModel;
        
        public ObservableCollection<CarViewModel> Cars { get; }
        public ObservableCollection<PedestrianViewModel> Pedestrians { get; }
        
        private string _trafficLightState;
        public string TrafficLightState
        {
            get => _trafficLightState;
            set => SetProperty(ref _trafficLightState, value);
        }
        
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        
        // Параметры пешеходного перехода
        public double CrossingX => _crossingModel?.CrossingX ?? 350;
        public double CrossingWidth => _crossingModel?.CrossingWidth ?? 80;
        public double RoadY => _crossingModel?.RoadY ?? 200;
        public double RoadHeight => _crossingModel?.RoadHeight ?? 100;

        public MainWindowViewModel()
        {
            Cars = new ObservableCollection<CarViewModel>();
            Pedestrians = new ObservableCollection<PedestrianViewModel>();
            
            _crossingModel = new CrossingModel();
            
            // Подписываемся на события моделей
            _crossingModel.Cars.CollectionChanged += CarsCollectionChanged;
            _crossingModel.Pedestrians.CollectionChanged += PedestriansCollectionChanged;
            _crossingModel.TrafficLight.StateChanged += TrafficLightStateChanged;
            _crossingModel.AccidentOccurred += CrossingModelOnAccidentOccurred;
            
            // Начальное состояние светофора
            TrafficLightState = _crossingModel.TrafficLight.CurrentState.ToString();
            StatusMessage = "Симуляция запущена";
            
            // Обновление положений объектов
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            
            timer.Tick += (s, e) =>
            {
                foreach (var car in Cars)
                {
                    car.UpdatePosition();
                }
                
                foreach (var pedestrian in Pedestrians)
                {
                    pedestrian.UpdatePosition();
                }
            };
            
            timer.Start();
        }

        private void CrossingModelOnAccidentOccurred(object sender, Car car)
        {
            Dispatcher.UIThread.Post(() =>
            {
                StatusMessage = "Произошла авария! Вызвана аварийная служба.";
                
                // Через 3 секунды сбрасываем сообщение
                DispatcherTimer.RunOnce(() =>
                {
                    StatusMessage = "Симуляция запущена";
                }, TimeSpan.FromSeconds(3));
            });
        }

        private void TrafficLightStateChanged(object sender, TrafficLight.LightState state)
        {
            Dispatcher.UIThread.Post(() =>
            {
                TrafficLightState = state.ToString();
            });
        }

        private void CarsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (e.NewItems != null)
                {
                    foreach (Car car in e.NewItems)
                    {
                        Cars.Add(new CarViewModel(car));
                    }
                }
                
                if (e.OldItems != null)
                {
                    foreach (Car car in e.OldItems)
                    {
                        var vmToRemove = Cars.FirstOrDefault(vm => vm.Model == car);
                        if (vmToRemove != null)
                        {
                            Cars.Remove(vmToRemove);
                        }
                    }
                }
                
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    Cars.Clear();
                }
            });
        }
        
        private void PedestriansCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (e.NewItems != null)
                {
                    foreach (Pedestrian pedestrian in e.NewItems)
                    {
                        Pedestrians.Add(new PedestrianViewModel(pedestrian));
                    }
                }
                
                if (e.OldItems != null)
                {
                    foreach (Pedestrian pedestrian in e.OldItems)
                    {
                        var vmToRemove = Pedestrians.FirstOrDefault(vm => vm.Model == pedestrian);
                        if (vmToRemove != null)
                        {
                            Pedestrians.Remove(vmToRemove);
                        }
                    }
                }
                
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    Pedestrians.Clear();
                }
            });
        }
    }

    // ViewModel для машины
    public class CarViewModel : ViewModelBase
    {
        public Car Model { get; }
        
        private double _x;
        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }
        
        private double _y;
        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }
        
        public bool IsEmergency => Model.IsEmergency;
        
        public CarViewModel(Car model)
        {
            Model = model;
            UpdatePosition();
        }
        
        public void UpdatePosition()
        {
            X = Model.X;
            Y = Model.Y;
        }
    }
    
    // ViewModel для пешехода
    public class PedestrianViewModel : ViewModelBase
    {
        public Pedestrian Model { get; }
        
        private double _x;
        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }
        
        private double _y;
        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }
        
        public PedestrianViewModel(Pedestrian model)
        {
            Model = model;
            UpdatePosition();
        }
        
        public void UpdatePosition()
        {
            X = Model.X;
            Y = Model.Y;
        }
    }
}