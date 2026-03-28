using System;

namespace QuantityMeasurementBusinessLayer.Exception
{
    public class QuantityMeasurementException : System.Exception
    {
        public QuantityMeasurementException(string message) : base(message) { }
        public QuantityMeasurementException(string message, System.Exception inner) : base(message, inner) { }
    }
}