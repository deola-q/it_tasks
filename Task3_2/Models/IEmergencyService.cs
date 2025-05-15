using System;
using System.Threading.Tasks;

namespace Task3_2.Models
{
    public interface IEmergencyService
    {
        bool IsBusy { get; }
        event EventHandler<Car> EmergencyVehicleArrived;
        Task RespondToAccident(double x, double y);
    }
}