﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Backend.Features.Shipments
{
    public interface IShipmentService
    {
        IEnumerable<Shipment> GetAllShipments();
        Shipment? GetShipmentById(int id);
        void AddShipment(Shipment shipment);
        void UpdateShipment(Shipment shipment);
        void DeleteShipment(int id);
    }

    public class ShipmentService
    {

    }
}
