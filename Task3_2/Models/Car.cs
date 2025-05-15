using System;

namespace Task3_2.Models
{
    public class Car
    {
        private readonly Random _random = new Random();
        private readonly CrossingModel _crossing;
        
        // Координаты автомобиля
        public double X { get; private set; }
        public double Y { get; private set; }
        
        // Скорость движения
        private double _speed;
        private readonly double _maxSpeed;
        
        // Направление движения (вправо = true, влево = false)
        private bool _movingRight;
        
        // Полоса движения (верхняя = 0, нижняя = 1)
        private int _lane;
        
        // Счетчик времени для попытки смены полосы
        private int _laneChangeCounter;
        
        // Является ли машина аварийной службой
        public bool IsEmergency { get; }

        // Расстояние для правил остановки
        private readonly double _stopDistance = 50; // Единое расстояние остановки

        // Стандартный конструктор для создания машины через модель перекрёстка
        public Car(CrossingModel crossing, bool isEmergency = false)
        {
            _crossing = crossing;
            IsEmergency = isEmergency;
            
            _maxSpeed = isEmergency ? 8 : _random.Next(3, 5);
            
            // Выбираем случайную полосу движения
            _lane = _random.Next(0, 2);
            
            // Направление движения зависит от полосы
            // Верхняя полоса (lane = 0): движение слева направо
            // Нижняя полоса (lane = 1): движение справа налево
            _movingRight = _lane == 0; 
            
            // Машины начинают движение с краев экрана в зависимости от направления
            X = _movingRight ? -50 : 850;
            
            // Размещение на дороге в зависимости от полосы
            double laneOffset = _lane == 0 ? 20 : 60;
            Y = _crossing.RoadY + laneOffset;
            
            _speed = _maxSpeed;
            _laneChangeCounter = 0;
        }
        
        // Перегруженный конструктор для создания машины с конкретными координатами
        public Car(double x, double y, bool isEmergency)
        {
            X = x;
            Y = y;
            IsEmergency = isEmergency;
            
            // Определяем полосу по Y-координате
            _lane = y < 230 ? 0 : 1;
            
            // Направление движения определяется полосой
            _movingRight = _lane == 0;
            
            _maxSpeed = isEmergency ? 8 : _random.Next(3, 5);
            _speed = _maxSpeed;
            _crossing = null; // В этом случае модель перекрёстка не используется
            _laneChangeCounter = 0;
        }

        public void Update()
        {
            // Для машин без перекрёстка просто двигаемся
            if (_crossing == null)
            {
                X += _movingRight ? _speed : -_speed;
                return;
            }

            // По умолчанию едем на максимальной скорости
            _speed = _maxSpeed;

            // Не проверяем условия для аварийных машин, они всегда едут
            if (!IsEmergency)
            {
                // ПРАВИЛО 1: Если горит красный и мы в зоне 20 от перехода, но не проехали критическую зону (10),
                // останавливаемся
                if (_crossing.TrafficLight.CurrentState == TrafficLight.LightState.RedForCars &&
                    IsInStopZone(_stopDistance) && !HasPassedCriticalZone())
                {
                    _speed = 0;
                }
                // ПРАВИЛО 2: Если горит зеленый, но на переходе есть пешеходы, останавливаемся за 20
                else if (_crossing.TrafficLight.CurrentState == TrafficLight.LightState.GreenForCars &&
                         IsPedestrianOnCrossing() &&
                         IsInStopZone(_stopDistance))
                {
                    _speed = 0;
                }
            }

            // Проверяем наличие машин впереди
            Car carAhead = FindCarAhead();
            if (carAhead != null)
            {
                // Вычисляем дистанцию до машины впереди
                double distance = _movingRight
                    ? carAhead.X - X - 40 // 40 - длина машины
                    : X - carAhead.X - 40;

                // Если дистанция меньше безопасной, останавливаемся
                if (distance < 20)
                {
                    _speed = 0; // Полная остановка при очень малой дистанции
                }
            }

            // Движение машины
            X += _movingRight ? _speed : -_speed;

            // Удаляем машину, если она ушла за пределы экрана
            if (X < -60 || X > 860)
            {
                _crossing.RemoveCar(this);
                return;
            }

            // Проверка столкновения с пешеходами
            CheckPedestrianCollision();
        }

        private bool IsInStopZone(double stopDistance)
        {
            if (_crossing == null) return false;

            if (_movingRight)
            {
                double distanceToLine = _crossing.CrossingX - X;
                return distanceToLine <= stopDistance && distanceToLine > 0;
            }
            else
            {
                double distanceToLine = X - (_crossing.CrossingX + _crossing.CrossingWidth);
                return distanceToLine <= stopDistance && distanceToLine > 0;
            }
        }

        private bool HasPassedCriticalZone()
        {
            if (_crossing == null) return false;
        const double criticalDistance = 30; // Критическое расстояние 10

            if (_movingRight)
            {
                return X > _crossing.CrossingX - criticalDistance;
            }
            else
            {
                return X < _crossing.CrossingX + _crossing.CrossingWidth + criticalDistance;
            }
        }

        private Car FindCarAhead()
        {
            Car closest = null;
            double minDistance = double.MaxValue;
            
            foreach (var otherCar in _crossing.Cars)
            {
                if (otherCar != this && otherCar._lane == _lane)
                {
                    // Если другая машина находится впереди в том же направлении
                    if (_movingRight && otherCar.X > X || !_movingRight && otherCar.X < X)
                    {
                        double distance = Math.Abs(X - otherCar.X);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closest = otherCar;
                        }
                    }
                }
            }
            
            return closest;
        }

        private bool IsPedestrianOnCrossing()
        {
            if (_crossing == null) return false;

            foreach (var pedestrian in _crossing.Pedestrians)
            {
                // Проверяем, находится ли пешеход на переходе
                if (pedestrian.X >= _crossing.CrossingX &&
                    pedestrian.X <= _crossing.CrossingX + _crossing.CrossingWidth &&
                    pedestrian.Y >= _crossing.RoadY &&
                    pedestrian.Y <= _crossing.RoadY + _crossing.RoadHeight)
                {
                    return true;
                }
            }

            return false;
        }

        private void CheckPedestrianCollision()
        {
            if (_crossing == null) return;
            
            foreach (var pedestrian in _crossing.Pedestrians)
            {
                // Более точная проверка столкновения
                double dx = Math.Abs((X + 20) - pedestrian.X);
                double dy = Math.Abs((Y + 10) - pedestrian.Y);
                
                // Если пешеход и машина достаточно близко друг к другу (столкновение)
                if (dx < 15 && dy < 10)
                {
                    // Проверяем, находятся ли они на переходе
                    bool onCrossing = pedestrian.X >= _crossing.CrossingX && 
                                      pedestrian.X <= _crossing.CrossingX + _crossing.CrossingWidth;
                    
                    // Столкновение происходит если:
                    // 1. Машина на переходе при красном для машин
                    // 2. Пешеход вне перехода
                    bool collision = (onCrossing && 
                                      _crossing.TrafficLight.CurrentState == TrafficLight.LightState.RedForCars) ||
                                     !onCrossing;
                                     
                    if (collision && _random.Next(0, 100) < 80) // 80% шанс аварии при столкновении
                    {
                        _crossing.TriggerAccident(this);
                        break;
                    }
                }
            }
        }
    }
}