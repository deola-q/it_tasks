using System;
using System.Collections.ObjectModel;
using System.Timers;

namespace Task3_2.Models
{
    public class CrossingModel : IDisposable
    {
        // Параметры модели 
        public double CrossingX { get; } = 350;
        public double CrossingWidth { get; } = 80;
        public double RoadY { get; } = 200;
        public double RoadHeight { get; } = 100;
        
        public TrafficLight TrafficLight { get; }
        private IEmergencyService _emergencyService;
        
        private readonly Timer _updateTimer;
        private readonly Timer _carSpawnTimer;
        private readonly Timer _pedestrianSpawnTimer;
        
        public ObservableCollection<Car> Cars { get; }
        public ObservableCollection<Pedestrian> Pedestrians { get; }
        
        private readonly Random _random = new Random();
        
        public event EventHandler<Car> AccidentOccurred;

        public CrossingModel()
        {
            TrafficLight = new TrafficLight();
            _emergencyService = new EmergencyService();
            
            // Подписываемся на событие прибытия аварийной машины
            ((EmergencyService)_emergencyService).EmergencyVehicleArrived += (sender, car) =>
            {
                if (!Cars.Contains(car))
                {
                    Cars.Add(car);
                }
            };
            
            Cars = new ObservableCollection<Car>();
            Pedestrians = new ObservableCollection<Pedestrian>();
            
            _updateTimer = new Timer(50);
            _updateTimer.Elapsed += UpdateObjects;
            _updateTimer.AutoReset = true;
            _updateTimer.Start();
            
            _carSpawnTimer = new Timer(2000);
            _carSpawnTimer.Elapsed += SpawnCar;
            _carSpawnTimer.AutoReset = true;
            _carSpawnTimer.Start();
            
            _pedestrianSpawnTimer = new Timer(3000);
            _pedestrianSpawnTimer.Elapsed += SpawnPedestrian;
            _pedestrianSpawnTimer.AutoReset = true;
            _pedestrianSpawnTimer.Start();
        }

        private void UpdateObjects(object sender, ElapsedEventArgs e)
        {
            // Обновление светофора
            TrafficLight.Update();
            
            // Безопасное обновление машин
            for (int i = Cars.Count - 1; i >= 0; i--)
            {
                if (i < Cars.Count)
                {
                    Cars[i].Update();
                }
            }
            
            // Безопасное обновление пешеходов
            for (int i = Pedestrians.Count - 1; i >= 0; i--)
            {
                if (i < Pedestrians.Count)
                {
                    Pedestrians[i].Update();
                }
            }
        }

        private void SpawnCar(object sender, ElapsedEventArgs e)
        {
            // Ограничиваем количество машин на экране для избежания перегрузки
            if (Cars.Count >= 10) return;
            
            // Создаем обычную машину с небольшим шансом на аварийную
            bool isEmergency = _random.Next(0, 100) < 5;
            var car = new Car(this, isEmergency);
            Cars.Add(car);
        }

        private void SpawnPedestrian(object sender, ElapsedEventArgs e)
        {
            // Ограничиваем количество пешеходов для лучшей производительности
            if (Pedestrians.Count >= 8) return;
            
            var pedestrian = new Pedestrian(this);
            Pedestrians.Add(pedestrian);
        }

        public void RemoveCar(Car car)
        {
            if (Cars.Contains(car))
            {
                Cars.Remove(car);
            }
        }

        public void RemovePedestrian(Pedestrian pedestrian)
        {
            if (Pedestrians.Contains(pedestrian))
            {
                Pedestrians.Remove(pedestrian);
            }
        }

        public async void TriggerAccident(Car car)
        {
            // Генерируем событие аварии
            AccidentOccurred?.Invoke(this, car);
            
            // Определяем, с какой стороны появится аварийная машина - в зависимости от полосы
            // На верхней полосе (Y около 220) машины едут слева направо, поэтому аварийка появляется слева
            // На нижней полосе (Y около 260) машины едут справа налево, поэтому аварийка появляется справа
            bool fromLeft = car.Y < 230; // True для верхней полосы, False для нижней
            
            // Вызываем аварийную службу через интерфейс
            await _emergencyService.RespondToAccident(
                fromLeft ? -50 : 850, // Позиция появления аварийной машины
                car.Y // На той же высоте, что и попавшая в аварию машина
            );
        }

        public void Dispose()
        {
            _updateTimer.Stop();
            _updateTimer.Dispose();
            _carSpawnTimer.Stop();
            _carSpawnTimer.Dispose();
            _pedestrianSpawnTimer.Stop();
            _pedestrianSpawnTimer.Dispose();
        }
    }
}