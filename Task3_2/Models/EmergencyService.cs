using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Task3_2.Models
{
    public class EmergencyService : IEmergencyService
    {
        private bool _isBusy;
        public event EventHandler<Car> EmergencyVehicleArrived;
        
        public bool IsBusy => _isBusy;

        public async Task RespondToAccident(double x, double y)
        {
            if (_isBusy) return;
            
            _isBusy = true;
            
            // Создаем аварийную машину
            var emergencyCar = new Car(-100, y, true);
            
            // Уведомляем о создании машины
            EmergencyVehicleArrived?.Invoke(this, emergencyCar);
            
            // Эмулируем время прибытия
            await Task.Delay(3000);
            
            _isBusy = false;
        }
    }
}