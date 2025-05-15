using System;

namespace Task3_2.Models
{
    public class Pedestrian
    {
        private readonly Random _random = new Random();
        private readonly CrossingModel _crossing;
        
        public double X { get; private set; }
        public double Y { get; private set; }
        
        private bool _movingDown;
        private readonly double _speed;
        private bool _isWaiting;
        private bool _crossingStarted;

        public Pedestrian(CrossingModel crossing)
        {
            _crossing = crossing;
            _speed = _random.Next(1, 3);
            _movingDown = _random.Next(0, 2) == 0;
            
            // Располагаем пешехода перед переходом
            Y = _movingDown ? _crossing.RoadY - 20 : _crossing.RoadY + _crossing.RoadHeight + 5;
            
            // Пешеход появляется рядом с переходом
            X = _crossing.CrossingX + _random.Next(0, (int)_crossing.CrossingWidth);
            
            // Пешеход ждет, если для машин горит зеленый (пешеходам красный)
            _isWaiting = _crossing.TrafficLight.CurrentState == TrafficLight.LightState.GreenForCars;
            _crossingStarted = false;
        }

        public void Update()
        {
            // ПРИОРИТЕТ 1: Проверяем сигнал светофора
            if (_crossing.TrafficLight.CurrentState == TrafficLight.LightState.GreenForCars && !_crossingStarted)
            {
                // Если светофор зеленый для машин, значит красный для пешеходов - ждем
                _isWaiting = true;
            }
            // ПРИОРИТЕТ 2: Если светофор красный для машин (зеленый для пешеходов)
            else if (_crossing.TrafficLight.CurrentState == TrafficLight.LightState.RedForCars)
            {
                // Проверяем наличие машин на переходе
                if ((IsOnCrossing() || IsApproachingCrossing()) && IsCarOnCrossing())
                {
                    _isWaiting = true; // Есть машины - ждем, несмотря на зеленый
                }
                else
                {
                    _isWaiting = false;
                    _crossingStarted = true; // Начинаем переход
                }
            }
            
            // Если пешеход перешел дорогу, сбрасываем флаг
            if (HasCrossedRoad())
            {
                _crossingStarted = false;
                _isWaiting = false;
            }

            // Перемещаем пешехода, если он не ждет
            if (!_isWaiting)
            {
                Y += _movingDown ? _speed : -_speed;
                
                // Удаляем пешехода, если он ушел за пределы экрана
                if (Y < 0 || Y > 450)
                {
                    _crossing.RemovePedestrian(this);
                }
            }
        }

        // Пешеход находится на переходе
        private bool IsOnCrossing()
        {
            return X >= _crossing.CrossingX && 
                   X <= _crossing.CrossingX + _crossing.CrossingWidth &&
                   Y >= _crossing.RoadY && 
                   Y <= _crossing.RoadY + _crossing.RoadHeight;
        }
        
        // Пешеход приближается к переходу
        private bool IsApproachingCrossing()
        {
            if (_movingDown)
            {
                return Y < _crossing.RoadY && Y > _crossing.RoadY - 30;
            }
            else
            {
                return Y > _crossing.RoadY + _crossing.RoadHeight && 
                       Y < _crossing.RoadY + _crossing.RoadHeight + 30;
            }
        }
        
        // Пешеход уже перешел дорогу
        private bool HasCrossedRoad()
        {
            if (_movingDown)
            {
                return Y > _crossing.RoadY + _crossing.RoadHeight;
            }
            else
            {
                return Y < _crossing.RoadY;
            }
        }
        
        // Проверка наличия машин на переходе
        private bool IsCarOnCrossing()
        {
            foreach (var car in _crossing.Cars)
            {
                // Машина находится на переходе или очень близко к нему
                if (car.X + 40 >= _crossing.CrossingX && 
                    car.X <= _crossing.CrossingX + _crossing.CrossingWidth + 10)
                {
                    return true;
                }
            }
            return false;
        }
    }
}