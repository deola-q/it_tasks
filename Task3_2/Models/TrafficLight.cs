using System;

namespace Task3_2.Models
{
    public class TrafficLight
    {
        public enum LightState
        {
            RedForCars,    // Красный для машин (зеленый для пешеходов)
            GreenForCars   // Зеленый для машин (красный для пешеходов)
        }

        private LightState _currentState;
        private readonly Random _random = new Random();
        private DateTime _lastStateChange;
        
        private readonly int _minStateTime = 8000;  // 8 секунд
        private readonly int _maxStateTime = 12000; // 12 секунд
        private int _currentStateDuration;

        public LightState CurrentState => _currentState;
        
        public event EventHandler<LightState> StateChanged;

        public TrafficLight()
        {
            _currentState = LightState.RedForCars; // Начинаем с красного для машин
            _lastStateChange = DateTime.Now;
            _currentStateDuration = _random.Next(_minStateTime, _maxStateTime);
        }

        public void Update()
        {
            if ((DateTime.Now - _lastStateChange).TotalMilliseconds >= _currentStateDuration)
            {
                // Меняем сигнал светофора
                _currentState = _currentState == LightState.RedForCars 
                    ? LightState.GreenForCars 
                    : LightState.RedForCars;
                    
                _lastStateChange = DateTime.Now;
                _currentStateDuration = _random.Next(_minStateTime, _maxStateTime);
                
                // Уведомляем подписчиков о смене сигнала
                StateChanged?.Invoke(this, _currentState);
            }
        }
    }
}